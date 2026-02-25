using Core.DependencyInjection;
using UnityEngine;

namespace Core.UI {
	public class PanelSwitcherRegistrator: Registrator {
		[SerializeField] private PanelSwitcher _switcher;
		
		public override void RegisterAll(DIContainer container) {
			_switcher.transform.SetParent(null);
			DontDestroyOnLoad(_switcher.gameObject);
			container.RegisterInstance(_switcher);
		}
	}
}