using Core.DependencyInjection;
using Ringhold.CMS;
using Ringhold.Interactions;
using Ringhold.Picking;
using Ringhold.World;
using UnityEngine;

namespace Ringhold.Buildings {
	[SelectionBase]
	public class OreDrill: MonoBehaviour, IOreDrill, IPickupable, IInteraction, ITickable {
		[Header("Settings")]
		[SerializeField, Min(0)] private int _interval = 20;
		[SerializeField] private Vector3 _throwVelocity = new Vector3(2, 5f, 0);
		
		[Header("Components")]
		[SerializeField] private Transform _itemDropRoot;
		[SerializeField] private Collider _collider;
		[Inject] private ITickGroup _tickGroup;
		
		private OreVein _vein;
		private int _timer;
		
		public OreVein Vein => _vein;

		public void OnTick() {
			if (!_vein || _vein.Remaining <= 0) {
				return;
			}
			_timer += 1;

			if (_timer < _interval) {
				return;
			}
			_timer = 0;

			_vein.Take(1);
			OnResourceMined(1);
		}

		public void SetVein(OreVein vein) {
			_vein = vein;
			_timer = 0;
		}
		private void OnResourceMined(int count) {
			Debug.Log($"[{name}] Mined {count} of {_vein?.Resource?.Id ?? "null"}");
			
			if (_vein?.Resource == null) {
				return;
			}
			var resource = _vein.Resource;
			var view = ItemsPool.Instance.GetDroppedItem(resource, count);
			view.transform.position = _itemDropRoot.position;
			view.ApplyVelocity(_itemDropRoot.TransformDirection(_throwVelocity));
		}

		private void OnEnable() {
			_tickGroup.Subscribe(this);
		}
		private void OnDisable() {
			_tickGroup.Unsubscribe(this);
		}

		#region Interaction & Pickupable
		public void Interact(InteractionContext context) {
			if (!CanInteract(context)) {
				return;
			}
			context.Hand.Pick(this);
		}
		public bool CanInteract(InteractionContext context) {
			return context.Hand.Current == null;
		}
		public void SetInteractionState(InteractionHighlightState state) { }
		
		public void OnPickup() {
			SetVein(null);
			_collider.enabled = false;
		}
		public void OnDrop() {
			SetVein(null);
			_collider.enabled = true;
		}
		#endregion
	}
}