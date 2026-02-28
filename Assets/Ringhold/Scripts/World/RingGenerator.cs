using System.Collections.Generic;
using System.Linq;
using Pathfinding;
using UnityEngine;

namespace Ringhold.Scripts.World {
	public class RingGenerator : MonoBehaviour {
		[Header("Settings")]
		[SerializeField] private int _partCount = 16;
		[SerializeField] private float _partLength = 20f;
		[SerializeField] private float _partWidth = 10f;
		[SerializeField] private Ring _ring;
		[SerializeField] private AstarPath _astar;
		[Header("Nav Graph")]
		[SerializeField] private float _agentHeight = 2f;
		[SerializeField] private float _agentRadius = 0.5f;
		[SerializeField] private float _maxSlope = 30f;
		[Header("Prefabs")]
		[SerializeField] private GameObject _partPrefab;

		private void Awake() {
			Generate();
		}

		public void Generate() {
			BuildRing();
			BuildNavMesh();
		}

		private void BuildRing() {
			if (_partPrefab == null || !_ring || _partCount <= 0) {
				return;
			}

			foreach (Transform child in transform) {
				DestroyImmediate(child.gameObject);
			}

			_ring.Radius = _partLength / (2f * Mathf.Sin(Mathf.PI / _partCount));
			foreach (var (pointA, pointB) in GetRingSections(_partLength, _partCount)) {
				var direction = (pointB - pointA).normalized;
				var toCenter = (transform.position - pointA).normalized;
				var rotation = Quaternion.LookRotation(direction, toCenter);
				
				var part = Instantiate(_partPrefab, transform);
				part.transform.position = pointA;
				part.transform.rotation = rotation;
			}
		}
		private void BuildNavMesh() {
			if (_astar == null || _partCount <= 0) {
				return;
			}
			
			_astar.data.graphs = new NavGraph[0];
			if (_astar.data.AddGraph(typeof(NavMeshGraph)) is not NavMeshGraph graph) {
				return;
			}

			graph.sourceMesh = CreateMesh();

			AstarPath.active.Scan();

			Mesh CreateMesh() {
				var sections = GetRingSections(_partLength, _partCount).ToArray();
				var vertices = new Vector3[_partCount * 2];
				var triangles = new int[_partCount * 6];

				for (var i = 0; i < _partCount; i++) {
					var (pointA, pointB) = sections[i];
					var midpoint = (pointA + pointB) * 0.5f;
					
					var toCenter = (transform.position - midpoint).normalized;
					var toNext = (pointB - pointA).normalized;

					var right = Vector3.Cross(toCenter, toNext).normalized;
					var offset = right * (_partWidth * 0.5f);

					vertices[i * 2] = pointA - offset;
					vertices[i * 2 + 1] = pointA + offset;
				}

				for (var i = 0; i < _partCount; i++) {
					var a = i * 2;
					var b = i * 2 + 1;
					var c = ((i + 1) % _partCount) * 2;
					var d = ((i + 1) % _partCount) * 2 + 1;

					triangles[i * 6 + 0] = a;
					triangles[i * 6 + 1] = b;
					triangles[i * 6 + 2] = c;
					triangles[i * 6 + 3] = b;
					triangles[i * 6 + 4] = d;
					triangles[i * 6 + 5] = c;
				}

				var mesh = new Mesh();
				mesh.name = "RingNavMesh";
				mesh.vertices = vertices;
				mesh.triangles = triangles;
				mesh.RecalculateNormals();
				return mesh;
			}
		}

		private IEnumerable<(Vector3, Vector3)> GetRingSections(float pathLength, int partCount) {
			var radius = pathLength / (2f * Mathf.Sin(Mathf.PI / partCount));
			var angleStep = 360f / partCount;

			for (var i = 0; i < partCount; i++) {
				var angleA = i * angleStep * Mathf.Deg2Rad;
				var angleB = (i + 1) * angleStep * Mathf.Deg2Rad;

				var pointA = transform.position + new Vector3(
					Mathf.Cos(angleA) * radius,
					Mathf.Sin(angleA) * radius,
					0f
				);
				var pointB = transform.position + new Vector3(
					Mathf.Cos(angleB) * radius,
					Mathf.Sin(angleB) * radius,
					0f
				);

				yield return (pointA, pointB);
			}
		}
		
		private void OnDrawGizmos() {
			if (!_ring || _partCount <= 0 || _partLength <= 0f) {
				return;
			}

			Gizmos.color = Color.white;
			foreach (var (pointA, pointB) in GetRingSections(_partLength, _partCount)) {
				Gizmos.DrawSphere(pointA, 0.3f);
				Gizmos.DrawLine(pointA, pointB);
			}
		}
	}
}