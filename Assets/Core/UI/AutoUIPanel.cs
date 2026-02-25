namespace Core.UI {
	public abstract class AutoUIPanel<T>: UIPanel<T> where T: IAutoViewModel {
		protected override void OnBind(T viewModel) => OnBindOthers(viewModel);
		protected override void OnUnbind(T viewModel) => OnUnbindOthers(viewModel);
		
		protected virtual void OnBindOthers(T viewModel) { }
		protected virtual void OnUnbindOthers(T viewModel) { }
	}
}