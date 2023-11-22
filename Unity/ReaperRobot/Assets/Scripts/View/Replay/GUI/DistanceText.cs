using UnityEngine;
using TMPro;

namespace Plusplus.ReaperRobot.Scripts.View.Replay
{
    public class DistanceText : MonoBehaviour
    {
        #region Serialized Private Fields
        [SerializeField] private ReplayManager _replayManager;
        [SerializeField] private Transform _reaperTransform;
        [SerializeField] private Transform _shadowReaperTransform;
        [SerializeField] private TMP_Text _nowDistanceText;
        [SerializeField] private TMP_Text _maxDistanceText;
        #endregion

        #region Private Fields
        private float _nowDistance = 0;
        private float _maxDistance = 0;
        #endregion

        #region MonoBehaviour Callbacks
        void Awake()
        {
            _replayManager.OnReset += ResetDistance;
        }

        void OnDestroy()
        {
            _replayManager.OnReset -= ResetDistance;
        }
        void Update()
        {
            if (_replayManager.IsReplaying.Value)
            {
                _nowDistance = Vector3.Distance(_reaperTransform.position, _shadowReaperTransform.position);
                _maxDistance = Mathf.Max(_maxDistance, _nowDistance);
                _nowDistanceText.text = _nowDistance.ToString("F2");
                _maxDistanceText.text = _maxDistance.ToString("F2");
            }
        }
        #endregion

        #region Private method
        private void ResetDistance()
        {
            _nowDistance = 0;
            _maxDistance = 0;
            _nowDistanceText.text = _nowDistance.ToString("F2");
            _maxDistanceText.text = _maxDistance.ToString("F2");
        }
        #endregion
    }
}
