using System;

namespace Core.DependencyInjection {
	public class LazyDIEntry<T>: IDIEntry<T> {
		private readonly Lazy<T> _instance;

		public LazyDIEntry(Func<T> factory) {
			_instance = new Lazy<T>(factory);
		}

		public T Resolve(DIContainer container) => _instance.Value;
		public object ResolveUptyped(DIContainer container) => Resolve(container);
	}
}