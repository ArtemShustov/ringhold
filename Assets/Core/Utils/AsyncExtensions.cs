using System;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core.Utils {
	public static class AsyncExtensions {
		public static void Forget(this Task task) {
			if (task == null)
				return;

			task.ContinueWith(t => {
				if (t.Exception != null) {
					Debug.LogError($"Error in Task: {t.Exception.InnerException}");
				}
			}, TaskScheduler.FromCurrentSynchronizationContext());
		}
		public static async UniTask WaitForEvent(Action<Action> subscribe, Action<Action> unsubscribe, CancellationToken token, Action onPerformed = default) {
			var tcs = new UniTaskCompletionSource();
			subscribe(Handler);

			await using (token.Register(Cancel)) {
				await tcs.Task;
			}

			void Cancel() {
				unsubscribe(Handler);
				tcs.TrySetCanceled(token);
			}
			void Handler() {
				unsubscribe(Handler);
				onPerformed?.Invoke();
				tcs.TrySetResult();
			}
		}
		public static async UniTask WaitForEvent<T>(Action<Action<T>> subscribe, Action<Action<T>> unsubscribe, CancellationToken token, Action<T> onPerformed = default) {
			var tcs = new UniTaskCompletionSource();
			subscribe(Handler);

			await using (token.Register(Cancel)) {
				await tcs.Task;
			}

			void Cancel() {
				unsubscribe(Handler);
				tcs.TrySetCanceled(token);
			}
			void Handler(T arg) {
				unsubscribe(Handler);
				onPerformed?.Invoke(arg);
				tcs.TrySetResult();
			}
		}
	}
}