using UnityEngine;
using UnityEngine.UI;
using UniRx;

namespace Plusplus.ReaperRobot.Scripts.View.ReaperRobot.GUI
{
    public class LiftUI : MonoBehaviour
    {
        [SerializeField] private ReaperManager _reaperManager;
        [SerializeField] private Image _liftLamp;
        [SerializeField] private Color _liftDownColor = new Color32(255, 90, 0, 255);
        [SerializeField] private Color _liftUpColor = new Color32(196, 196, 196, 255);

        #region MonoBehaviour Callbacks
        private void Awake()
        {
            //Liftのランプ
            _reaperManager.IsLiftDown.Subscribe(isDown =>
            {
                if (isDown)
                {
                    _liftLamp.color = _liftDownColor;
                }
                else
                {
                    _liftLamp.color = _liftUpColor;
                }
            }).AddTo(this);
        }
        #endregion

        #region Public Method for Button
        public void OnClickDown()
        {
            _reaperManager.MoveLift(true);
        }

        public void OnClickUp()
        {
            _reaperManager.MoveLift(false);
        }
        #endregion
    }

}
