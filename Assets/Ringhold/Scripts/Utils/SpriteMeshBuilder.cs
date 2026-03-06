using System.Collections.Generic;
using UnityEngine;

namespace Ringhold.Utils {
	public static class SpriteMeshBuilder {
		public static Mesh Build(Texture2D frontTexture, float depth, float alphaThreshold, float pixelsPerUnit, float simplifyTolerance) {
			var width = frontTexture.width;
			var height = frontTexture.height;
			var pixels = frontTexture.GetPixels32();
			var visited = new bool[width * height];
			var outerPixels = new List<Vector2Int>();
			var holesPixels = new List<List<Vector2Int>>();

			TraceAllContours(pixels, width, height, visited, outerPixels, holesPixels, alphaThreshold);

			if (outerPixels.Count < 3) {
				Debug.LogError("[SpriteMeshBuilder] Could not extract outer contour.");
				return null;
			}

			var outer2d = ToWorld(outerPixels, width, height, pixelsPerUnit);
			outer2d = DouglasPeucker(outer2d, simplifyTolerance / pixelsPerUnit);
			if (SignedArea(outer2d) < 0f) {
				outer2d.Reverse();
			}

			var holePolys = new List<List<Vector2>>();
			foreach (var hp in holesPixels) {
				var h = ToWorld(hp, width, height, pixelsPerUnit);
				h = DouglasPeucker(h, simplifyTolerance / pixelsPerUnit);
				if (SignedArea(h) > 0f) {
					h.Reverse();
				}
				holePolys.Add(h);
			}

			var merged = MergeWithHoles(outer2d, holePolys);
			var frontTris = EarClip(merged);

			return BuildMesh(frontTexture, merged, frontTris, outer2d, holePolys, depth, pixelsPerUnit);
		}

		private static float SignedArea(List<Vector2> poly) {
			var area = 0f;
			var n = poly.Count;
			for (var i = 0; i < n; i++) {
				var a = poly[i];
				var b = poly[(i + 1) % n];
				area += (a.x * b.y) - (b.x * a.y);
			}
			return area * 0.5f;
		}

		private static Mesh BuildMesh(Texture2D frontTexture, List<Vector2> contour, List<int> frontTris, List<Vector2> outerLoop, List<List<Vector2>> holeLoops, float depth, float pixelsPerUnit) {
			var mesh = new Mesh();
			mesh.name = "SpriteMesh";

			var texW = (float)frontTexture.width;
			var texH = (float)frontTexture.height;
			var count = contour.Count;

			var allVerts = new List<Vector3>();
			var allUVs = new List<Vector2>();
			var allNormals = new List<Vector3>();

			for (var i = 0; i < count; i++) {
				var p = contour[i];
				allVerts.Add(new Vector3(p.x, p.y, 0f));
				allUVs.Add(new Vector2(p.x * pixelsPerUnit / texW + 0.5f, p.y * pixelsPerUnit / texH + 0.5f));
				allNormals.Add(Vector3.back);
			}

			var backOffset = count;
			for (var i = 0; i < count; i++) {
				var p = contour[i];
				allVerts.Add(new Vector3(p.x, p.y, depth));
				allUVs.Add(new Vector2(p.x * pixelsPerUnit / texW + 0.5f, p.y * pixelsPerUnit / texH + 0.5f));
				allNormals.Add(Vector3.forward);
			}

			var frontTrisFlipped = new List<int>(frontTris.Count);
			for (var i = 0; i < frontTris.Count; i += 3) {
				frontTrisFlipped.Add(frontTris[i + 0]);
				frontTrisFlipped.Add(frontTris[i + 2]);
				frontTrisFlipped.Add(frontTris[i + 1]);
			}

			var backTris = new List<int>(frontTris.Count);
			for (var i = 0; i < frontTris.Count; i += 3) {
				backTris.Add(backOffset + frontTris[i + 0]);
				backTris.Add(backOffset + frontTris[i + 1]);
				backTris.Add(backOffset + frontTris[i + 2]);
			}

			var sideTris = new List<int>();
			var allLoops = new List<List<Vector2>>();
			allLoops.Add(outerLoop);
			allLoops.AddRange(holeLoops);

			foreach (var loop in allLoops) {
				var loopCount = loop.Count;
				var runU = 0f;

				for (var i = 0; i < loopCount; i++) {
					var next = (i + 1) % loopCount;
					var p0 = loop[i];
					var p1 = loop[next];

					var edgeLen = Vector2.Distance(p0, p1);

					if (edgeLen < 1e-6f) {
						runU += edgeLen;
						continue;
					}

					var v00 = new Vector3(p0.x, p0.y, 0f);
					var v01 = new Vector3(p0.x, p0.y, depth);
					var v10 = new Vector3(p1.x, p1.y, 0f);
					var v11 = new Vector3(p1.x, p1.y, depth);

					var edge = v10 - v00;
					var depthDir = v01 - v00;
					var sideNormal = Vector3.Cross(edge, depthDir).normalized;

					var u0 = runU;
					var u1 = runU + edgeLen;

					var vi = allVerts.Count;

					allVerts.Add(v00);
					allVerts.Add(v01);
					allVerts.Add(v10);
					allVerts.Add(v11);

					allUVs.Add(new Vector2(u0, 0f));
					allUVs.Add(new Vector2(u0, depth));
					allUVs.Add(new Vector2(u1, 0f));
					allUVs.Add(new Vector2(u1, depth));

					allNormals.Add(sideNormal);
					allNormals.Add(sideNormal);
					allNormals.Add(sideNormal);
					allNormals.Add(sideNormal);

					sideTris.Add(vi + 0);
					sideTris.Add(vi + 2);
					sideTris.Add(vi + 1);

					sideTris.Add(vi + 2);
					sideTris.Add(vi + 3);
					sideTris.Add(vi + 1);

					runU += edgeLen;
				}
			}

			mesh.SetVertices(allVerts);
			mesh.SetUVs(0, allUVs);
			mesh.SetNormals(allNormals);
			mesh.subMeshCount = 3;
			mesh.SetTriangles(frontTrisFlipped, 0);
			mesh.SetTriangles(backTris, 1);
			mesh.SetTriangles(sideTris, 2);
			mesh.RecalculateBounds();

			return mesh;
		}

