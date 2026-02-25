using System;
using System.Collections.Generic;

namespace Core.DependencyInjection {
	public class DIContainer {
		private readonly Dictionary<(Type, string), IDIEntry> _entries = new Dictionary<(Type, string), IDIEntry>();
		private readonly DIContainer _parent;
		private readonly HashSet<Type> _cache = new HashSet<Type>();
		
		public DIContainer(DIContainer parent = null) {
			_parent = parent;
		}

		public object Resolve(Type type, string id = null) {
			if (!_cache.Add(type)) {
				throw new Exception($"Cyclic dependency for {type}");
			}

			try {
				if (_entries.TryGetValue((type, id), out var entry)) {
					return entry.ResolveUptyped(this);
				} 
				if (_parent != null) {
					return _parent.Resolve(type, id);
				}
			} finally {
				_cache.Remove(type);
			}
			throw new Exception($"Could not resolve type {type}");
		}
		public T Resolve<T>(string id = null) {
			if (!_cache.Add(typeof(T))) {
				throw new Exception($"Cyclic dependency for {typeof(T)}");
			}

			try {
				if (_entries.TryGetValue((typeof(T), id), out var entry)) {
					return entry is IDIEntry<T> tEntry 
						? tEntry.Resolve(this) 
						: (T)entry.ResolveUptyped(this);
				} 
				if (_parent != null) {
					return _parent.Resolve<T>(id);
				}
			} finally {
				_cache.Remove(typeof(T));
			}
			throw new Exception($"Could not resolve type {typeof(T)}");
		}

		public void Register(IDIEntry entry, Type type, string id = null) {
			_entries.Add((type, id), entry);
		}
		public void Register<T>(IDIEntry<T> entry, string id = null) {
			_entries.Add((typeof(T), id), entry);
		}
		public InstanceDIEntry<T> RegisterInstance<T>(T instance, string id = null) {
			var entry = new InstanceDIEntry<T>(instance);
			_entries.Add((typeof(T), id), entry);
			return entry;
		}
		public FactoryDIEntry<T> RegisterFactory<T>(Func<DIContainer, T> factory, string id = null) {
			var entry = new FactoryDIEntry<T>(factory);
			_entries.Add((typeof(T), id), entry);
			return entry;
		}
	}
}