using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Core.Utils.Editor {
	public static class ClearOnReload {
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		private static void Init() {
			var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.IsDynamic);
			var allTypes = assemblies.SelectMany(GetTypesSafe).Where(t => t != null);

			foreach (var type in allTypes) {
				foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)) {
					if (field.IsDefined(typeof(ClearOnReloadAttribute), false)) {
						try {
							field.SetValue(null, null);
						}
						catch (Exception e) {
							Debug.LogWarning($"[ClearOnReload] Failed to clear field {type.Name}.{field.Name}: {e.Message}");
						}
					}
				}

				foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)) {
					if (property.IsDefined(typeof(ClearOnReloadAttribute), false) && property.CanWrite) {
						try {
							property.SetValue(null, null);
						}
						catch (Exception e) {
							Debug.LogWarning($"[ClearOnReload] Failed to clear property {type.Name}.{property.Name}: {e.Message}");
						}
					}
				}
			}
			
			Debug.Log("[ClearOnReload] All fields cleared.");
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