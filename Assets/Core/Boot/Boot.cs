using System.Threading;
using Core.DependencyInjection;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core.Boot {
	public class Boot: MonoBehaviour {
		[SerializeField] private BootService[] _services;

		public void Run(string nextScene = null) {
			RunAsync(nextScene).Forget();
		}

		private async UniTask RunAsync(string nextScene = null, CancellationToken token = default) {
			await Init(token);
			if (string.IsNullOrEmpty(nextScene)) {
				Debug.Log("[Boot] Loading next scene...");
				await SceneManager.LoadSceneAsync(1);
			} else {
				Debug.Log($"[Boot] Loading '{nextScene}' scene...");
				await SceneManager.LoadSceneAsync(nextScene);
			}
		}
		private async UniTask Init(CancellationToken token) {
			Debug.Log($"[Boot] Initializing {_services.Length} services...");
			foreach (var service in _services) {
				Debug.Log($"[Boot] Initializing {service.name}");
				await service.Init(GameContext.Container, token);
			}
		}
	}
}