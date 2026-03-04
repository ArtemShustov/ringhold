using UnityEditor;
using UnityEngine;

namespace Ringhold.Editor {
	public static class CoolGUILayout {
		public static readonly Color DefaultAccent = new Color(0.2f, 0.8f, 0.1f, 0.9f);

		public static void SectionHeader(string title) => SectionHeader(title, DefaultAccent);
		public static void SectionHeader(string title, Color accent) {
			EditorGUILayout.Space(2);
			var rect = GUILayoutUtility.GetRect(0, 20, GUILayout.ExpandWidth(true));

			EditorGUI.DrawRect(rect, new Color(0f, 0f, 0f, 0.15f));
			EditorGUI.DrawRect(new Rect(rect.x, rect.y, 2, rect.height), accent);

			var style = new GUIStyle(EditorStyles.boldLabel) {
				fontSize = 10,
				padding = new RectOffset(8, 0, 0, 0),
			};
			style.normal.textColor = new Color(0.85f, 0.82f, 0.78f, 1f);

			GUI.Label(rect, title.ToUpper(), style);
			EditorGUILayout.Space(2);
		}

		public static void InfoBox(string message) => InfoBox(message, DefaultAccent);
		public static void InfoBox(string message, Color accent) {
			var rect = GUILayoutUtility.GetRect(0, EditorGUIUtility.singleLineHeight + 8, GUILayout.ExpandWidth(true));

			EditorGUI.DrawRect(rect, new Color(accent.r, accent.g, accent.b, 0.08f));
			EditorGUI.DrawRect(new Rect(rect.x, rect.y, 2, rect.height), new Color(accent.r, accent.g, accent.b, 0.7f));

			var style = new GUIStyle(EditorStyles.miniLabel) {
				padding = new RectOffset(8, 4, 4, 4),
				wordWrap = true,
			};
			style.normal.textColor = new Color(accent.r, accent.g, accent.b, 0.9f);

			GUI.Label(rect, message, style);
		}

		public static bool AccentToggle(string label, bool value) => AccentToggle(label, value, DefaultAccent);
		public static bool AccentToggle(string label, bool value, Color accent) {
			var rect = GUILayoutUtility.GetRect(0, EditorGUIUtility.singleLineHeight + 4, GUILayout.ExpandWidth(true));

			if (value) {
				EditorGUI.DrawRect(rect, new Color(accent.r, accent.g, accent.b, 0.08f));
			}

			var toggleRect = new Rect(rect.x, rect.y + 2, rect.width, EditorGUIUtility.singleLineHeight);
			return EditorGUI.Toggle(toggleRect, label, value);
		}
	}
}