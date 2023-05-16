using UnityEngine;
using UnityEngine.UI;
using UniRx;
using Cysharp.Threading.Tasks;
using Plusplus.ReaperRobot.Scripts.View.ReaperRobot;

namespace Plusplus.ReaperRobot.Scripts.View.Replay
{
    public class RecorderButton : MonoBehaviour
    {
        #region Serialized Private Fields
        [SerializeField] private Recorder _recorder;
        [SerializeField] private ReaperController _reaperController;
        [SerializeField] private Button _recordButton;
        [SerializeField] private Button _stopButton;
        [SerializeField] private Button _backButton;
        #endregion

        #region Private Fields

        #endregion

        #region MonoBehaviour Callbacks
        void Awake()
        {
            _recordButton.onClick.AddListener(PlayButton);
            _stopButton.onClick.AddListener(StopButton);
            _backButton.onClick.AddListener(BackButton);

            SetIntaractable(true, false, false);

            _reaperController.enabled = false;
        }
        #endregion

        #region Private method
        private void PlayButton()
        {
            _recorder.StartRecording();
            SetIntaractable(false, true, true);
            _reaperController.enabled = true;
        }

        private async void StopButton()
        {
            _reaperController.enabled = false;
            SetIntaractable(false, false, false);

            //書き込みが終わるまで待つ
            await _recorder.StopRecording();

            SetIntaractable(false, false, true);
        }

        private void BackButton()
        {
            _recorder.ResetRecording();
            SetIntaractable(true, false, false);
            _reaperController.enabled = false;
        }

        private void SetIntaractable(bool play, bool stop, bool back)
        {
            _recordButton.interactable = play;
            _stopButton.interactable = stop;
            _backButton.interactable = back;
        }
        #endregion
    }
}
