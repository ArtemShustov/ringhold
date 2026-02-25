using System;

namespace Core.UI {
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public class AutoBindAttribute: Attribute {
		public Type As { get; }

		public AutoBindAttribute(Type @as = null) {
			As = @as;
		}
	}
}