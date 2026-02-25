using System;
using System.Collections;
using System.Collections.Generic;

namespace Core.Utils {
	public interface IReadOnlyReactiveList<T>: IReadOnlyList<T> {
		event Action<T> ItemAdded;
		event Action<T> ItemRemoved;
		event ValueChanged<T> ItemChanged;
		event Action ListChanged;
	}
	public class ReactiveList<T>: IReadOnlyReactiveList<T>, IList<T> {
		private readonly List<T> _list;

		public event Action<T> ItemAdded;
		public event Action<T> ItemRemoved;
		public event ValueChanged<T> ItemChanged;
		public event Action ListChanged;
		
		public bool IsReadOnly => false;
		public int Count => _list.Count;

		public T this[int index] {
			get => _list[index];
			set {
				var oldValue = _list[index];
				if (EqualityComparer<T>.Default.Equals(oldValue, value)) {
					return;
				}
				_list[index] = value;
				ItemChanged?.Invoke(oldValue, value);
				ListChanged?.Invoke();
			}
		}

		public ReactiveList() {
			_list = new List<T>();
		}
		public ReactiveList(IEnumerable<T> collection) {
			_list = new List<T>(collection);
		}

		public void Insert(int index, T item) {
			_list.Insert(index, item);
			ItemAdded?.Invoke(item);
			ListChanged?.Invoke();
		}
		
		public void Add(T item) {
			_list.Add(item);
			ItemAdded?.Invoke(item);
			ListChanged?.Invoke();
		}
		
		public bool Remove(T item) {
			if (!_list.Remove(item)) {
				return false;
			}
			ItemRemoved?.Invoke(item);
			ListChanged?.Invoke();
			return true;
		}
		public bool Remove(Predicate<T> match) {
			int index = _list.FindIndex(match);
			if (index == -1) return false;
            
			RemoveAt(index);
			return true;
		}
		public void RemoveAt(int index) {
			var item = _list[index];
			_list.RemoveAt(index);
			ItemRemoved?.Invoke(item);
			ListChanged?.Invoke();
		}
		public void RemoveAll(Predicate<T> match) {
			for (int i = _list.Count - 1; i >= 0; i--) {
				var item = _list[i];
				if (match(item)) {
					_list.RemoveAt(i);
					ItemRemoved?.Invoke(item);
				}
			}
			ListChanged?.Invoke();
		}
		
		public void Clear() {
			foreach (var item in _list) {
				ItemRemoved?.Invoke(item);
			}
			_list.Clear();
			ListChanged?.Invoke();
		}
		
		public int IndexOf(T item) => _list.IndexOf(item);
		public bool Contains(T item) => _list.Contains(item);
		public void CopyTo(T[] array, int arrayIndex) => _list.CopyTo(array, arrayIndex);
		
		public IEnumerator<T> GetEnumerator() => _list.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}