using System;

namespace Core.Utils {
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public class ClearOnReloadAttribute: Attribute { }
}