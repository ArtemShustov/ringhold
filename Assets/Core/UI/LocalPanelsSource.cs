using Core.DependencyInjection;
using UnityEngine;

namespace Core.UI {
	[DefaultExecutionOrder(-1000)]
	public class LocalPanelsSource: MonoBehaviour {
		[SerializeField] private bool _disableOnAwake = true;
		[Inject] private PanelSwitcher _switcher;
		private readonly PanelSource _source = new PanelSource();
		
		private void Awake() {
			foreach (Transform child in transform) {
				if (child.TryGetComponent<IUIPanel>(out var panel)) {
					if (_disableOnAwake) {
						child.gameObject.SetActive(false);
					}
					_source.Add(panel);
				}
			}
		}

		private void OnEnable() {
			_switcher.AddPanelSource(_source);
		}
		private void OnDisable() {
			_switcher.RemovePanelSource(_source);
		}
	}
}