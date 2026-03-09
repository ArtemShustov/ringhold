using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Poly2Tri;
using UnityEngine;

namespace Ringhold.Utils {
	public static class SpriteMesh {
		[Serializable]
		public struct SpriteMeshConfig {
			[Header("Mesh")]
			[Min(0)] public float Depth;

			[Header("Contour")]
			[Range(0, 1f)] public float ContoursDetail;
			[Range(0, 0.2f)] public float DouglasPeuckerDetail;
		}

		public static Mesh Build(Sprite sprite, SpriteMeshConfig config) {
			var contours = GetContours(sprite, config.ContoursDetail, 250, true);
			contours = DouglasPeucker(contours, config.DouglasPeuckerDetail);

			var mesh = Extrude(sprite, contours, config.Depth);
			mesh.name = sprite.name;

			return mesh;
		}

		public static Mesh Extrude(Sprite sprite, Vector2[][] contours, float depth) {

			var pixelsPerUnit = sprite.pixelsPerUnit;

			var (flatVerts, flatTris) = Triangulate(contours);

			var frontVertices = flatVerts.Select(p => new Vector3(p.x, p.y, 0f)).ToArray();
			var backVertices = flatVerts.Select(p => new Vector3(p.x, p.y, depth)).ToArray();

			var frontUVs = flatVerts.Select(p => {
				var u = p.x * pixelsPerUnit / sprite.rect.width + 0.5f;
				var v = p.y * pixelsPerUnit / sprite.rect.height + 0.5f;
				return new Vector2(u, v);
			}).ToArray();

			var backUVs = frontUVs.ToArray();

			var frontTris = FlipTriangles(flatTris);
			var backTris = flatTris.Select(i => i + frontVertices.Length).ToArray();

			var sideVertices = new List<Vector3>();
			var sideTris = new List<int>();
			var sideUVs = new List<Vector2>();

			var sideVertexOffset = frontVertices.Length + backVertices.Length;

			foreach (var contour in contours) {
				var contourLength = contour.Length;
				var cumulativeLength = 0f;

				for (var i = 0; i < contourLength; i++) {
					var current = contour[i];
					var next = contour[(i + 1) % contourLength];

					var segmentLength = Vector2.Distance(current, next);

					var v0 = new Vector3(current.x, current.y, 0f);
					var v1 = new Vector3(next.x, next.y, 0f);
					var v2 = new Vector3(current.x, current.y, depth);
					var v3 = new Vector3(next.x, next.y, depth);

					var baseIndex = sideVertexOffset + sideVertices.Count;

					sideVertices.Add(v0);
					sideVertices.Add(v1);
					sideVertices.Add(v2);
					sideVertices.Add(v3);

					var uLeft = cumulativeLength;
					var uRight = cumulativeLength + segmentLength;

					sideUVs.Add(new Vector2(uLeft, 0f));
					sideUVs.Add(new Vector2(uRight, 0f));
					sideUVs.Add(new Vector2(uLeft, depth));
					sideUVs.Add(new Vector2(uRight, depth));

					sideTris.Add(baseIndex + 0);
					sideTris.Add(baseIndex + 1);
					sideTris.Add(baseIndex + 2);

					sideTris.Add(baseIndex + 1);
					sideTris.Add(baseIndex + 3);
					sideTris.Add(baseIndex + 2);

					cumulativeLength += segmentLength;
				}
			}

			var allVertices = frontVertices
				.Concat(backVertices)
				.Concat(sideVertices)
				.ToArray();

			var allUVs = frontUVs
				.Concat(backUVs)
				.Concat(sideUVs)
				.ToArray();

			var faceTris = frontTris
				.Concat(backTris)
				.ToArray();

			var sideTrisFinal = sideTris.ToArray();

			var mesh = new Mesh();
			mesh.vertices = allVertices;
			mesh.uv = allUVs;
			mesh.subMeshCount = 2;
			mesh.SetTriangles(faceTris, 0);
			mesh.SetTriangles(sideTrisFinal, 1);
			mesh.RecalculateNormals();
			mesh.RecalculateBounds();

			return mesh;
		}

		private static int[] FlipTriangles(int[] triangles) {
			var result = new int[triangles.Length];
			for (var i = 0; i < triangles.Length; i += 3) {
				result[i] = triangles[i];
				result[i + 1] = triangles[i + 2];
				result[i + 2] = triangles[i + 1];
			}
			return result;
		}

		public static Vector2[][] GetContours(Sprite sprite, float detail, byte alphaTolerance, bool detectHoles) {
			var generateOutlineMethodInfo = typeof(UnityEditor.Sprites.SpriteUtility).GetMethod(
				"GenerateOutlineFromSprite",
				BindingFlags.NonPublic | BindingFlags.Static
			);
			var newDetail = Mathf.Pow(detail, 3);
			var parameters = new object[] { sprite, newDetail, alphaTolerance, detectHoles, null };
			generateOutlineMethodInfo?.Invoke(null, parameters);

			return (Vector2[][])parameters[4];
		}

		public static Vector2[][] DouglasPeucker(Vector2[][] contours, float epsilon) {
			return Array.ConvertAll(contours, contour => SimplifyContour(contour, epsilon));

			static Vector2[] SimplifyContour(Vector2[] points, float epsilon) {
				if (points.Length < 3) {
					return points;
				}

				var maxDistance = 0f;
				var maxIndex = 0;

				for (int i = 1; i < points.Length - 1; i++) {
					var distance = PerpendicularDistance(points[i], points[0], points[^1]);
					if (distance > maxDistance) {
						maxDistance = distance;
						maxIndex = i;
					}
				}

				if (maxDistance > epsilon) {
					var left = SimplifyContour(points[..(maxIndex + 1)], epsilon);
					var right = SimplifyContour(points[maxIndex..], epsilon);

					var result = new Vector2[left.Length + right.Length - 1];
					Array.Copy(left, result, left.Length);
					Array.Copy(right, 1, result, left.Length, right.Length - 1);
					return result;
				}

				return new[] { points[0], points[^1] };
			}

			static float PerpendicularDistance(Vector2 point, Vector2 lineStart, Vector2 lineEnd) {
				var dx = lineEnd.x - lineStart.x;
				var dy = lineEnd.y - lineStart.y;

				var length = Mathf.Sqrt(dx * dx + dy * dy);
				if (length < Mathf.Epsilon) {
					return Vector2.Distance(point, lineStart);
				}

				return Mathf.Abs(dy * point.x - dx * point.y + lineEnd.x * lineStart.y - lineEnd.y * lineStart.x) / length;
			}
		}

		public static (Vector2[] verts, int[] tris) Triangulate(Vector2[][] contours) {
			var outerPoints = contours[0].Select(p => new PolygonPoint(p.x, p.y)).ToList();
			var polygon = new Polygon(outerPoints);

			for (var i = 1; i < contours.Length; i++) {
				if (contours[i].Length < 3) {
					continue;
				}
				var holePoints = contours[i].Select(p => new PolygonPoint(p.x, p.y)).ToList();
				var hole = new Polygon(holePoints);
				polygon.AddHole(hole);
			}

			var ctx = new DTSweepContext();
			ctx.PrepareTriangulation(polygon);
			DTSweep.Triangulate(ctx);

			var vertList = new List<Vector2>();
			var triList = new List<int>();

			foreach (var tri in polygon.Triangles) {
				foreach (var tp in tri.Points) {
					triList.Add(vertList.Count);
					vertList.Add(new Vector2((float)tp.X, (float)tp.Y));
				}
			}

			return (vertList.ToArray(), triList.ToArray());
		}
	}
}