using Ringhold.CMS;
using Ringhold.Interactions;
using Ringhold.Picking;
using UnityEngine;

namespace Ringhold.Buildings {
	[SelectionBase]
	public class OreDrill: MonoBehaviour, IOreDrill, IPickupable, IInteraction {
		[field: SerializeField] public float Efficiency { get; private set; } = 1f;
		[SerializeField] private Vector3 _throwVelocity = new Vector3(2, 5f, 0);
		[Space]
		[SerializeField] private Transform _itemDropRoot;
		[SerializeField] private Collider _collider;
		
		private OreVein _vein;
		private float _timer;
		
		public OreVein Vein => _vein;

		private float MineInterval => _vein != null && _vein.Productivity > 0f
			? 1f / (_vein.Productivity * Efficiency)
			: float.PositiveInfinity;

		private void Update() {
			if (!_vein || _vein.Remaining <= 0f) {
				return;
			}

			_timer += Time.deltaTime;

			var interval = MineInterval;
			if (_timer < interval) {
				return;
			}

			var count = Mathf.FloorToInt(_timer / interval);
			_timer -= interval * count;

			_vein.Take(count);
			OnResourceMined(count);
		}

		public void SetVein(OreVein vein) {
			_vein = vein;
			_timer = 0;
		}
		
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
		public void OnPickup() {
			SetVein(null);
			_collider.enabled = false;
		}
		public void OnDrop() {
			SetVein(null);
			_collider.enabled = true;
		}
	}
}