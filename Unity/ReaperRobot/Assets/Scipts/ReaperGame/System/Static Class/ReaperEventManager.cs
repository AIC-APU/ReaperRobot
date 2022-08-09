using System;

namespace smart3tene.Reaper
{
    public static class ReaperEventManager
    {
        public static event Action GameOverEvent;
        public static  void InvokeGameOverEvent() => GameOverEvent?.Invoke();

        public static event Action MenuEvent;
        public static void InvokeMenuEvent() => MenuEvent?.Invoke();

        public static event Action ResetEvent;
        public static void InvokeResetEvent() => ResetEvent?.Invoke();

        public static event Action<string> SaveFileEvent;
        public static void InvokeSaveFileEvent(string fileName) => SaveFileEvent?.Invoke(fileName);

        public static event Action FullReapRateEvent;
        public static void InvokeFullReapRateEvent() => FullReapRateEvent?.Invoke();

        public static event Action AllCheckPointPathEvent;
        public static void InvokeAllCheckPointPassEvent() => AllCheckPointPathEvent?.Invoke();
    }
}
