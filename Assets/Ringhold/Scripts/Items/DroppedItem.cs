using Ringhold.Interactions;
using Ringhold.Picking;
using UnityEngine;

namespace Ringhold.Items {
	[SelectionBase]
	public class DroppedItem: Pickupable {
		[field: SerializeField] public Item Item { get; private set; }
		[field: SerializeField] public int Count { get; private set; }
		[Space]
		[SerializeField] private Rigidbody _rigidbody;
		
		public void Set(Item item, int count) {
			Item = item;
			Count = count;
		}
		public void Add(int count) {
			Count += count;
		}
		public void Take(int count) {
			Count -= count;
		}
		public void ApplyVelocity(Vector3 velocity) {
			_rigidbody.AddForce(velocity, ForceMode.VelocityChange);
		}

		public override void Interact(InteractionContext context) {
			if (context.Hand.Current is DroppedItem item && item.Item == Item) {
				Add(item.Count);
				context.Hand.Clear();
				Destroy(item.gameObject);
				return;
			}
			base.Interact(context);
		}
		public override bool CanInteract(InteractionContext context) {
			var canStack = context.Hand.Current is DroppedItem item && item.Item == Item;
			return canStack || base.CanInteract(context);
		}

		private void OnTriggerEnter(Collider other) {
			if (!other.TryGetComponent<DroppedItem>(out var otherItem)) {
				return;
			}
			if (otherItem.Item != Item) {
				return;
			}

			var mySpeed = _rigidbody.linearVelocity.sqrMagnitude;
			var otherSpeed = otherItem._rigidbody.linearVelocity.sqrMagnitude;

			if (mySpeed >= otherSpeed) {
				otherItem.Add(Count);
				Destroy(gameObject);
			}
		}
	}
}