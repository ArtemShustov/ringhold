using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Core.UI {
	public interface IPanelSource {
		UniTask<bool> TryRequest<T>(CancellationToken cancellationToken, out IUIPanel<T> instance) where T: IViewModel;
	}

	public class PanelSource: IPanelSource {
		private readonly Dictionary<Type, IUIPanel> _panels = new Dictionary<Type, IUIPanel>();

		public void Add(IUIPanel panel) {
			_panels.Add(panel.ViewModelType, panel);
		}
		public void Remove(IUIPanel panel) {
			_panels.Remove(panel.ViewModelType);
		}
		
		public UniTask<bool> TryRequest<T>(CancellationToken cancellationToken, out IUIPanel<T> instance) where T: IViewModel {
			var result = _panels.TryGetValue(typeof(T), out var aInstance);
			instance = aInstance as IUIPanel<T>;
			return UniTask.FromResult(result);
		}
	}
}