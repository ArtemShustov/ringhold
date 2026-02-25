using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace Core.Boot {
#if UNITY_EDITOR
	 [InitializeOnLoad]
#endif
	 public static class GameEntryPoint {
#if UNITY_EDITOR
		 private const string MenuPath = "Tools/Auto load first scene";
		 private static string _targetScene;

		 static GameEntryPoint() {
			 EditorApplication.delayCall += () => {
				 Menu.SetChecked(MenuPath, AutoBoot);
				 
				 if (AutoBoot) {
					 EditorSceneManager.playModeStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(
						 EditorBuildSettings.scenes[0].path
					 );
				 } else {
					 EditorSceneManager.playModeStartScene = null;
				 }
			 };
		 }

		 [MenuItem(MenuPath)]
		 private static void ToggleLoadFirstScene() {
			 AutoBoot = !AutoBoot;
			 Menu.SetChecked(MenuPath, AutoBoot);
			 
			 if (AutoBoot) {
				 EditorSceneManager.playModeStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(
					 EditorBuildSettings.scenes[0].path
				 );
			 } else {
				 EditorSceneManager.playModeStartScene = null;
			 }
		 }

		 private static bool AutoBoot {
			 get => EditorPrefs.GetBool("AutoBoot", true);
			 set => EditorPrefs.SetBool("AutoBoot", value);
		 }
		 
		 [InitializeOnLoadMethod]
		 private static void OnEditorLoad() {
			 EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
		 }

		 private static void OnPlayModeStateChanged(PlayModeStateChange state) {
			 if (state == PlayModeStateChange.ExitingEditMode && AutoBoot) {
				 var activeScene = SceneManager.GetActiveScene();
				 if (activeScene.buildIndex != 0) {
					 _targetScene = activeScene.name;
					 Debug.Log($"Boot will load target scene after initialization: {_targetScene}");
				 } else {
					 _targetScene = null;
				 }
			 }
		 }
#endif

		 [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		 private static async void StartBoot() {
			 await UniTask.WaitForEndOfFrame();
				
			 var boot = Object.FindFirstObjectByType<Boot>();
			 if (boot != null) {
#if UNITY_EDITOR
				 if (AutoBoot && !string.IsNullOrEmpty(_targetScene)) {
					 boot.Run(_targetScene);
					 return;
				 }
#endif
				 boot.Run();
			 } else {
				 Debug.LogWarning("Boot component not found!");
			 }
		 }
	 }
}