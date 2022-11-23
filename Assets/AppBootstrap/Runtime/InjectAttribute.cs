using System;

namespace AppBootstrap.Runtime
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public class InjectAttribute : Attribute
	{
		public InjectAttribute() {}

		public InjectAttribute(string name)
		{
			Name = name;
		}

		public string Name { get; private set; }
	}
}
