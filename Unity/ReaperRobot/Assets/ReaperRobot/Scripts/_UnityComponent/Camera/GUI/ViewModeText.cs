using UnityEngine;
using TMPro;
using UniRx;

namespace Plusplus.ReaperRobot.Scripts.UnityComponent.Camera.GUI
{
    public class ViewModeText : MonoBehaviour
    {
        [SerializeField] TMP_Text _viewModeText;
        [SerializeField] CameraController _cameraController;

        void Start()
        {
            _viewModeText.text = _cameraController.ActiveCamera.Value.ViewMode;

            _cameraController
                .ActiveCamera
                .Subscribe(x => _viewModeText.text = x.ViewMode)
                .AddTo(this);
        }
    }
}