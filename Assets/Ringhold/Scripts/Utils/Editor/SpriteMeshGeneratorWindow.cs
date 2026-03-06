using UnityEditor;
using UnityEngine;

namespace Ringhold.Utils.Editor {
	public class SpriteMeshGeneratorWindow : EditorWindow {
		private Texture2D _frontTexture;
		private Texture2D _backTexture;
		private Material _facesMaterialRef;
		private Material _sideMaterial;
		private float _depth = 0.1f;
		private float _alphaThreshold = 0.1f;
		private float _pixelsPerUnit = 256f;
		private float _simplifyTolerance = 2f;

		[MenuItem("Tools/Sprite Mesh Generator")]
		private static void Open() {
			GetWindow<SpriteMeshGeneratorWindow>("Sprite Mesh Generator");
		}

		private void OnGUI() {
			EditorGUILayout.LabelField("Textures", EditorStyles.boldLabel);
			_frontTexture = (Texture2D)EditorGUILayout.ObjectField("Front Texture", _frontTexture, typeof(Texture2D), false);
			_backTexture = (Texture2D)EditorGUILayout.ObjectField("Back Texture", _backTexture, typeof(Texture2D), false);

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Materials", EditorStyles.boldLabel);
			_facesMaterialRef = (Material)EditorGUILayout.ObjectField("Faces Material", _facesMaterialRef, typeof(Material), false);
			_sideMaterial = (Material)EditorGUILayout.ObjectField("Side Material", _sideMaterial, typeof(Material), false);

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Settings", EditorStyles.boldLabel);
			_depth = EditorGUILayout.FloatField("Depth", _depth);
			_alphaThreshold = EditorGUILayout.Slider("Alpha Threshold", _alphaThreshold, 0f, 1f);
			_pixelsPerUnit = EditorGUILayout.FloatField("Pixels Per Unit", _pixelsPerUnit);
			_simplifyTolerance = EditorGUILayout.Slider("Simplify Tolerance", _simplifyTolerance, 0f, 20f);

			EditorGUILayout.Space();

			var canGenerate = _frontTexture != null;
			EditorGUI.BeginDisabledGroup(!canGenerate);
			if (GUILayout.Button("Generate & Save")) {
				Generate();
			}
			EditorGUI.EndDisabledGroup();

			if (!canGenerate) {
				EditorGUILayout.HelpBox("Assign a Front Texture to generate.", MessageType.Info);
			}
		}

		private void Generate() {
			var texName = _frontTexture.name;
			var absPath = EditorUtility.SaveFilePanelInProject("Save Prefab", texName, "prefab", "Choose where to save the prefab", "Assets");
			if (string.IsNullOrEmpty(absPath)) {
				return;
			}

			var mesh = SpriteMeshBuilder.Build(_frontTexture, _depth, _alphaThreshold, _pixelsPerUnit, _simplifyTolerance);
			if (mesh == null) {
				Debug.LogError("[SpriteMeshGeneratorWindow] Mesh generation failed.");
				return;
			}

			var prefabPath = absPath;

			var frontMat = _facesMaterialRef != null ? new Material(_facesMaterialRef) : new Material(Shader.Find("Standard"));
			frontMat.mainTexture = _frontTexture;
			frontMat.name = $"{texName}_Front";

			var backMat = new Material(frontMat);
			backMat.mainTexture = _backTexture != null ? _backTexture : _frontTexture;
			backMat.name = $"{texName}_Back";

			var go = new GameObject(texName);
			go.AddComponent<MeshFilter>().sharedMesh = mesh;
			go.AddComponent<MeshRenderer>().sharedMaterials = new Material[] { frontMat, backMat, _sideMaterial };

			var prefab = PrefabUtility.SaveAsPrefabAsset(go, prefabPath);
			DestroyImmediate(go);

			AssetDatabase.AddObjectToAsset(mesh, prefab);
			AssetDatabase.AddObjectToAsset(frontMat, prefab);
			AssetDatabase.AddObjectToAsset(backMat, prefab);

			prefab.GetComponent<MeshFilter>().sharedMesh = mesh;
			prefab.GetComponent<MeshRenderer>().sharedMaterials = new Material[] { frontMat, backMat, _sideMaterial };

			PrefabUtility.SavePrefabAsset(prefab);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();

			EditorUtility.FocusProjectWindow();
			Selection.activeObject = prefab;

			Debug.Log($"[SpriteMeshGeneratorWindow] Saved prefab to {prefabPath}");
		}

	}
}