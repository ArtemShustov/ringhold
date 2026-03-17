using System;
using UnityEngine;

namespace Ringhold.Items {
	[Serializable]
	public class ItemStack: IEquatable<ItemStack>, IEquatable<FastItemStack> {
		[field: SerializeField] public ItemDefinition Item { get; set; }
		[field: SerializeField] public int Count { get; set; }

		public ItemStack() { }
		public ItemStack(ItemDefinition item, int count = 1) {
			Item = item;
			Count = count;
		}
		
		public bool Equals(ItemStack other) {
			if (other is null) {
				return false;
			}
			return Item == other.Item && Count == other.Count;
		}
		public bool Equals(FastItemStack other) {
			return Item == other.Item && Count == other.Count;
		}
		public override bool Equals(object obj) {
			if (obj is ItemStack itemStack) {
				return Equals(itemStack);
			}
			if (obj is FastItemStack fastItemStack) {
				return Equals(fastItemStack);
			}
			return false;
		}
		public override int GetHashCode() {
			return HashCode.Combine(Item, Count);
		}

		public static bool operator ==(ItemStack left, ItemStack right) {
			if (left is null) {
				return right is null;
			}
			return left.Equals(right);
		}
		public static bool operator !=(ItemStack left, ItemStack right) {
			return !(left == right);
		}
		
		public static bool operator ==(ItemStack left, FastItemStack right) {
			if (left is null) {
				return false;
			}
			return left.Equals(right);
		}
		public static bool operator !=(ItemStack left, FastItemStack right) {
			return !(left == right);
		}
		
		public static bool operator ==(FastItemStack left, ItemStack right) {
			return right == left;
		}
		public static bool operator !=(FastItemStack left, ItemStack right) {
			return !(left == right);
		}
	}

	[Serializable]
	public struct FastItemStack: IEquatable<FastItemStack>, IEquatable<ItemStack> {
		public ItemDefinition Item;
		public int Count;

		public FastItemStack(ItemDefinition item, int count = 1) {
			Item = item;
			Count = count;
		}
		
		public bool IsValid() => Item != null;

		public bool Equals(FastItemStack other) {
			return Item == other.Item && Count == other.Count;
		}
		public bool Equals(ItemStack other) {
			if (other is null) {
				return false;
			}
			return Item == other.Item && Count == other.Count;
		}
		public override bool Equals(object obj) {
			if (obj is FastItemStack fastItemStack) {
				return Equals(fastItemStack);
			}
			if (obj is ItemStack itemStack) {
				return Equals(itemStack);
			}
			return false;
		}
		public override int GetHashCode() {
			return HashCode.Combine(Item, Count);
		}
		public static bool operator ==(FastItemStack left, FastItemStack right) {
			return left.Equals(right);
		}
		public static bool operator !=(FastItemStack left, FastItemStack right) {
			return !(left == right);
		}
	}
}