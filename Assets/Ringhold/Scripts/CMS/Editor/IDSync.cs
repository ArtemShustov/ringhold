using UnityEditor;
using UnityEngine;

namespace Ringhold.CMS {
	public class IDSync: AssetPostprocessor {
		private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths) {
			foreach (string path in importedAssets) {
				var entry = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);
				if (entry == null || entry is not ICMSEntry) {
					continue;
				}

				var serialized = new SerializedObject(entry);
				var id = serialized.FindProperty("<Id>k__BackingField");
				var fileName = entry.name;

				if (id != null && id.stringValue != fileName) {
					id.stringValue = fileName;
					serialized.ApplyModifiedPropertiesWithoutUndo();
				}
			}
		}
	}
}