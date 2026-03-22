using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ringhold.Utils {
	public class ComponentOverlapSphere<T> {
		private readonly float _radius;
		private readonly LayerMask _mask;

		private readonly Dictionary<T, ObjectInfo> _current = new Dictionary<T, ObjectInfo>();

		private readonly HashSet<T> _locatedBuffer = new HashSet<T>(32);
		private readonly List<T> _toAddBuffer = new List<T>(16);
		private readonly List<T> _toRemoveBuffer = new List<T>(16);
		private readonly Collider[] _hitsBuffer = new Collider[16];
		private readonly List<T> _componentsBuffer = new List<T>(4);
		
		public IReadOnlyDictionary<T, ObjectInfo> Current => _current;
		public IReadOnlyList<T> AddBuffer => _toAddBuffer;
		public IReadOnlyList<T> RemoveBuffer => _toRemoveBuffer;
		
		public event Action<T> Entered;
		public event Action<T> Exited;

		public ComponentOverlapSphere(float radius, LayerMask mask) {
			_radius = radius;
			_mask = mask;
		}

		public virtual void Update(Vector3 position) {
			_locatedBuffer.Clear();
			_toAddBuffer.Clear();
			
			var size = Physics.OverlapSphereNonAlloc(position, _radius, _hitsBuffer, _mask);
			for (var i = 0; i < size; i++) {
				var hit = _hitsBuffer[i];
				hit.GetComponents(_componentsBuffer);

				var objInfo = new ObjectInfo() {
					GameObject = hit.gameObject,
					Transform = hit.transform,
				};
				
				foreach (var component in _componentsBuffer) {
					if (_current.TryAdd(component, objInfo)) {
						_toAddBuffer.Add(component);
						OnEnter(component);
					}
					_locatedBuffer.Add(component);
				}
			}

			_toRemoveBuffer.Clear();
			foreach (var (component, _) in _current) {
				if ((!_locatedBuffer.Contains(component)) || (component is Component c && c == null)) {
					_toRemoveBuffer.Add(component);
				}
			}
			foreach (var component in _toRemoveBuffer) {
				_current.Remove(component);
				OnExit(component);
			}
		}
		
		public virtual T GetClosest(Vector3 position) {
			T closest = default;
			var closestSqrDistance = float.MaxValue;

			foreach (var (component, info) in _current) {
				var sqrDistance = (info.Transform.position - position).sqrMagnitude;
				if (sqrDistance < closestSqrDistance) {
					closestSqrDistance = sqrDistance;
					closest = component;
				}
			}

			return closest;
		}

		protected virtual void OnEnter(T component) {
			Entered?.Invoke(component);
		}
		protected virtual void OnExit(T component) {
			Exited?.Invoke(component);
		}
		
		public struct ObjectInfo {
			public GameObject GameObject;
			public Transform Transform;
		}
	}
}