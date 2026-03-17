using Core.DependencyInjection;
using Ringhold.CMS;
using Ringhold.World;
using UnityEngine;

namespace Ringhold.Buildings {
	[SelectionBase]
	public class OreDrill: MonoBehaviour, IOreDrill, ITickable {
		[Header("Settings")]
		[SerializeField, Min(0)] private int _interval = 20;
		[SerializeField] private Vector3 _throwVelocity = new Vector3(2, 5f, 0);
		
		[Header("Components")]
		[SerializeField] private Transform _itemDropRoot;
		[Inject] private ITickGroup _tickGroup;
		
		private OreVein _vein;
		private int _timer;
		
		public OreVein Vein => _vein;

		public void OnTick() {
			if (!_vein || _vein.Remaining <= 0) {
				return;
			}
			_timer += 1;

			if (_timer < _interval) {
				return;
			}
			_timer = 0;

			_vein.Take(1);
			OnResourceMined(1);
		}

		public void ClearVein() {
			SetVein(null);
		} 
		public void SetVein(OreVein vein) {
			_vein = vein;
			_timer = 0;
		}
		private void OnResourceMined(int count) {
			Debug.Log($"[{name}] Mined {count} of {_vein?.Resource?.Id ?? "null"}");
			
			if (_vein?.Resource == null) {
				return;
			}
			var resource = _vein.Resource;
			var view = GameResources.ItemsRegistry.GetDroppedItem(resource, count);
			view.transform.position = _itemDropRoot.position;
			view.ApplyVelocity(_itemDropRoot.TransformDirection(_throwVelocity));
		}

		private void OnEnable() {
			_tickGroup.Subscribe(this);
		}
		private void OnDisable() {
			_tickGroup.Unsubscribe(this);
		}
	}
}