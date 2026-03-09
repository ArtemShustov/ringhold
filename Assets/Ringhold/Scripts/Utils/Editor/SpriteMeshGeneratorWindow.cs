using UnityEditor;
using UnityEngine;
using Ringhold.Utils;

namespace Ringhold.Utils.Editor {
	public class SpriteMeshGeneratorWindow : EditorWindow {
		private const string PrefDepth = "SpriteMeshGen_Depth";
		private const string PrefContoursDetail = "SpriteMeshGen_ContoursDetail";
		private const string PrefDouglasPeuckerDetail = "SpriteMeshGen_DouglasPeuckerDetail";
		private const string PrefFacesMaterial = "SpriteMeshGen_FacesMaterial";
		private const string PrefSideMaterial = "SpriteMeshGen_SideMaterial";

		private Sprite _sprite;
		private Material _facesMaterialRef;
		private Material _sideMaterial;
		private SpriteMesh.SpriteMeshConfig _config;

		[MenuItem("Tools/Sprite Mesh Generator")]
		private static void Open() {
			GetWindow<SpriteMeshGeneratorWindow>("Sprite Mesh Generator");
		}

		private void OnEnable() {
			_config.Depth = EditorPrefs.GetFloat(PrefDepth, 0.1f);
			_config.ContoursDetail = EditorPrefs.GetFloat(PrefContoursDetail, 1f);
			_config.DouglasPeuckerDetail = EditorPrefs.GetFloat(PrefDouglasPeuckerDetail, 0.01f);

			var facesMaterialPath = EditorPrefs.GetString(PrefFacesMaterial, "");
			if (!string.IsNullOrEmpty(facesMaterialPath)) {
				_facesMaterialRef = AssetDatabase.LoadAssetAtPath<Material>(facesMaterialPath);
			}

			var sideMaterialPath = EditorPrefs.GetString(PrefSideMaterial, "");
			if (!string.IsNullOrEmpty(sideMaterialPath)) {
				_sideMaterial = AssetDatabase.LoadAssetAtPath<Material>(sideMaterialPath);
			}
		}

		private void OnDisable() {
			EditorPrefs.SetFloat(PrefDepth, _config.Depth);
			EditorPrefs.SetFloat(PrefContoursDetail, _config.ContoursDetail);
			EditorPrefs.SetFloat(PrefDouglasPeuckerDetail, _config.DouglasPeuckerDetail);

			var facesMaterialPath = _facesMaterialRef != null ? AssetDatabase.GetAssetPath(_facesMaterialRef) : "";
			EditorPrefs.SetString(PrefFacesMaterial, facesMaterialPath);

			var sideMaterialPath = _sideMaterial != null ? AssetDatabase.GetAssetPath(_sideMaterial) : "";
			EditorPrefs.SetString(PrefSideMaterial, sideMaterialPath);
		}

		private void OnGUI() {
			EditorGUILayout.LabelField("Sprite", EditorStyles.boldLabel);
			_sprite = (Sprite)EditorGUILayout.ObjectField("Sprite", _sprite, typeof(Sprite), false);

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Materials", EditorStyles.boldLabel);
			_facesMaterialRef = (Material)EditorGUILayout.ObjectField("Faces Material", _facesMaterialRef, typeof(Material), false);
			_sideMaterial = (Material)EditorGUILayout.ObjectField("Side Material", _sideMaterial, typeof(Material), false);

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Settings", EditorStyles.boldLabel);
			_config.Depth = EditorGUILayout.FloatField("Depth", _config.Depth);
			_config.ContoursDetail = EditorGUILayout.Slider("Contours Detail", _config.ContoursDetail, 0f, 1f);
			_config.DouglasPeuckerDetail = EditorGUILayout.Slider("Douglas Peucker Detail", _config.DouglasPeuckerDetail, 0f, 0.2f);

			EditorGUILayout.Space();

			var canGenerate = _sprite != null;
			EditorGUI.BeginDisabledGroup(!canGenerate);
			if (GUILayout.Button("Generate & Save")) {
				Generate();
			}
			EditorGUI.EndDisabledGroup();

			if (!canGenerate) {
				EditorGUILayout.HelpBox("Assign a Sprite to generate.", MessageType.Info);
			}
		}

		private void Generate() {
			var texName = _sprite.name;
			var absPath = EditorUtility.SaveFilePanelInProject("Save Prefab", texName, "prefab", "Choose where to save the prefab", "Assets");
			if (string.IsNullOrEmpty(absPath)) {
				return;
			}

			var mesh = SpriteMesh.Build(_sprite, _config);
			if (mesh == null) {
				Debug.LogError("[SpriteMeshGeneratorWindow] Mesh generation failed.");
				return;
			}

			var prefabPath = absPath;

			var frontMat = _facesMaterialRef != null ? new Material(_facesMaterialRef) : new Material(Shader.Find("Standard"));
			frontMat.mainTexture = _sprite.texture;
			frontMat.name = $"{texName}_Front";

			var go = new GameObject(texName);
			go.AddComponent<MeshFilter>().sharedMesh = mesh;
			go.AddComponent<MeshRenderer>().sharedMaterials = new Material[] { frontMat, _sideMaterial };

			var prefab = PrefabUtility.SaveAsPrefabAsset(go, prefabPath);
			DestroyImmediate(go);

			AssetDatabase.AddObjectToAsset(mesh, prefab);
			AssetDatabase.AddObjectToAsset(frontMat, prefab);

			prefab.GetComponent<MeshFilter>().sharedMesh = mesh;
			prefab.GetComponent<MeshRenderer>().sharedMaterials = new Material[] { frontMat, _sideMaterial };

			PrefabUtility.SavePrefabAsset(prefab);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();

			EditorUtility.FocusProjectWindow();
			Selection.activeObject = prefab;

			Debug.Log($"[SpriteMeshGeneratorWindow] Saved prefab to {prefabPath}");
		}
	}
}