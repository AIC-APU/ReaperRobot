using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UniRx;

namespace smart3tene.Reaper
{
    public class FPVCameraParamPanel : MonoBehaviour
    {
        [SerializeField] private GameObject _reaperFPVCameraPanel;
        [SerializeField] private FPVCamera _fpvCameraManager;
        [SerializeField] private TMP_Text _positonXNum;
        [SerializeField] private TMP_Text _positonYNum;
        [SerializeField] private TMP_Text _positonZNum;
        [SerializeField] private TMP_Text _rotationXNum;
        [SerializeField] private TMP_Text _rotationYNum;
        [SerializeField] private TMP_Text _rotationZNum;

        private void Awake()
        {
            //ReaperCameraの位置・角度テキスト
            _fpvCameraManager.CameraOffsetPos.Subscribe(vec =>
            {
                _positonXNum.text = vec.x.ToString("F1");
                _positonYNum.text = vec.y.ToString("F1");
                _positonZNum.text = vec.z.ToString("F1");
            });

            _fpvCameraManager.CameraOffsetRot.Subscribe(vec =>
            {
                _rotationXNum.text = ((int)vec.x).ToString();
                _rotationYNum.text = ((int)vec.y).ToString();
                _rotationZNum.text = ((int)vec.z).ToString();
            });

            ViewMode.NowViewMode.Subscribe(mode =>
            {
                if (mode == ViewMode.ViewModeCategory.REAPER_FPV)
                {
                    _reaperFPVCameraPanel.SetActive(true);
                }
                else
                {
                    _reaperFPVCameraPanel.SetActive(false);
                }
            });
        }
    }

}
