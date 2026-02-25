using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Core.Events.Editor {
	public static class EventBusCleaner {
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		private static void Init() {
			var eventType = typeof(IGameEvent);
			var busType = typeof(EventBus<>);
			
			var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.IsDynamic);
			var eventTypes = assemblies
				.SelectMany(GetTypesSafe)
				.Where(t => t != null 
				            && eventType.IsAssignableFrom(t) 
				            && !t.IsInterface 
				            && !t.IsAbstract
				            && t != eventType);
			
			foreach (var type in eventTypes) {
				try {
					var genericEventBusType = busType.MakeGenericType(type);
					var clearMethod = genericEventBusType.GetMethod("RemoveAllListeners", BindingFlags.Public | BindingFlags.Static);
					
					clearMethod.Invoke(null, null);
				}
				catch (Exception e) {
					Debug.LogWarning($"[EventBusCleaner] Failed to clear EventBus<{type.Name}>: {e.Message}");
				}
			}
			
			Debug.Log("[EventBusCleaner] All event buses cleared.");
		}
		
		private static Type[] GetTypesSafe(Assembly assembly) {
			try {
				return assembly.GetTypes();
			}
			catch (ReflectionTypeLoadException ex) {
				return ex.Types.Where(t => t != null).ToArray();
			}
			catch {
				return Array.Empty<Type>();
			}
		}
	}
}