using UnityEngine;
using UnityEngine.UI;
using UniRx;

namespace ReaperRobot.Scripts.UnityComponent.ReaperRobot.GUI
{
    public class CutterUI : MonoBehaviour
    {
        [SerializeField] private ReaperManager _reaperManager;
        [SerializeField] private Image _cutterLamp;
        [SerializeField] private Color _cuttingColor = new Color32(255, 90, 0, 255);
        [SerializeField] private Color _notCuttingColor = new Color32(196, 196, 196, 255);


        #region MonoBehaviour Callbacks
        private void Awake()
        {
            //Cutterのランプ
            _reaperManager.IsCutting.Subscribe(isCutting =>
            {
                if (isCutting)
                {
                    _cutterLamp.color = _cuttingColor;
                }
                else
                {
                    _cutterLamp.color = _notCuttingColor;
                }
            }).AddTo(this);
        }
        #endregion

        #region Public Method for Button
        public void OnClickRotate()
        {
            _reaperManager.RotateCutter(true);
        }

        public void OnClickStop()
        {
            _reaperManager.RotateCutter(false);
        }
        #endregion
    }

}