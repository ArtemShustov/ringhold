using System.Collections.Generic;
using System.Linq;
using Ringhold.Items;
using Ringhold.Utils;
using UnityEngine;

namespace Ringhold.Construction.UI {
	public class ConstructionGhostUI: MonoBehaviour {
		[Header("Settings")]
		[SerializeField] private RequiredItem _itemPrefab;
		
		[Header("Components")]
		[SerializeField] private ConstructionGhost _building;
		[SerializeField] private Transform _itemsContainer;
		[SerializeField] private Bar _progressBar;

		private readonly Dictionary<ItemDefinition, RequiredItem> _items = new();

		private void OnEnable() {
			_building.SchemeChanged += OnSchemeChanged;
			_building.ProgressChanged += OnProgressChanged;
			_building.StackChanged += OnStackChanged;
		}
		private void OnDisable() {
			_building.SchemeChanged -= OnSchemeChanged;
			_building.ProgressChanged -= OnProgressChanged;
			_building.StackChanged -= OnStackChanged;
		}
		
		private void OnSchemeChanged(ConstructionSchemeDefinition scheme) {
			foreach (var (_, view) in _items) {
				Destroy(view.gameObject);
			}
			_items.Clear();
			
			foreach (var requiredStack in scheme.RequiredItems) {
				var view = Instantiate(_itemPrefab, _itemsContainer);
				view.gameObject.SetActive(true);
				_items.Add(requiredStack.Item, view);
				
				var existingCount = _building.Stored.FirstOrDefault()?.Count ?? 0;
				view.SetCurrent(existingCount);
				view.SetItem(requiredStack.Item, requiredStack.Count);
			}
			
			_progressBar.SetFill(_building.GetProgress());
		}
		private void OnProgressChanged(float value) {
			_progressBar.SetFill(value);
		}
		private void OnStackChanged(ItemStack stack) {
			if (!_items.TryGetValue(stack.Item, out var view)) {
				return;
			}
			view.SetCurrent(stack.Count);
		}
	}
}