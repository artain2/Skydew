using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Custom.Extentions
{
    public static class UIExtentions
    {
        public static void AddTrigger(this EventTrigger et, EventTriggerType type, UnityAction<BaseEventData> action)
        {
            var entry = new EventTrigger.Entry {eventID = type};
            entry.callback.AddListener(action);
            et.triggers.Add(entry);
        }
    }
}