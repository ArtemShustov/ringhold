using UnityEditor;
using UnityEngine;

namespace Ringhold.Items.Editor {
	[CustomPropertyDrawer(typeof(ItemStack))]
	[CustomPropertyDrawer(typeof(FastItemStack))]
	public class ItemStackDrawer: PropertyDrawer {
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			var count = property.FindPropertyRelative("Count") ?? property.FindPropertyRelative("_count");
			var item = property.FindPropertyRelative("Item") ?? property.FindPropertyRelative("<Item>k__BackingField");
			
			label = EditorGUI.BeginProperty(position, label, property);
			position = EditorGUI.PrefixLabel(position, label);

			var countRect = new Rect(position.x, position.y, 50, position.height);
			var ofRect = new Rect(position.x + 55, position.y, 20, position.height);
			var itemRect = new Rect(position.x + 80, position.y, position.width - 80, position.height);

			EditorGUI.PropertyField(countRect, count, GUIContent.none);
			EditorGUI.LabelField(ofRect, "of");
			EditorGUI.PropertyField(itemRect, item, GUIContent.none);

			EditorGUI.EndProperty();
		}
	}
}