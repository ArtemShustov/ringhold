using System;
using System.Linq;
using Core.DependencyInjection;
using Ringhold.CMS;
using Ringhold.Items;
using Ringhold.World;
using UnityEngine;

namespace Ringhold.Buildings {
	public class Refinery : MonoBehaviour, ITickable {
		[Header("Settings")]
		[SerializeField] private RefineryRecipe[] _recipes;
		[SerializeField, Min(0)] private int _interval = 20;

		[Header("Components")]
		[SerializeField] private ItemSlot _input;
		[SerializeField] private ItemSlot _output;
		[Inject] private ITickGroup _tickGroup;

		private int _timer;
		private RefineryRecipe _current;

		public void OnTick() {
			if (!_current.IsValid && !TakeItem(1)) {
				return;
			}

			_timer += 1;

			if (_timer < _interval) {
				return;
			}

			_timer = 0;
			OnRefined(1);
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

		private bool TryGetRecipe(ItemDefinition input, out RefineryRecipe recipe) {
			recipe = _recipes.FirstOrDefault(r => r.Input == input);
			return recipe.Input == input;
		}

		private void OnEnable() {
			_tickGroup.Subscribe(this);
		}
		private void OnDisable() {
			_tickGroup.Unsubscribe(this);
		}

		[Serializable]
		public struct RefineryRecipe {
			public ItemDefinition Input;
			public ItemDefinition Output;

			public bool IsValid => Input != null && Output != null;
		}

		[Serializable]
		public class RefineryInputItemFilter : IItemFilter {
			[SerializeField] private Refinery _refinery;

			public bool Accept(ItemDefinition item) => _refinery?.TryGetRecipe(item, out _) ?? false;
		}
	}
}