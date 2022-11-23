using System;
using System.Reflection;

namespace AppBootstrap.Runtime.Initialization.InitializationUtils
{
    public class SerialGroupItem: IInvokable
    {
        public Action CompleteCallback { get; set; }

        public object Target;
        public MethodInfo Method;
        public bool HasCallback;

        public SerialGroupItem(object target, MethodInfo method, bool hasCallback)
        {
            Target = target;
            Method = method;
            HasCallback = hasCallback;
        }

        public void Invoke()
        {
            //Debug.Log($"____Start method: {Method.Name} at {Target.GetType().Name} {HasCallback}");
            if (HasCallback)
            {
                Action callback = AtMethodComplete;
                Method.Invoke(Target, new object[] {callback});
            }
            else
            {
                Method.Invoke(Target, null);
                AtMethodComplete();
            }
        }

        private void AtMethodComplete()
        {
            //Debug.Log($"____Complete method: {Method.Name} at {Target.GetType().Name}");
            CompleteCallback?.Invoke();
        }
    }
}