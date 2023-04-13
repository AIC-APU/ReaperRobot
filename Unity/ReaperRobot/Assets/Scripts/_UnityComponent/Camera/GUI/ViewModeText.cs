using UnityEngine;
using TMPro;
using UniRx;

namespace ReaperRobot.Scripts.UnityComponent.Camera
{
    public class ViewModeText : MonoBehaviour
    {
        [SerializeField] TMP_Text _viewModeText;
        [SerializeField] CameraController _cameraController;

        void Start()
        {
            _cameraController
                .ActiveCamera
                .Subscribe(x => _viewModeText.text = x.ViewMode)
                .AddTo(this);
        }
    }
}