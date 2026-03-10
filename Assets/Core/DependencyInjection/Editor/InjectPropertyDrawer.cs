using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Core.DependencyInjection.Editor {
	[CustomEditor(typeof(UnityEngine.Object), true)]
	[CanEditMultipleObjects]
	public class InjectPropertyDrawer: UnityEditor.Editor {
		private bool _foldout;

		public override void OnInspectorGUI() {
			DrawDefaultInspector();

			if (targets.Length > 1) {
				return;
			}

			var targetType = target.GetType();
			var fields = targetType.GetFields(
				BindingFlags.Instance |
				BindingFlags.Public |
				BindingFlags.NonPublic
			);

			var injectFields = new System.Collections.Generic.List<FieldInfo>();

			foreach (var field in fields) {
				var attribute = field.GetCustomAttribute<InjectAttribute>();

				if (attribute != null) {
					injectFields.Add(field);
				}
			}

			if (injectFields.Count == 0) {
				return;
			}

			EditorGUILayout.Space();

			_foldout = EditorGUILayout.Foldout(_foldout, "Injected Fields", true);

			if (!_foldout) {
				return;
			}

			EditorGUI.indentLevel++;

			foreach (var field in injectFields) {
				var attribute = field.GetCustomAttribute<InjectAttribute>();
				var fieldName = ObjectNames.NicifyVariableName(field.Name);
				var typeName = GetFullTypeName(field.FieldType);

				if (attribute.Id != null) {
					fieldName = $"{fieldName} [{attribute.Id}]";
				}

				if (Application.isPlaying) {
					var value = field.GetValue(target);
					var isNull = IsNullOrUnityNull(value);
					var emoji = isNull ? "❌" : "✅";
					fieldName = $"{emoji} {fieldName}";
				}

				EditorGUILayout.BeginHorizontal();

				var labelRect = GUILayoutUtility.GetRect(
					GUIContent.none,
					EditorStyles.label,
					GUILayout.ExpandWidth(true)
				);

				var nameRect = new Rect(
					labelRect.x,
					labelRect.y,
					labelRect.width * 0.4f,
					labelRect.height
				);

				var typeRect = new Rect(
					labelRect.x + labelRect.width * 0.4f,
					labelRect.y,
					labelRect.width * 0.6f,
					labelRect.height
				);

				EditorGUI.LabelField(nameRect, fieldName, EditorStyles.label);

				var prevEnabled = GUI.enabled;
				GUI.enabled = false;
				EditorGUI.TextField(typeRect, typeName);
				GUI.enabled = prevEnabled;

				EditorGUILayout.EndHorizontal();
			}

			EditorGUI.indentLevel--;
		}

		private string GetFullTypeName(Type type) {
			if (!type.IsGenericType) {
				return type.FullName ?? type.Name;
			}

			var genericTypeName = type.GetGenericTypeDefinition().FullName;
			var backtickIndex = genericTypeName.IndexOf('`');

			if (backtickIndex > 0) {
				genericTypeName = genericTypeName.Substring(0, backtickIndex);
			}

			var genericArgs = type.GetGenericArguments();
			var argNames = new string[genericArgs.Length];

			for (var i = 0; i < genericArgs.Length; i++) {
				argNames[i] = GetFullTypeName(genericArgs[i]);
			}

			return $"{genericTypeName}<{string.Join(", ", argNames)}>";
		}

		private bool IsNullOrUnityNull(object value) {
			if (value == null) {
				return true;
			}

			if (value is UnityEngine.Object unityObj) {
				return unityObj == null;
			}

			return false;
		}
	}
}