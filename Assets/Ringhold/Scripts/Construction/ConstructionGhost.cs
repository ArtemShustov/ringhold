using System;
using System.Collections.Generic;
using System.Linq;
using Ringhold.CMS;
using Ringhold.Interactions;
using Ringhold.Items;
using UnityEngine;

namespace Ringhold.Construction {
	public class ConstructionGhost: MonoBehaviour, IInteraction {
		[field: SerializeField] public ConstructionSchemeDefinition Scheme { get; private set; }
		private readonly List<ItemStack> _items = new List<ItemStack>();

		public IReadOnlyList<FastItemStack> Required => Scheme.RequiredItems;
		public IReadOnlyList<ItemStack> Stored => _items;
		
		public event Action<float> ProgressChanged;
		public event Action<ItemStack> StackChanged; 
		public event Action<ConstructionSchemeDefinition> SchemeChanged; 
		public event Action<InteractionHighlightState> InteractionStateChanged;
		
		public void Interact(InteractionContext context) {
			var droppedItem = context.Hand.Current as DroppedItem;
			if (droppedItem == null || !CanAccept(droppedItem.Item)) {
				return;
			}
			
			var required = Scheme.RequiredItems.FirstOrDefault(s => s.Item == droppedItem.Item);
			var stack = _items.FirstOrDefault(s => s.Item == droppedItem.Item);
			var currentCount = stack?.Count ?? 0;
			var acceptedCount = Mathf.Min(required.Count - currentCount, droppedItem.Count);
			
			if (stack == null) {
				stack = new ItemStack(droppedItem.Item, acceptedCount);
				_items.Add(stack);
			} else {
				stack.Count += acceptedCount;
			}

			if (droppedItem.Count <= acceptedCount) {
				context.Hand.Clear();
				Destroy(droppedItem.gameObject);
			} else {
				droppedItem.Take(acceptedCount);
			}
			
			OnItemAdded(stack, acceptedCount);
		}
		public bool CanInteract(InteractionContext context) {
			return context.Hand.Current is DroppedItem dItem
			       && CanAccept(dItem.Item);
		}
		public void SetInteractionState(InteractionHighlightState state) {
			InteractionStateChanged?.Invoke(state);
		}

		private void Start() {
			SetScheme(Scheme);
		}

		public void Complete() {
			var building = GameResources.EntityRegistry.Pool.Get(Scheme.Building);
			building.transform.position = transform.position;
			building.transform.rotation = transform.rotation;
				
			Destroy(gameObject);
		}
		public void SetScheme(ConstructionSchemeDefinition scheme) {
			Scheme = scheme;
			
			SchemeChanged?.Invoke(Scheme);
		}
		public float GetProgress() {
			var totalRequired = Scheme.RequiredItems.Sum(s => s.Count);
			if (totalRequired <= 0) {
				return 1f;
			}

			var current = 0;
			foreach (var required in Scheme.RequiredItems) {
				var stack = _items.FirstOrDefault(s => s.Item == required.Item);
				var currentCount = stack?.Count ?? 0;
				
				current += Mathf.Min(currentCount, required.Count);
			}

			return (float)current / totalRequired;
		}
		
		private bool CanAccept(ItemDefinition item) {
			var required = Scheme.RequiredItems.FirstOrDefault(s => s.Item == item);
			if (!required.IsValid()) {
				return false;
			}
			var count = _items.FirstOrDefault(s => s.Item == required.Item)?.Count ?? 0;
			return count < required.Count;
		}
		private bool IsFinished() {
			foreach (var required in Scheme.RequiredItems) {
				var stack = _items.FirstOrDefault(s => s.Item == required.Item);
				var count = stack?.Count ?? 0;

				if (count < required.Count) {
					return false;
				}
			}
			return true;
		}
		
		private void OnItemAdded(ItemStack stack, int addedCount) {
			Debug.Log($"[{name}] Added {addedCount} of {stack.Item.name}");
			
			StackChanged?.Invoke(stack);

			var progress = GetProgress();
			ProgressChanged?.Invoke(progress);
			
			if (progress >= 1f) {
				Complete();
			}
		}
	}
}