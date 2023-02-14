using UnityEngine;

namespace ReaperRobot.Scripts.UnityComponent.GUI
{
    public class ShowAndHideButton : MonoBehaviour
    {
        [SerializeField] private GameObject _targetObject;
        [SerializeField] private bool _defaultActive = true;

        private void Awake()
        {
            _targetObject.SetActive(_defaultActive);
        }

        //ボタンに登録する関数
        public void ShowAndHide()
        {
            //targetObjectの,表示・非表示を切り替える
            _targetObject.SetActive(!_targetObject.activeSelf);
        }
    }
}