		private static void TraceAllContours(Color32[] pixels, int width, int height, bool[] visited, List<Vector2Int> outer, List<List<Vector2Int>> holes, float alphaThreshold) {
			var byteThreshold = (byte)(alphaThreshold * 255f);

			for (var y = 0; y < height; y++) {
				for (var x = 0; x < width; x++) {
					var idx = y * width + x;
					if (visited[idx] || pixels[idx].a < byteThreshold) {
						continue;
					}
					var isOuter = x == 0 || pixels[y * width + (x - 1)].a < byteThreshold;
					if (!isOuter) {
						continue;
					}

					var contour = TraceContour(pixels, width, height, x, y, byteThreshold);
					MarkVisited(contour, visited, width);

					if (outer.Count == 0) {
						outer.AddRange(contour);
					} else {
						holes.Add(contour);
					}
				}
			}
		}

		private static List<Vector2Int> TraceContour(Color32[] pixels, int width, int height, int startX, int startY, byte threshold) {
			var dirs = new Vector2Int[] {
				new Vector2Int( 1,  0),
				new Vector2Int( 1, -1),
				new Vector2Int( 0, -1),
				new Vector2Int(-1, -1),
				new Vector2Int(-1,  0),
				new Vector2Int(-1,  1),
				new Vector2Int( 0,  1),
				new Vector2Int( 1,  1),
			};

			var start = new Vector2Int(startX, startY);
			var contour = new List<Vector2Int>();
			var current = start;
			var prevDir = 4;

			do {
				contour.Add(current);
				var startSearch = (prevDir + 6) % 8;
				var moved = false;

				for (var k = 0; k < 8; k++) {
					var di = (startSearch + k) % 8;
					var next = current + dirs[di];
					if (next.x < 0 || next.x >= width || next.y < 0 || next.y >= height) {
						continue;
					}
					if (pixels[next.y * width + next.x].a >= threshold) {
						prevDir = di;
						current = next;
						moved = true;
						break;
					}
				}

				if (!moved) {
					break;
				}
			} while (current != start && contour.Count < width * height);

			return contour;
		}

		private static void MarkVisited(List<Vector2Int> contour, bool[] visited, int width) {
			foreach (var p in contour) {
				visited[p.y * width + p.x] = true;
			}
		}

		private static List<Vector2> ToWorld(List<Vector2Int> contour, int texW, int texH, float pixelsPerUnit) {
			var result = new List<Vector2>(contour.Count);
			var cx = texW / 2f;
			var cy = texH / 2f;
			foreach (var p in contour) {
				result.Add(new Vector2((p.x - cx) / pixelsPerUnit, (p.y - cy) / pixelsPerUnit));
			}
			return result;
		}

