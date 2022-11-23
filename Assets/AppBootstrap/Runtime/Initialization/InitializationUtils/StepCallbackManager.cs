using System;

namespace AppBootstrap.Runtime.Initialization.InitializationUtils
{
    public class StepCallbackManager
    {
        public event Action OnAllCompleted;
        public int Count { get; set; }

        public int CompletedCount { get; private set; }
        public bool IsCompleted => CompletedCount >= Count;

        public Action Callback => AtComplete;

        public StepCallbackManager(int count)
        {
            Count = count;
        }

        private void AtComplete()
        {
            ++CompletedCount;
            // Debug.Log("CurrentCompletedCount = " + CompletedCount);
            if (CompletedCount == Count)
            {
                OnAllCompleted?.Invoke();
            }
        }
    }
}