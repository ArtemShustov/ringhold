using System.Threading;
using Core.DependencyInjection;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core.Boot {
	public abstract class BootService: MonoBehaviour {
		public abstract UniTask Init(DIContainer container, CancellationToken cancellationToken = default);
	}
}