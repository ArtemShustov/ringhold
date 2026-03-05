using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Core.DependencyInjection {
	public static class Injecting {
		public static void Inject(object instance, DIContainer container) {
			var type = instance.GetType();

			if (!_cache.TryGetValue(type, out var injectors)) {
				injectors = BakeType(type);
				_cache[type] = injectors;
			}

			for (int i = 0; i < injectors.Length; i++) {
				injectors[i].Inject(instance, container);
			}
		}
		
		#region GameObjects
		public static void InjectAllOnScene(DIContainer container) {
			var objects = UnityEngine.Object.FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None);
			
			foreach (var instance in objects) {
				Inject(instance, container);
			}
		}
		
		public static void Inject(GameObject gameObject, DIContainer container) {
			foreach (var component in gameObject.GetComponents<MonoBehaviour>()) {
				Inject(component, container);
			}
		}
		public static void InjectTree(GameObject gameObject, DIContainer container) {
			var childs = new Queue<GameObject>();
			childs.Enqueue(gameObject);
			while (childs.Count > 0) {
				var target = childs.Dequeue();
				Inject(target, container);
				
				foreach (Transform child in target.transform) {
					childs.Enqueue(child.gameObject);
				}
			}
		}

		public static T Instantiate<T>(T prefab, DIContainer container) where T: Component {
			var wasActive = prefab.gameObject.activeSelf;
			prefab.gameObject.SetActive(false);
			
			var instance = UnityEngine.Object.Instantiate(prefab);
			InjectTree(instance.gameObject, container);
			instance.gameObject.SetActive(wasActive);
			
			prefab.gameObject.SetActive(wasActive);
			return instance;
		}
		public static T Instantiate<T>(T prefab, Transform parent, DIContainer container) where T: Component {
			var wasActive = prefab.gameObject.activeSelf;
			prefab.gameObject.SetActive(false);
			
			var instance = UnityEngine.Object.Instantiate(prefab, parent);
			InjectTree(instance.gameObject, container);
			
			instance.gameObject.SetActive(wasActive);
			return instance;
		}
		public static T Instantiate<T>(T prefab, Vector3 position, Quaternion rotation, DIContainer container) where T: Component {
			var wasActive = prefab.gameObject.activeSelf;
			prefab.gameObject.SetActive(false);
			
			var instance = UnityEngine.Object.Instantiate(prefab, position, rotation);
			InjectTree(instance.gameObject, container);
			
			instance.gameObject.SetActive(wasActive);
			return instance;
		}
		#endregion
		
		#region Internal
		private readonly static Dictionary<Type, FieldInjector[]> _cache = new();
		private const BindingFlags Flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
		
		private static FieldInjector[] BakeType(Type type) {
			var injectors = new List<FieldInjector>();
			var currentType = type;

			while (currentType != null && currentType != typeof(object)) {
				var fields = currentType.GetFields(Flags);
				foreach (var field in fields) {
					var attr = field.GetCustomAttribute<InjectAttribute>();
					if (attr == null) {
						continue;
					}
					
					var injector = new FieldInjector(field, attr.Id);
					injectors.Add(injector);
				}
				currentType = currentType.BaseType;
			}

			return injectors.ToArray();
		}
		private class FieldInjector {
			private readonly FieldInfo _fieldInfo;
			private readonly string _id;

			public FieldInjector(FieldInfo fieldInfo, string id) {
				_fieldInfo = fieldInfo;
				_id = id;
			}

			public void Inject(object instance, DIContainer container) {
				var dependency = container.Resolve(_fieldInfo.FieldType, _id);
				_fieldInfo.SetValue(instance, dependency);
			}
		}
		#endregion
	}
}