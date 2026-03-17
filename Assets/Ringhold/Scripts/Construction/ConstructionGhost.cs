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
			
			OnItemAdded(new FastItemStack(droppedItem.Item, acceptedCount));
		}
		public bool CanInteract(InteractionContext context) {
			return context.Hand.Current is DroppedItem dItem
			       && CanAccept(dItem.Item);
		}
		public void SetInteractionState(InteractionHighlightState state) { }

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
				if (stack == null || stack.Count < required.Count) {
					return false;
				}
			}
			return true;
		}
		private void OnItemAdded(FastItemStack item) {
			Debug.Log($"[{name}] Added {item.Count} of {item.Item.name}");

			if (IsFinished()) {
				var building = GameResources.EntityRegistry.Pool.Get(Scheme.Building);
				building.transform.position = transform.position;
				building.transform.rotation = transform.rotation;
				
				Destroy(gameObject);
			}
		}
	}
}