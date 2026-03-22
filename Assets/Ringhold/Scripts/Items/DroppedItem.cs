using Ringhold.Interactions;
using Ringhold.Picking;
using UnityEngine;

namespace Ringhold.Items {
	[SelectionBase]
	public class DroppedItem: Pickupable {
		[field: SerializeField] public ItemStack Stack { get; private set; }
		[Space]
		[SerializeField] private Rigidbody _rigidbody;
		
		public ItemDefinition Item => Stack?.Item;
		public int Count => Stack?.Count ?? 0;
		
		public void Set(ItemDefinition item, int count) {
			Stack = new ItemStack(item, count);
		}
		public void Add(int count) {
			Stack.Count += count;
		}
		public void Take(int count) {
			Stack.Count -= count;
		}
		public void ApplyVelocity(Vector3 velocity) {
			_rigidbody.AddForce(velocity, ForceMode.VelocityChange);
		}

		public override void Interact(InteractionContext context) {
			if (context.Hand.Current is DroppedItem item && item.Stack.Item == Stack.Item) {
				Add(item.Stack.Count);
				context.Hand.Clear();
				Destroy(item.gameObject);
				return;
			}
			base.Interact(context);
		}
		public override bool CanInteract(InteractionContext context) {
			var canStack = context.Hand.Current is DroppedItem item && item.Stack.Item == Stack.Item;
			return canStack || base.CanInteract(context);
		}

		private void OnTriggerEnter(Collider other) {
			Debug.Log($"[{name}] OnTriggerEnter with {other.name}"); 
			if (!other.TryGetComponent<DroppedItem>(out var otherItem)) {
				Debug.Log($"Not a dropped item");
				return;
			}
			if (otherItem.Stack.Item != Stack.Item) {
				Debug.Log($"Stacks is not equal");
				return;
			}

			var mySpeed = _rigidbody.linearVelocity.sqrMagnitude;
			var otherSpeed = otherItem._rigidbody.linearVelocity.sqrMagnitude;

			if (mySpeed >= otherSpeed) {
				otherItem.Add(Stack.Count);
				Destroy(gameObject);
			}
		}
	}
}