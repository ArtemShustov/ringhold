using System;
using UnityEngine;

namespace Core.UI {
	public interface IView {
		Type ViewModelType { get; }
		bool TryBind(IViewModel viewModel);
		void Unbind();
	}

	public interface IView<T>: IView where T: IViewModel {
		void Bind(T viewModel);
	}
	
	public abstract class View<T>: MonoBehaviour, IView<T> where T : IViewModel {
		public Type ViewModelType => typeof(T);
		public T ViewModel { get; private set; }
		
		public bool TryBind(IViewModel viewModel) {
			if (viewModel is T tViewModel) {
				Bind(tViewModel);
				return true;
			} 
			return false;
		}
		public void Bind(T viewModel) {
			Unbind();
			ViewModel = viewModel;
			if (ViewModel != null) {
				OnBind(ViewModel);
			}
		}
		public void Unbind() {
			if (ViewModel != null) {
				OnUnbind(ViewModel);
			}
			if (ViewModel is IDisposable disposable) {
				disposable.Dispose();
			}
			ViewModel = default(T);
		}
		
		protected virtual void OnBind(T viewModel) { }
		protected virtual void OnUnbind(T viewModel) { }

		protected void OnDestroy() {
			if (ViewModel is IDisposable disposable) {
				disposable.Dispose();
			}
		}
	}
}