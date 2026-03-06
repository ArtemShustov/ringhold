using UnityEngine;

namespace Ringhold.Utils {
	[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
	public class SpriteMeshGenerator : MonoBehaviour {
		[SerializeField] private Texture2D _frontTexture;
		[SerializeField] private Texture2D _backTexture;
		[SerializeField] private Material _facesMaterialRef;
		[SerializeField] private Material _sideMaterial;
		[SerializeField] private float _depth = 0.1f;
		[SerializeField] private float _alphaThreshold = 0.1f;
		[SerializeField] private float _pixelsPerUnit = 256f;
		[Range(0f, 20f)]
		[SerializeField] private float _simplifyTolerance = 2f;

		private MeshFilter _meshFilter;
		private MeshRenderer _meshRenderer;

		private void Awake() {
			_meshFilter = GetComponent<MeshFilter>();
			_meshRenderer = GetComponent<MeshRenderer>();
			GenerateMesh();
		}

		private void GenerateMesh() {
			if (_frontTexture == null) {
				Debug.LogError("[SpriteMeshGenerator] Front texture is not assigned.");
				return;
			}

			var mesh = SpriteMeshBuilder.Build(_frontTexture, _depth, _alphaThreshold, _pixelsPerUnit, _simplifyTolerance);
			if (mesh == null) {
				return;
			}

			_meshFilter.mesh = mesh;

			var frontMat = _facesMaterialRef != null ? new Material(_facesMaterialRef) : new Material(Shader.Find("Standard"));
			frontMat.mainTexture = _frontTexture;
			var backMat = new Material(frontMat);
			backMat.mainTexture = _backTexture != null ? _backTexture : _frontTexture;

			_meshRenderer.materials = new Material[] { frontMat, backMat, _sideMaterial };
		}
	}
}