using UnityEngine;

namespace Ringhold.Picking {
	public class Hand: MonoBehaviour {
		[SerializeField] private float _groundRayDist = 3;
		[SerializeField] private LayerMask _groundRayMask = ~0;
		[SerializeField] private Transform _root;
		public IPickupable Current { get; private set; }

		public bool CurrentIs<T>(out T current) {
			current = Current is T c ? c : default;
			return Current switch {
				null => false,
				T => true,
				Component component => component.gameObject.TryGetComponent<T>(out current),
				_ => false
			};
		}
		
		public void Pick(IPickupable item) {
			if (Current == item) {
				Drop();
			}
			Current = item;
			item.OnPickup();
			PickVisual(item);
		}
		private void PickVisual(IPickupable item) {
			if (item is not MonoBehaviour mono) {
				return;
			}
			var itemTransform = mono.transform;
			itemTransform.SetParent(_root);
			itemTransform.localPosition = Vector3.zero;
			itemTransform.localRotation = Quaternion.identity;
		}
		
		public IPickupable Drop() {
			if (Current == null) {
				return null;
			}
			var item = Current;
			Current = null;
			item.OnDrop();
			DropVisual(item);
			return item;
		}
		private void DropVisual(IPickupable item) {
			if (item is not MonoBehaviour mono) {
				return;
			}
			var itemTransform = mono.transform;
			itemTransform.SetParent(null);
			itemTransform.transform.position = GetGroundPosition();
		}

		public IPickupable Clear() {
			if (Current == null) {
				return null;
			}
			var item = Current;
			Current = null;
			return item;
		}

		private Vector3 GetGroundPosition() {
			var result = Physics.Raycast(_root.position, -_root.up, out var hit, _groundRayDist, _groundRayMask);
			return result ? hit.point : _root.position;
		}
	}
}