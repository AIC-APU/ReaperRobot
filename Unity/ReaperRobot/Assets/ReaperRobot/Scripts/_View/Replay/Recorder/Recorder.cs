using UnityEngine;
using UnityEngine.Events;
using UniRx;
using System;
using System.Collections.Generic;
using Plusplus.ReaperRobot.Scripts.View.ReaperRobot;
using Plusplus.ReaperRobot.Scripts.Model;

namespace Plusplus.ReaperRobot.Scripts.View.Replay
{
    public class Recorder : MonoBehaviour
    {
        #region Public Properties
        public IReadOnlyReactiveProperty<float> Time => _timer.Time;
        #endregion

        #region Serialized Fields
        [SerializeField] private ReaperManager _reaperManager;
        [SerializeField] private Transform _reaperTransform;
        [SerializeField] private string _fileName = "ReaperLog";
        #endregion

        #region Unity Events
        [SerializeField] private UnityEvent OnStartRecording;
        [SerializeField] private UnityEvent OnStopRecording;
        #endregion

        #region Reactive Properties
        public IReadOnlyReactiveProperty<ReaperDataSet> NowData => _nowData;
        private ReactiveProperty<ReaperDataSet> _nowData = new(null);
        public IReadOnlyReactiveProperty<bool> IsRecording => _isRecording;
        private ReactiveProperty<bool> _isRecording = new(false);
        #endregion

        #region Private Fields
        private List<ReaperDataSet> _dataList = new();
        private Timer _timer = new();
        #endregion

        #region Readonly Fields
        [Zenject.Inject] readonly ISaveModel _saveModel;
        #endregion

        #region MonoBehaviour Callbacks

        void Update()
        {
            if (!_isRecording.Value) return;

            //データの記録
            var time = _timer.Time.Value;
            var inputH = _reaperManager.InputH.Value;
            var inputV = _reaperManager.InputV.Value;
            var lift = _reaperManager.IsLiftDown.Value;
            var cutter = _reaperManager.IsCutting.Value;
            var positionX = _reaperTransform.position.x;
            var positionY = _reaperTransform.position.y;
            var positionZ = _reaperTransform.position.z;
            var angleY = _reaperTransform.eulerAngles.y;

            _nowData.Value = new ReaperDataSet(time, inputH, inputV, lift, cutter, positionX, positionY, positionZ, angleY);

            _dataList.Add(_nowData.Value);
        }

        void OnDestroy()
        {
            //記録の途中でゲームが終わったならそこでExportする
            if (_isRecording.Value) ExportCSV(_dataList);
        }
        #endregion

        #region Public method
        public void StartRecording()
        {
            //記録開始
            _isRecording.Value = true;
            _timer.StartTimer();
            OnStartRecording?.Invoke();
        }
        public void StopRecording()
        {
            //記録停止
            _isRecording.Value = false;
            _timer.StopTimer();
            OnStopRecording?.Invoke();

            //CSV書き出し
            ExportCSV(_dataList);

            //初期化
            _nowData.Value = null;
            _dataList.Clear();
            _timer.ResetTimer();
        }
        #endregion

        #region Private method
        private void ExportCSV(List<ReaperDataSet> dataSet)
        {
            //ファイル名作成
            var now = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            var fileName = $"{_fileName}_{now}";

            //ファイル書き出し
            Debug.Log($"{fileName}を書き出しています...");
            _saveModel.Save(dataSet, fileName);
            Debug.Log($"{fileName}を書き出しました");
        }
        #endregion
    }
}
