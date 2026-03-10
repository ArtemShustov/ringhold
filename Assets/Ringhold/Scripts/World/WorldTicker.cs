using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Ringhold.World {
	public interface ITickable {
		void OnTick();
	}

	public interface ITickGroup {
		void Subscribe(ITickable tickable);
		void Unsubscribe(ITickable tickable);
	}

	public class TickGroup: ITickGroup {
		private readonly List<ITickable> _tickables = new List<ITickable>();

		public void Subscribe(ITickable tickable) => _tickables.Add(tickable);
		public void Unsubscribe(ITickable tickable) => _tickables.Remove(tickable);

		public void TickAll() {
			foreach (var tickable in _tickables) {
				tickable.OnTick();
			}
		}
	}

	public class WorldTicker: MonoBehaviour {
		[SerializeField] private int _targetTPS = 20;
		private readonly TickGroup _main = new TickGroup();
		private CancellationTokenSource _cts;

		public ITickGroup Main => _main;

		private void OnEnable() {
			_cts = new CancellationTokenSource();
			RunTickLoop(_cts.Token).Forget();
		}

		private void OnDisable() {
			_cts?.Cancel();
			_cts?.Dispose();
		}

		private async UniTask RunTickLoop(CancellationToken token) {
			while (!token.IsCancellationRequested) {
				var intervalMs = 1000 / _targetTPS;
				var next = System.DateTime.UtcNow.AddMilliseconds(intervalMs);

				_main.TickAll();

				var delay = next - System.DateTime.UtcNow;
				if (delay.TotalMilliseconds > 0) {
					await UniTask.Delay((int)delay.TotalMilliseconds, cancellationToken: token);
				}
			}
		}
	}
}