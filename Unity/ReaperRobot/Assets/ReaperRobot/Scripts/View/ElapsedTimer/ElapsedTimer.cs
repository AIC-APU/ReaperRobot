using UnityEngine;
using Plusplus.ReaperRobot.Scripts.Model;

namespace Plusplus.ReaperRobot.Scripts.View.ElapsedTimer
{
    public class ElapsedTimer : MonoBehaviour
    {
        #region Public Fields
        public float Time => _timer.Time.Value;
        public bool IsTimerRunning => _timer.IsRunning.Value;
        #endregion

        #region Serialized Private Fields
        //インスペクタ上で表示するためのフィールド（デバッグ用）
        [SerializeField] private float _nowTime = 0f;
        [SerializeField] private bool _startOnAwake = false;
        #endregion

        #region Private Fields
        private Timer _timer;
        #endregion

        #region MonoBehaviour Callbacks
        void Awake()
        {
            _timer = new Timer();
            
            if (_startOnAwake)
            {
                _timer.StartTimer();
            }
        }

        void Update()
        {
           //インスペクタ上で表示するためのフィールドの更新（デバッグ用）
            _nowTime = _timer.Time.Value;
        }
        #endregion

        #region Public method
        public void StartTimer() => _timer.StartTimer();
        public void StopTimer() => _timer.StopTimer();
        public void ResetTimer() => _timer.ResetTimer();
        #endregion
    }
}
