using UnityEngine;

namespace Plusplus.ReaperRobot.Scripts.View.CheckPoint
{
    public class TouchCheckPoint : BaseCheckPoint
    {
        #region Serialized Private Fields
        [SerializeField] private GameObject _target;
        #endregion

        #region Private Fields
        private bool _isActive = false;
        #endregion

        #region MonoBehaviour Callbacks
        void Awake()
        {
            gameObject.SetActive(false);
        }
        void OnTriggerEnter(Collider other)
        {
            if (_isActive && other.gameObject == _target)
            {
                _isChecked.Value = true;
            }
        }
        #endregion

        #region Public method
        public override void InitializeCheckPoint()
        {
            _isActive = true;
            gameObject.SetActive(true);
        }

        public override void FinalizeCheckPoint()
        {
            _isActive = false;
            gameObject.SetActive(false);
        }
        #endregion
    }
}
