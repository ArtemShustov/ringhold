using System.Threading;
using Cysharp.Threading.Tasks;

namespace Core.UI {
	public interface IUIPanel: IView {
		UniTask ShowAsync(CancellationToken token = default);
		UniTask HideAsync(CancellationToken token = default);
	}
	public interface IUIPanel<T>: IUIPanel, IView<T> where T: IViewModel { }
	
	public abstract class UIPanel<T>: View<T>, IUIPanel<T> where T: IViewModel {
		public virtual UniTask ShowAsync(CancellationToken token = default) {
			gameObject.SetActive(true);
			return UniTask.CompletedTask;
		}
		public virtual UniTask HideAsync(CancellationToken token = default) {
			gameObject.SetActive(false);
			return UniTask.CompletedTask;
		}
	}
}