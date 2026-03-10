using System;
using Ringhold.Interactions;
using UnityEngine;

namespace Ringhold.Items {
	public class ItemSlot: MonoBehaviour, IInteraction {
		[SerializeField] private bool _canPut = true;
		[SerializeField] private bool _canTake = true;
		[SerializeReference, SubclassSelector] private IItemFilter _filter;
		
		private DroppedItem _current;
		public DroppedItem Current => _current;

		public event Action<DroppedItem> ItemPlaced;
		public event Action<DroppedItem> ItemRemoved;

		public void Put(DroppedItem item) {
			_current = item;
			
			if (_current != null) {
				_current.transform.SetParent(transform);
				_current.transform.localPosition = Vector3.zero;
				_current.transform.localRotation = Quaternion.identity;
			}
			
			ItemPlaced?.Invoke(item);
		}
		public DroppedItem Clear() {
			var item = _current;
			_current = null;
			ItemRemoved?.Invoke(item);
			return item;
		}
		
		public void Interact(InteractionContext context) {
			if (CanTake(context)) {
				context.Hand.Pick(_current);
				Clear();
				return;
			}
			if (CanPut(context)) {
				var item = context.Hand.Clear();
				Put(item as DroppedItem);
				return;
			}
		}
		public bool CanInteract(InteractionContext context) {
			return CanPut(context) || CanTake(context);
		}
		public void SetInteractionState(InteractionHighlightState state) { }
		
		public bool CanPut(InteractionContext context) {
			return _canPut && _current == null
			       && context.Hand.Current is DroppedItem item
			       && (_filter?.Accept(item.Item) ?? true);
		}
		public bool CanTake(InteractionContext context) {
			return _canTake && _current != null 
			       && context.Hand.Current == null;
		}
	}

	public interface IItemFilter {
		bool Accept(Item item);
	}
}