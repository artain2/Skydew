using System;

namespace AppBootstrap.Runtime
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class InitAttribute : Attribute
    {
        public string Name { get; set; }


        public InitAttribute(string name)
        {
            Name = name;
        }
    }
}