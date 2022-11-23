using System;

namespace AppBootstrap.Runtime.Initialization.InitializationUtils
{
    public interface IInvokable
    {
        Action CompleteCallback { get; set; }
        void Invoke();
    }
}