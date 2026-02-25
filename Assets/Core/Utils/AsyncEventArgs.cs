using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Core.Utils {
	public class AsyncEventArgs {
		private readonly List<UniTask> _tasks = new();
		public readonly CancellationToken CancellationToken;

		public AsyncEventArgs(CancellationToken token) {
			CancellationToken = token;
		}

		public virtual void AddTask(UniTask task) {
			if (CancellationToken.IsCancellationRequested) {
				return;
			}
			_tasks.Add(task);
		}

		public UniTask WaitAll() => UniTask.WhenAll(_tasks);

		public readonly static AsyncEventArgs Empty = new VoidAsyncEventArgs(CancellationToken.None);

		private class VoidAsyncEventArgs: AsyncEventArgs {
			public VoidAsyncEventArgs(CancellationToken token): base(token) { }
			public override void AddTask(UniTask task) => task.Forget();
		}
	}
}