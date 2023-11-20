using UnityEngine;
using UnityEngine.UI;

namespace Plusplus.ReaperRobot.Scripts.View.ReaperRobot
{
    public class PositionResetButton : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private GameObject _reaperRobot;

        private Vector3 _startPosition;
        private Quaternion _startRotation;
        void Awake()
        {
            _startPosition = _reaperRobot.transform.position;
            _startRotation = _reaperRobot.transform.rotation;
        }

        void OnEnable()
        {
            _button.onClick.AddListener(ResetPosition);
        }

        void OnDisable()
        {
            _button.onClick.RemoveListener(ResetPosition);
        }

        public void ResetPosition()
        {
            _reaperRobot.transform.position = _startPosition;
            _reaperRobot.transform.rotation = _startRotation;
        }
    }
}
