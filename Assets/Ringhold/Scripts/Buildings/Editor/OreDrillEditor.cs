using Ringhold.Editor;
using UnityEditor;
using UnityEngine;

namespace Ringhold.Buildings.Editor {
	[CustomEditor(typeof(OreDrill))]
	public class OreDrillEditor : UnityEditor.Editor {
		private SerializedProperty _efficiency;
		private SerializedProperty _throwVelocity;
		
		private SerializedProperty _itemDropRoot;
		private SerializedProperty _collider;

		private void OnEnable() {
			_efficiency = serializedObject.FindProperty("<Efficiency>k__BackingField");
			_throwVelocity = serializedObject.FindProperty("_throwVelocity");
			
			_collider = serializedObject.FindProperty("_collider");
			_itemDropRoot = serializedObject.FindProperty("_itemDropRoot");
		}

		public override void OnInspectorGUI() {
			GUI.enabled = false;
			EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((MonoBehaviour)target), typeof(MonoScript), false);
			GUI.enabled = true;
			
			serializedObject.Update();

			CoolGUILayout.SectionHeader("Settings");
			EditorGUILayout.PropertyField(_efficiency, new GUIContent("Efficiency"));
			EditorGUILayout.PropertyField(_throwVelocity, new GUIContent("Item throw velocity"));
			
			CoolGUILayout.SectionHeader("Components");
			EditorGUILayout.PropertyField(_itemDropRoot, new GUIContent("Drop Root"));
			EditorGUILayout.PropertyField(_collider, new GUIContent("Collider"));

			serializedObject.ApplyModifiedProperties();

			var drill = (OreDrill)target;
			var vein = drill.Vein;

			if (vein == null) {
				EditorGUILayout.Space(4);
				CoolGUILayout.InfoBox("No vein attached. Place the drill on a vein.");
				return;
			}

			EditorGUILayout.Space(4);
			CoolGUILayout.SectionHeader("Active Vein");

			var accentYellow = new Color(0.95f, 0.75f, 0.2f, 0.9f);

			using (new EditorGUI.DisabledScope(true)) {
				EditorGUILayout.TextField(new GUIContent("Resource"), vein.Resource?.Id ?? "—");

				var productivity = vein.Productivity;
				var efficiency = _efficiency.floatValue;
				var interval = productivity > 0f && efficiency > 0f
					? 1f / (productivity * efficiency)
					: float.PositiveInfinity;

				var intervalLabel = float.IsInfinity(interval) ? "∞" : $"{interval:F2}s";
				EditorGUILayout.TextField(new GUIContent("Mine Interval"), intervalLabel);
			}

			if (!vein.Infinite) {
				var capacity = vein.Capacity;
				var remaining = vein.Remaining;
				var progress = capacity > 0f ? remaining / capacity : 0f;

				EditorGUILayout.Space(2);

				var progressRect = GUILayoutUtility.GetRect(0, EditorGUIUtility.singleLineHeight + 4, GUILayout.ExpandWidth(true));
				EditorGUI.DrawRect(progressRect, new Color(0f, 0f, 0f, 0.2f));

				var fillRect = new Rect(progressRect.x, progressRect.y, progressRect.width * progress, progressRect.height);
				EditorGUI.DrawRect(fillRect, new Color(accentYellow.r, accentYellow.g, accentYellow.b, 0.35f));
				EditorGUI.DrawRect(new Rect(progressRect.x, progressRect.y, 2, progressRect.height), accentYellow);

				var style = new GUIStyle(EditorStyles.miniLabel) {
					padding = new RectOffset(8, 4, 3, 0),
					alignment = TextAnchor.MiddleLeft,
				};
				style.normal.textColor = new Color(0.9f, 0.85f, 0.7f, 1f);

				GUI.Label(progressRect, $"Remaining  {remaining:F0} / {capacity:F0}  ({progress * 100f:F0}%)", style);
			} else {
				EditorGUILayout.Space(2);
				CoolGUILayout.InfoBox("This vein is infinite.", accentYellow);
			}

			if (Application.isPlaying) {
				Repaint();
			}
		}
	}
}