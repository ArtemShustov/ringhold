using UnityEngine;

namespace Core.UI {
	[DefaultExecutionOrder(-1000)]
	[RequireComponent(typeof(PanelSwitcher))]
	public class InitPanelsOnAwake: MonoBehaviour {
		private readonly PanelSource _source = new PanelSource();
		
		private void Awake() {
			var ui = GetComponent<PanelSwitcher>();
			
			foreach (Transform child in transform) {
				if (child.TryGetComponent<IUIPanel>(out var panel)) {
					child.gameObject.SetActive(false);
					_source.Add(panel);
				}
			}
			
			ui.AddPanelSource(_source);
		}

		private void OnDestroy() {
			var ui = GetComponent<PanelSwitcher>();
			ui?.RemovePanelSource(_source);
		}
	}
}