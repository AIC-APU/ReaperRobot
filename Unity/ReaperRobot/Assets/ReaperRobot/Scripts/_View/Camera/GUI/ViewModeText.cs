using UnityEngine;
using TMPro;
using UniRx;

namespace Plusplus.ReaperRobot.Scripts.View.Camera
{
    public class ViewModeText : MonoBehaviour
    {
        [SerializeField] TMP_Text _viewModeText;
        [SerializeField] CameraManager _cameraManager;

        void Start()
        {
            _cameraManager
                .ActiveCamera
                .Subscribe(x => _viewModeText.text = x.ViewMode)
                .AddTo(this);

            _viewModeText.text = _cameraManager.ActiveCamera.Value.ViewMode;
        }
    }
}