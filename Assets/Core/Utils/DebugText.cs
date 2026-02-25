using UnityEditor;
using UnityEngine;

namespace Core.Utils {
	public static class DebugText {
		public static float BaseHeight = 1080;
		public static int FontSize = 28;
		public readonly static GUIStyle Style;

		static DebugText() {
			Style = new GUIStyle();
			Style.alignment = TextAnchor.LowerCenter;
			Style.normal.textColor = Color.red;
			Style.fontStyle = FontStyle.Bold;
			Style.fontSize = FontSize;
		}

		public static void Draw(string text, Vector3 position) => Draw(text, position, Color.red);
		public static void Draw(string text, Vector3 position, Color color) {
			if (!IsEnabled) {
				return;
			}
			
			Style.fontSize = Style.fontSize = Mathf.RoundToInt(FontSize * (Screen.height / BaseHeight));
			Style.normal.textColor = color;
			
			var sPos = Camera.main.WorldToScreenPoint(position);
			if (sPos.z < 0) {
				return;
			}
			var rect = new Rect(sPos.x, Screen.height - sPos.y, 0, 0);
			
			GUI.Box(rect, text, Style);
		}
		
		
		#if UNITY_EDITOR
		private const string TOGGLE_KEY = "Editor.DebugText.Enabled";
		private const string MENU_PATH = "Tools/Show debug text";
		
		[MenuItem(MENU_PATH)]
		private static void Toggle() {
			IsEnabled = !IsEnabled;
			Menu.SetChecked(MENU_PATH, IsEnabled);
		}
		#endif

		public static bool IsEnabled {
			#if UNITY_EDITOR
			get => EditorPrefs.GetBool(TOGGLE_KEY, true);
			set => EditorPrefs.SetBool(TOGGLE_KEY, value);
			#elif DEBUG
			get => true;
			set { }
			#else
			get => false;
			set { }
			#endif
		}
	}
}