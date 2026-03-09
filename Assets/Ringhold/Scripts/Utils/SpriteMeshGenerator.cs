using System.Linq;
using UnityEngine;

namespace Ringhold.Utils {
	[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
	public class SpriteMeshGenerator : MonoBehaviour {
		[SerializeField] private Sprite _frontTexture;
		[SerializeField, Min(0)] private float _depth = 0.1f;
		[SerializeField, Range(0, 1f)] private float _contoursDetail = 1;
		[SerializeField, Range(0, 0.1f)] private float _douglasPeuckerDetail = 0.02f;

		private MeshFilter _meshFilter;
		private MeshRenderer _meshRenderer;

		private void Awake() {
			_meshFilter = GetComponent<MeshFilter>();
			_meshRenderer = GetComponent<MeshRenderer>();
		}

		private void OnDrawGizmosSelected() {
			_meshFilter = GetComponent<MeshFilter>();
			_meshRenderer = GetComponent<MeshRenderer>();
			
			var conts = SpriteMesh.GetContours(
				_frontTexture,
				_contoursDetail,
				250,
				true
			);
			conts = SpriteMesh.DouglasPeucker(conts, _douglasPeuckerDetail);
			foreach (Vector2[] cont in conts) {
				Gizmos.DrawLine(transform.position + (Vector3)cont.First(), transform.position + (Vector3)cont.Last());
				for (int i = 0; i < cont.Length - 1; i++) {
					var pos1 = cont[i];
					var pos2 = cont[i + 1];
					Gizmos.DrawLine(transform.position + (Vector3)pos1, transform.position + (Vector3)pos2);
				}
				
				foreach (var pos in cont) {
					Gizmos.DrawSphere(transform.position + (Vector3)pos, 0.03f);
				}
			}
			
			_meshFilter.mesh = SpriteMesh.Build(_frontTexture, new SpriteMesh.SpriteMeshConfig {
				Depth = _depth,
				ContoursDetail = _contoursDetail,
				DouglasPeuckerDetail = _douglasPeuckerDetail,
			});
		}
	}
}