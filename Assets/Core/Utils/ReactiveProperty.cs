using System;
using UnityEngine;

namespace Core.Utils {
	public interface IReadOnlyReactiveProperty<T> {
		T Value { get; }
		
		event ValueChanged<T> ValueChanged;
	}
	[Serializable]
	public class ReactiveProperty<T>: IReadOnlyReactiveProperty<T> {
		[SerializeField] private T _value;
		public virtual T Value {
			get => _value;
			set => Set(value);
		}

		public event ValueChanged<T> ValueChanged;

		public ReactiveProperty(T value = default) {
			_value = value;
		}

		private void Set(T value) {
			var oldValue = _value;
			_value = value;
			ValueChanged?.Invoke(oldValue, _value);
		}
	}
}