		private static List<Vector2> DouglasPeucker(List<Vector2> points, float epsilon) {
			if (points.Count < 3) {
				return points;
			}

			var maxDist = 0f;
			var maxIdx = 0;
			var end = points.Count - 1;

			for (var i = 1; i < end; i++) {
				var d = PerpendicularDistance(points[i], points[0], points[end]);
				if (d > maxDist) {
					maxDist = d;
					maxIdx = i;
				}
			}

			if (maxDist > epsilon) {
				var left = DouglasPeucker(points.GetRange(0, maxIdx + 1), epsilon);
				var right = DouglasPeucker(points.GetRange(maxIdx, points.Count - maxIdx), epsilon);
				left.RemoveAt(left.Count - 1);
				left.AddRange(right);
				return left;
			}

			return new List<Vector2> { points[0], points[end] };
		}

		private static float PerpendicularDistance(Vector2 p, Vector2 a, Vector2 b) {
			var ab = b - a;
			if (ab.sqrMagnitude < 1e-10f) {
				return Vector2.Distance(p, a);
			}
			var t = Vector2.Dot(p - a, ab) / ab.sqrMagnitude;
			return Vector2.Distance(p, a + ab * Mathf.Clamp01(t));
		}

		private static List<Vector2> MergeWithHoles(List<Vector2> outer, List<List<Vector2>> holes) {
			var polygon = new List<Vector2>(outer);

			foreach (var hole in holes) {
				var bestOuterIdx = 0;
				var bestHoleIdx = 0;
				var bestDist = float.MaxValue;

				for (var oi = 0; oi < polygon.Count; oi++) {
					for (var hi = 0; hi < hole.Count; hi++) {
						var d = Vector2.Distance(polygon[oi], hole[hi]);
						if (d < bestDist) {
							bestDist = d;
							bestOuterIdx = oi;
							bestHoleIdx = hi;
						}
					}
				}

				var merged = new List<Vector2>();
				for (var i = 0; i <= bestOuterIdx; i++) {
					merged.Add(polygon[i]);
				}
				for (var i = 0; i <= hole.Count; i++) {
					merged.Add(hole[(bestHoleIdx + i) % hole.Count]);
				}
				merged.Add(polygon[bestOuterIdx]);
				for (var i = bestOuterIdx + 1; i < polygon.Count; i++) {
					merged.Add(polygon[i]);
				}

				polygon = merged;
			}

			return polygon;
		}

		private static List<int> EarClip(List<Vector2> polygon) {
			var tris = new List<int>();
			var indices = new List<int>(polygon.Count);
			for (var i = 0; i < polygon.Count; i++) {
				indices.Add(i);
			}

			var safety = 0;
			var maxIter = polygon.Count * polygon.Count + 10;

			while (indices.Count > 3 && safety < maxIter) {
				safety++;
				var earFound = false;

				for (var i = 0; i < indices.Count; i++) {
					var prev = indices[(i - 1 + indices.Count) % indices.Count];
					var curr = indices[i];
					var next = indices[(i + 1) % indices.Count];

					var a = polygon[prev];
					var b = polygon[curr];
					var c = polygon[next];

					if (CrossZ(a, b, c) <= 0f) {
						continue;
					}

					var isEar = true;
					for (var j = 0; j < indices.Count; j++) {
						var idx = indices[j];
						if (idx == prev || idx == curr || idx == next) {
							continue;
						}
						if (PointInTriangle(polygon[idx], a, b, c)) {
							isEar = false;
							break;
						}
					}

					if (!isEar) {
						continue;
					}

					tris.Add(prev);
					tris.Add(curr);
					tris.Add(next);
					indices.RemoveAt(i);
					earFound = true;
					break;
				}

				if (!earFound) {
					break;
				}
			}

			if (indices.Count == 3) {
				tris.Add(indices[0]);
				tris.Add(indices[1]);
				tris.Add(indices[2]);
			}

			return tris;
		}

		private static float CrossZ(Vector2 o, Vector2 a, Vector2 b) {
			return (a.x - o.x) * (b.y - o.y) - (a.y - o.y) * (b.x - o.x);
		}

		private static bool PointInTriangle(Vector2 p, Vector2 a, Vector2 b, Vector2 c) {
			var d1 = CrossZ(p, a, b);
			var d2 = CrossZ(p, b, c);
			var d3 = CrossZ(p, c, a);
			var hasNeg = d1 < 0f || d2 < 0f || d3 < 0f;
			var hasPos = d1 > 0f || d2 > 0f || d3 > 0f;
			return !(hasNeg && hasPos);
		}
	}
}