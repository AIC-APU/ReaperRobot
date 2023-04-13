using UnityEngine;

namespace Plusplus.ReaperRobot.Scripts.UnityComponent.GUI
{
    public class ShowAndHide : MonoBehaviour
    {
        [SerializeField] private GameObject _targetObject;
        [SerializeField] private bool _defaultActive = true;

        private void Awake()
        {
            _targetObject.SetActive(_defaultActive);
        }

        //ボタンに登録する関数
        public void ShowAndHideFunc()
        {
            //targetObjectの,表示・非表示を切り替える
            _targetObject.SetActive(!_targetObject.activeSelf);
        }
    }
}