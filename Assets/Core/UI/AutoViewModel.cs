using Core.DependencyInjection;

namespace Core.UI {
	public interface IAutoViewModel: IViewModel {
		DIContainer GetContainer(DIContainer parent);
	}
	public abstract partial class AutoViewModel: IAutoViewModel {
		public virtual DIContainer GetContainer(DIContainer parent) => new DIContainer(parent);
	}
}