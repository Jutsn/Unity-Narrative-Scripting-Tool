using UnityEngine;
using UnityEngine.Events;

namespace Justin{
public class ActionEvent : Action
{
        public string reference;
        public int unityEventIndex;
        public void InvokeEvent()
        {
            DialogManager.Instance.unityEventList[unityEventIndex].Invoke();
            print("Invoked UnityEvent at index: " + unityEventIndex);
        }
}
}