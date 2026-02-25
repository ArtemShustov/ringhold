using System;

namespace Core.DependencyInjection {
	public class FactoryDIEntry<T>: IDIEntry<T> {
		private readonly Func<DIContainer, T> _factory;
		
		public FactoryDIEntry(Func<DIContainer, T> factory) {
			_factory = factory;
		}
		
		public T Resolve(DIContainer container) => _factory(container);
		public object ResolveUptyped(DIContainer container) => Resolve(container);
	}
}