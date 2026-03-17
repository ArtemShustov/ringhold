using Ringhold.Interactions;
using Ringhold.Items;
using Ringhold.Picking;
using UnityEngine;

namespace Ringhold.Buildings {
	[SelectionBase]
	public class OreVein: MonoBehaviour, IInteraction {
		[field: Header("Resource")]
		[field: SerializeField] public ItemDefinition Resource { get; private set; }
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
			if (CanTakeDrill(context, out var pickupableDrill)) {
				context.Hand.Pick(pickupableDrill);
				_drill.SetVein(null);
				_drill = null;
				return;
			} 
			
			if (CanPlaceDrill(context, out var drill)) {
				_drill = drill;
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
			return CanTakeDrill(context, out _) || CanPlaceDrill(context, out _);
		}
		public void SetInteractionState(InteractionHighlightState state) { }

		private bool CanTakeDrill(InteractionContext context, out IPickupable pickupableDrill) {
			if (_drill is not MonoBehaviour monoDrill || context.Hand.Current != null) {
				pickupableDrill = null;
				return false;
			}
			return monoDrill.TryGetComponent<IPickupable>(out pickupableDrill);
		}
		private bool CanPlaceDrill(InteractionContext context, out IOreDrill drill) {
			drill = null;
			return _drill == null && context.Hand.CurrentIs<IOreDrill>(out drill);
		}
	}
}