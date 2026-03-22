using TMPro;
using UnityEngine;

namespace Ringhold.Items {
	public class DroppedItemView: MonoBehaviour {
		[SerializeField] private TMP_Text _countLabel;
		[SerializeField] private Transform _frameRoot;
		[Space]
		[SerializeField] private DroppedItem _item;

		private void RefreshLabel() {
			var count = _item.Stack?.Count ?? 0;
			_frameRoot.gameObject.SetActive(count > 1);
			_countLabel.text = count.ToString();
		}
		
		private void OnCountChanged(int oldValue, int newValue) => RefreshLabel();
		
		private void OnEnable() {
			_item.Stack.CountChanged += OnCountChanged;
			
			RefreshLabel();
		}
		private void OnDisable() {
			_item.Stack.CountChanged -= OnCountChanged;
		}
	}
}