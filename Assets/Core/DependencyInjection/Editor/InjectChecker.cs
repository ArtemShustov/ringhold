using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.Collections.Generic;

namespace Core.DependencyInjection.Editor {
    public class InjectChecker : EditorWindow {
        private readonly List<MonoBehaviourData> _results = new List<MonoBehaviourData>();
        private Vector2 _scrollPos;
        private int _totalInjectFields;
        private int _notNullCount;
        private int _nullCount;

        private class MonoBehaviourData {
            public MonoBehaviour Target;
            public bool IsExpanded;
            public bool HasNulls;
            public List<FieldData> Fields = new List<FieldData>();
        }

        private struct FieldData {
            public string Name;
            public string TypeName;
            public bool IsNull;
        }

        [MenuItem("Tools/Inject checker")]
        public static void ShowWindow() {
            var window = GetWindow<InjectChecker>("Inject Checker");
            window.minSize = new Vector2(350, 400);
        }

        private void OnEnable() => PerformCheck();

        private void OnGUI() {
            DrawHeader();
            DrawStatistics();

            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
            DrawResults();
            EditorGUILayout.EndScrollView();
        }

        private void DrawHeader() {
            EditorGUILayout.Space(5);
            using (new EditorGUILayout.HorizontalScope()) {
                if (GUILayout.Button("Refresh Scene", GUILayout.Height(25))) {
                    PerformCheck();
                }
                if (GUILayout.Button("Clear", GUILayout.Width(60), GUILayout.Height(25))) {
                    _results.Clear();
                    _totalInjectFields = _notNullCount = _nullCount = 0;
                }
            }
            EditorGUILayout.Space(5);
        }

        private void DrawStatistics() {
            if (_totalInjectFields == 0) return;

            var msgType = _nullCount > 0 ? MessageType.Warning : MessageType.Info;
            var stats = $"Fields: {_totalInjectFields} | Assigned: {_notNullCount} | Null: {_nullCount}";
            EditorGUILayout.HelpBox(stats, msgType);
            EditorGUILayout.Space(5);
        }

        private void DrawResults() {
            if (_results.Count == 0) {
                if (_totalInjectFields > 0) {
                    EditorGUILayout.LabelField("🎉 All fields are valid!", EditorStyles.centeredGreyMiniLabel);
                } else {
                    EditorGUILayout.LabelField("No [Inject] attributes found on scene.", EditorStyles.centeredGreyMiniLabel);
                }
                return;
            }

            for (int i = 0; i < _results.Count; i++) {
                var data = _results[i];

                if (data.Target == null) {
                    _results.RemoveAt(i--);
                    continue;
                }

                DrawObjectBlock(data);
            }
        }

        private void DrawObjectBlock(MonoBehaviourData data) {
            var prevColor = GUI.backgroundColor;
            if (data.HasNulls) GUI.backgroundColor = new Color(1f, 0.45f, 0.45f);

            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox)) {
                GUI.backgroundColor = prevColor;

                using (new EditorGUILayout.HorizontalScope()) {
                    var icon = data.HasNulls ? "❌" : "✅";
                    var label = $"{icon} {data.Target.gameObject.name} ({data.Target.GetType().Name})";
                    
                    data.IsExpanded = EditorGUILayout.Foldout(data.IsExpanded, label, true, EditorStyles.foldoutHeader);

                    if (GUILayout.Button("Select", EditorStyles.miniButton, GUILayout.Width(50))) {
                        Selection.activeObject = data.Target.gameObject;
                        EditorGUIUtility.PingObject(data.Target.gameObject);
                    }
                }

                if (data.IsExpanded) {
                    EditorGUILayout.Space(2);
                    EditorGUI.indentLevel++;
                    foreach (var field in data.Fields) {
                        DrawFieldLabel(field);
                    }
                    EditorGUI.indentLevel--;
                    EditorGUILayout.Space(2);
                }
            }
        }

        private void DrawFieldLabel(FieldData field) {
            var style = new GUIStyle(EditorStyles.label);
            var content = new GUIContent($"{field.TypeName} {field.Name}");

            if (field.IsNull) {
                style.normal.textColor = new Color(0.9f, 0.2f, 0.2f);
                content.text += " (Null)";
            } else {
                style.normal.textColor = Color.gray;
            }

            EditorGUILayout.LabelField(content, style);
        }

        private void PerformCheck() {
            _results.Clear();
            _totalInjectFields = _notNullCount = _nullCount = 0;

            var allScripts = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);
            var bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

            foreach (var script in allScripts) {
                if (script == null) continue;

                var fields = script.GetType().GetFields(bindingFlags);
                var scriptData = new MonoBehaviourData { Target = script };
                bool hasInjectAttribute = false;

                foreach (var field in fields) {
                    if (!Attribute.IsDefined(field, typeof(InjectAttribute))) continue;

                    hasInjectAttribute = true;
                    _totalInjectFields++;

                    var value = field.GetValue(script);
                    var isNull = value == null || value.Equals(null);

                    scriptData.Fields.Add(new FieldData {
                        Name = field.Name,
                        TypeName = field.FieldType.Name,
                        IsNull = isNull
                    });

                    if (isNull) {
                        scriptData.HasNulls = true;
                        _nullCount++;
                    } else {
                        _notNullCount++;
                    }
                }

                if (hasInjectAttribute) {
                    scriptData.IsExpanded = scriptData.HasNulls;
                    _results.Add(scriptData);
                }
            }
            
            // Сортировка: объекты с ошибками в начало списка
            _results.Sort((a, b) => b.HasNulls.CompareTo(a.HasNulls));
        }
    }
}
