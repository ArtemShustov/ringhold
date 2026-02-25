using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core.UI {
	public class PanelSwitcher: MonoBehaviour {
		private readonly List<IPanelSource> _panels = new List<IPanelSource>();
		private IUIPanel _current;

		public IUIPanel Current => _current;

		public void Change<T>(T viewModel) where T: IViewModel {
			ChangeAsync(viewModel, destroyCancellationToken).Forget();
		}
		public async UniTask<bool> ChangeAsync<T>(T viewModel, CancellationToken cancellationToken = default) where T: IViewModel {
			var panel = await GetPanel<T>(cancellationToken);
			if (panel == null) {
				Debug.LogWarning($"Could not find a suitable IUIPanel for ViewModel type {viewModel.GetType().Name}");
				return false;
			}
			panel.Bind(viewModel);
			await ChangePanel(panel, cancellationToken);
			return true;
		}
		
		public async UniTask ChangePanel(IUIPanel panel, CancellationToken token = default) {
			if (_current != null) {
				await _current.HideAsync(token);
				_current.Unbind();
			}
			_current = panel;
			if (_current != null) {
				await _current.ShowAsync(token);
			}
		}

		public UniTask HideAsync(CancellationToken token = default) {
			return _current != null ? HideAndUnbind() : UniTask.CompletedTask;

			async UniTask HideAndUnbind() {
				await _current.HideAsync(token);
				_current.Unbind();
			}
		}

		private async UniTask<IUIPanel<T>> GetPanel<T>(CancellationToken cancellationToken) where T: IViewModel {
			foreach (var source in _panels) {
				if (await source.TryRequest<T>(cancellationToken, out var panel)) {
					return panel;
				}
			}
			return null;
		} 
		
		public void AddPanelSource(IPanelSource source) {
			_panels.Add(source);
		}
		public void RemovePanelSource(IPanelSource source) {
			_panels.Remove(source);
		}
	}
}