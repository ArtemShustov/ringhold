using UnityEngine;

namespace Core.Utils {
	public interface IWallet<T> {
		T Value { get; set; }
		
		event ValueChanged<T> ValueChanged;

		bool Has(T count); 
		bool Set(T count);
		bool Add(T count);
		bool Take(T count);
	}
	public abstract class Wallet<T>: IWallet<T> {
		protected T InternalValue;
		public T Value {
			get => InternalValue;
			set => Set(value);
		}
		
		public event ValueChanged<T> ValueChanged;

		protected Wallet() { }
		protected Wallet(T value) {
			InternalValue = value;
		}

		protected virtual void SetInternal(T value) {
			var old = InternalValue;
			InternalValue = value;
			ValueChanged?.Invoke(old, value);
		}

		public abstract bool Has(T count);
		public abstract bool Set(T count);
		public abstract bool Add(T count);
		public abstract bool Take(T count);
	}
}