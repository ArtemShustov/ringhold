using System;
using System.Linq;
using Ringhold.CMS;
using Ringhold.Items;
using UnityEngine;

namespace Ringhold.Buildings {
	public class Refinery : MonoBehaviour {
		[Header("Settings")]
		[SerializeField] private RefineryRecipe[] _recipes;
		[SerializeField] private float _duration = 1f;
		[Header("Components")]
		[SerializeField] private ItemSlot _input;
		[SerializeField] private ItemSlot _output;

		private float _timer;
		private RefineryRecipe _current;

		private void Update() {
			if (!_current.IsValid && !TakeItem(1)) {
				return;
			}

			_timer += Time.deltaTime;

			if (_timer < _duration) {
				return;
			}

			var count = Mathf.FloorToInt(_timer / _duration);
			_timer -= _duration * count;
			OnRefined(count);
		}

		private void OnRefined(int count) {
			if (_output.Current == null) {
				var item = ItemsPool.Instance.GetDroppedItem(_current.Output, count);
				item.OnPickup();
				_output.Put(item);
			} else {
				_output.Current.Add(count);
			}

			_current = default;
		}

		private bool TakeItem(int count) {
			var inputItem = _input.Current;

			if (!CanTake() || !TryGetRecipe(inputItem.Item, out var recipe)) {
				return false;
			}

			inputItem.Take(count);
			_current = recipe;

			if (inputItem.Count <= 0) {
				_input.Clear();
				Destroy(inputItem.gameObject);
			}

			return true;
			
			bool CanTake() {
				return inputItem != null
				       && inputItem.Count >= count
				       && TryGetRecipe(inputItem.Item, out var recipe)
				       && (_output.Current == null || _output.Current.Item == recipe.Output);
			}
		}
		private bool TryGetRecipe(Item input, out RefineryRecipe recipe) {
			recipe = _recipes.FirstOrDefault(r => r.Input == input);
			return recipe.Input == input;
		}

		[Serializable]
		public struct RefineryRecipe {
			public Item Input;
			public Item Output;
			
			public bool IsValid => Input != null && Output != null;
		}

		[Serializable]
		public class RefineryInputItemFilter: IItemFilter {
			[SerializeField] private Refinery _refinery;
			
			public bool Accept(Item item) => _refinery?.TryGetRecipe(item, out _) ?? false;
		}
	}
}