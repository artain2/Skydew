using System;
using System.Collections.Generic;
using System.Linq;

namespace AppBootstrap.Runtime.Initialization.InitializationUtils
{
    public class SerialGroup : IInvokable
    {
        public Action CompleteCallback { get; set; }
        public List<SerialGroupItem> Items = new List<SerialGroupItem>();
        public string GroupName { get; set; }

        private int _currentItemIndex = 0;

        public SerialGroup(string groupName)
        {
            GroupName = groupName;
        }

        public void Invoke()
        {
            //Debug.Log($"Start group: {GroupName}");
            _currentItemIndex = -1;
            InvokeNext();
        }

        private void InvokeNext()
        {
            ++_currentItemIndex;
            if (_currentItemIndex >= Items.Count())
            {
                //Debug.Log($"Complete group: {GroupName}");
                CompleteCallback?.Invoke();
                return;
            }

            Items[_currentItemIndex].CompleteCallback = InvokeNext;
            Items[_currentItemIndex].Invoke();
        }
    }
}