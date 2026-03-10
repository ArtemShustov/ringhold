using Ringhold.Interactions;
using Ringhold.Items;
using Ringhold.Picking;
using UnityEngine;

namespace Ringhold.Buildings {
	[SelectionBase]
	public class OreVein: MonoBehaviour, IInteraction {
		[field: Header("Resource")]
		[field: SerializeField] public Item Resource { get; private set; }
		[field: SerializeField] public int MineLevel { get; private set; }
		
		[Header("Storage")]
		[SerializeField] private int _remaining;
		[field: SerializeField] public int Capacity { get; private set; }
		[field: SerializeField] public bool Infinite { get; private set; }

		private IOreDrill _drill;
		
		public int Remaining => Infinite ? int.MaxValue : _remaining;

		public void Take(int amount) {
			_remaining = Mathf.Max(0, _remaining - amount);
		}
		
		public void Interact(InteractionContext context) {
			var canTake = _drill != null && context.Hand.Current == null && _drill is IPickupable;
			var canPlace = _drill == null && context.Hand.Current is IOreDrill;

			if (canTake) {
				context.Hand.Pick(_drill as IPickupable);
				_drill.SetVein(null);
				_drill = null;
				return;
			} 
			if (canPlace) {
				_drill = context.Hand.Current as IOreDrill;
				_drill?.SetVein(this);
				context.Hand.Clear();

				if (_drill is MonoBehaviour monoDrill) {
					var drillTransform = monoDrill.transform;
					drillTransform.SetParent(transform);
					drillTransform.localPosition = Vector3.zero;
					drillTransform.localRotation = Quaternion.identity;
				}
				return;
			}
		}
		public bool CanInteract(InteractionContext context) {
			var canTake = _drill != null && context.Hand.Current == null && _drill is IPickupable;
			var canPlace = _drill == null && context.Hand.Current is IOreDrill;
			return canTake || canPlace;
		}
		public void SetInteractionState(InteractionHighlightState state) { }
	}
}