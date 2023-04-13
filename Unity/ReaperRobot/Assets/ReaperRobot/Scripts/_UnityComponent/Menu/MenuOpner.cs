using UnityEngine;
using UnityEngine.InputSystem;
using UniRx;

namespace Plusplus.ReaperRobot.Scripts.UnityComponent.Menu
{
    [RequireComponent(typeof(PlayerInput))]
    public class MenuOpner : MonoBehaviour
    {
        #region Serialized Private Fields
        [SerializeField] private GameObject _menuCanvas;

        [Header("When Menu is Open...")]
        [SerializeField] private bool _pauseGame = true;
        [SerializeField] private bool _disableController = true;
        #endregion

        #region Private Fields
        private PlayerInput _playerInput;
        private InputActionMap _menuMap;
        #endregion

        #region MonoBehaviour Callbacks
        void Awake()
        {
            //PlayerInputの初期設定
            _playerInput = GetComponent<PlayerInput>();
            _menuMap = _playerInput.actions.FindActionMap("Menu");
            _menuMap.Enable();

            //MenuCanvasを非表示にする
            _menuCanvas.SetActive(false);

            //MenuCanvasの表示状態に合わせて処理を行う
            _menuCanvas
                .ObserveEveryValueChanged(x => x.activeSelf)
                .Skip(1)
                .Subscribe(isOpen =>
                {
                    if (isOpen)
                    {
                        if (_pauseGame)
                        {
                            Time.timeScale = 0;
                        }
                        if (_disableController)
                        {
                            _playerInput.enabled = false;
                        }
                    }
                    else
                    {
                        if (_pauseGame)
                        {
                            Time.timeScale = 1;
                        }
                        if (_disableController)
                        {
                            _playerInput.enabled = true;
                        }
                    }
                })
                .AddTo(this);

            //イベント登録
            _menuMap["Open"].started += MenuAction;
        }

        private void OnDestroy()
        {   
            //イベント解除
            _menuMap["Open"].started -= MenuAction;

            //TimerScaleを元に戻す
            if(_pauseGame) Time.timeScale = 1;
        }
        #endregion

        #region Private method
        private void MenuAction(InputAction.CallbackContext obj)
        {
            _menuCanvas.SetActive(!_menuCanvas.activeSelf);
        }
        #endregion
    }
}
