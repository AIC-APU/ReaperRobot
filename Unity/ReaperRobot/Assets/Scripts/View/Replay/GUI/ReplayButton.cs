using System;
using UnityEngine;
using UnityEngine.UI;

namespace Plusplus.ReaperRobot.Scripts.View.Replay
{
    public class ReplayButton : MonoBehaviour
    {
        #region Serialized Fields
        [SerializeField] private ReplayManager _replayManager;
        [SerializeField] private Button _playButton;
        [SerializeField] private Button _pauseButton;
        [SerializeField] private Button _backButton;
        #endregion

        #region private Fields
        private Action _resetButtonIntaractable;
        #endregion

        #region MonoBehaviour Callbacks
        void Awake()
        {
            _playButton.onClick.AddListener(PlayButton);
            _pauseButton.onClick.AddListener(PauseButton);
            _backButton.onClick.AddListener(BackButton);

            SetIntaractable(false, false, false);

            _resetButtonIntaractable = () => SetIntaractable(true, false, true);
            _replayManager.OnReset += _resetButtonIntaractable;
        }

        void OnDestroy()
        {
            _playButton.onClick.RemoveListener(PlayButton);
            _pauseButton.onClick.RemoveListener(PauseButton);
            _backButton.onClick.RemoveListener(BackButton);

            _replayManager.OnReset -= _resetButtonIntaractable;
        }
        #endregion

        #region Private method
        private void PlayButton()
        {
            _replayManager.StartReplay();

            SetIntaractable(false, true, true);
        }

        private void PauseButton()
        {
            _replayManager.PauseReplay();

            SetIntaractable(true, false, true);
        }

        private void BackButton()
        {
            _replayManager.ResetReplay();

            SetIntaractable(true, false, true);
        }

        private void SetIntaractable(bool play, bool pause, bool back)
        {
            _playButton.interactable = play;
            _pauseButton.interactable = pause;
            _backButton.interactable = back;
        }
        #endregion
    }
}
