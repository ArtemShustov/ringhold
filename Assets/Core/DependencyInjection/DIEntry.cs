namespace Core.DependencyInjection {
	public interface IDIEntry<out T>: IDIEntry {
		public T Resolve(DIContainer container);
	}

	public interface IDIEntry {
		public object ResolveUptyped(DIContainer container);
	}
}