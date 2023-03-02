using System;

namespace ReaperRobot.Scripts.UnityComponent.Menu
{
    public static class MenuEvent
    {
        public static event Action OpenMenuEvent;
        public static void InvokeOpenMenuEvent() => OpenMenuEvent?.Invoke();

        public static event Action CloseMenuEvent;
        public static void InvokeCloseMenuEvent() => CloseMenuEvent?.Invoke();
    }
}