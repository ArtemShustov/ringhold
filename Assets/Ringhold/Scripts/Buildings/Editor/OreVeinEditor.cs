using Ringhold.Editor;
using UnityEditor;
using UnityEngine;

namespace Ringhold.Buildings.Editor {
	[CustomEditor(typeof(OreVein))]
	public class OreVeinEditor: UnityEditor.Editor {
		private SerializedProperty _resource;
		private SerializedProperty _productivity;
		private SerializedProperty _mineLevel;
		private SerializedProperty _remaining;
		private SerializedProperty _capacity;
		private SerializedProperty _infinite;

		private void OnEnable() {
			_resource = serializedObject.FindProperty("<Resource>k__BackingField");
			_productivity = serializedObject.FindProperty("<Productivity>k__BackingField");
			_mineLevel = serializedObject.FindProperty("<MineLevel>k__BackingField");
			_remaining = serializedObject.FindProperty("_remaining");
			_capacity = serializedObject.FindProperty("<Capacity>k__BackingField");
			_infinite = serializedObject.FindProperty("<Infinite>k__BackingField");
		}

		public override void OnInspectorGUI() {
			serializedObject.Update();

			CoolGUILayout.SectionHeader("Resource");
			EditorGUILayout.PropertyField(_resource);
			EditorGUILayout.PropertyField(_productivity);
			EditorGUILayout.PropertyField(_mineLevel, new GUIContent("Mine Level"));

			EditorGUILayout.Space(4);
			CoolGUILayout.SectionHeader("Storage");

			_infinite.boolValue = CoolGUILayout.AccentToggle("Infinite", _infinite.boolValue);

			if (!_infinite.boolValue) {
				EditorGUILayout.PropertyField(_capacity, new GUIContent("Capacity"));

				var cap = _capacity.floatValue;

				EditorGUI.BeginChangeCheck();
				var newRem = EditorGUILayout.Slider(new GUIContent("Remaining"), _remaining.floatValue, 0f, Mathf.Max(cap, 0.001f));
				if (EditorGUI.EndChangeCheck()) {
					_remaining.floatValue = newRem;
				}
			} else {
				CoolGUILayout.InfoBox("This vein will never be depleted.");
			}

			serializedObject.ApplyModifiedProperties();
		}
	}
}