using System;

namespace Plusplus.ReaperRobot.Scripts.UnityComponent.CheckPoint
{
    public static class CheckPointEvents
    {
        public static event Action OnSetupCheckPoint;
        public static void InvokeSetupCheckPoint() => OnSetupCheckPoint?.Invoke();
        public static event Action OnCheckPointPass;
        public static void InvokeCheckPointPass() => OnCheckPointPass?.Invoke(); 
        public static event Action OnAllCheckPointPass;
        public static void InvokeAllCheckPointPass() => OnAllCheckPointPass?.Invoke();
        public static event Action<string> OnTextPopup;
        public static void InvokeTextPopup(string message) => OnTextPopup?.Invoke(message);
    }
}