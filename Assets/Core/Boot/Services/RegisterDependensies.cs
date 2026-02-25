using System.Threading;
using Core.DependencyInjection;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core.Boot.Services {
	public class RegisterDependencies: BootService {
		[SerializeField] private Registrator[] _registrators;
		
		public override UniTask Init(DIContainer container, CancellationToken cancellationToken = default) {
			foreach (var registrator in _registrators) {
				registrator.RegisterAll(container);
			}
			return UniTask.CompletedTask;
		}
	}
}