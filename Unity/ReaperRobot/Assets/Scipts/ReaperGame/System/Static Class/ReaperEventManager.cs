using System;

namespace smart3tene.Reaper
{
    public static class ReaperEventManager
    {
        public static event Action GameOverEvent;
        public static  void InvokeGameOverEvent() => GameOverEvent?.Invoke();

        public static event Action OpenMenuEvent;
        public static void InvokeOpenMenuEvent() => OpenMenuEvent?.Invoke();

        public static event Action CloseMenuEvent;
        public static void InvokeCloseMenuEvent() => CloseMenuEvent?.Invoke();

        public static event Action ResetEvent;
        public static void InvokeResetEvent() => ResetEvent?.Invoke();

        public static event Action<string> SaveFileEvent;
        public static void InvokeSaveFileEvent(string fileName) => SaveFileEvent?.Invoke(fileName);

        public static event Action FullReapRateEvent;
        public static void InvokeFullReapRateEvent() => FullReapRateEvent?.Invoke();

        public static event Action AllCheckPointPassEvent;
        public static void InvokeAllCheckPointPassEvent() => AllCheckPointPassEvent?.Invoke();

        public static event Action CheckPointPassEvent;
        public static void InvokeCheckPointPassEvent() => CheckPointPassEvent?.Invoke();

        public static event Action<string> TextPopupEvent;
        public static void InvokeTextPopupEvent(string text) => TextPopupEvent?.Invoke(text);

        public static event Action PenaltyEvent;
        public static void InvokePenaltyEvent() => PenaltyEvent?.Invoke();

    }
}
