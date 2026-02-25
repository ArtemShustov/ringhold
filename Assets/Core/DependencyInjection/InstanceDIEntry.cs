namespace Core.DependencyInjection {
	public class InstanceDIEntry<T>: IDIEntry<T> {
		private readonly T _instance;
		
		public InstanceDIEntry(T instance) {
			_instance = instance;
		}
		
		public T Resolve(DIContainer container) => _instance;
		public object ResolveUptyped(DIContainer container) => Resolve(container);
	}
}