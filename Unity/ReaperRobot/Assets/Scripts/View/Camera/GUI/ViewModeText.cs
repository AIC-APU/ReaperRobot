using UnityEngine;
using TMPro;
using UniRx;

namespace Plusplus.ReaperRobot.Scripts.View.Camera
{
    public class ViewModeText : MonoBehaviour
    {
        [SerializeField] TMP_Text _viewModeText;
        [SerializeField] CameraController _cameraController;

        void Start()
        {
            _viewModeText.text = _cameraController.ActiveCamera.ViewMode;

            _cameraController
                .ObserveEveryValueChanged(x => x.ActiveCamera)
                .Subscribe(x => _viewModeText.text = x.ViewMode)
                .AddTo(this);
        }
    }
}