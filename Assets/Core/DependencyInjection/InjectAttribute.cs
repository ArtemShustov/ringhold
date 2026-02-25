using System;

namespace Core.DependencyInjection {
	[AttributeUsage(AttributeTargets.Field)]
	public class InjectAttribute: Attribute {
		public readonly string Id;
		
		public InjectAttribute(string id = null) {
			Id = id;
		}
	}
}