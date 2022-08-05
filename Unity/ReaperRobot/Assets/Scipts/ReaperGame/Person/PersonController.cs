using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;

namespace smart3tene.Reaper
{
    [RequireComponent(typeof(PlayerInput))]
    public class PersonController : MonoBehaviour, ICameraController
    {
        #region Serialized Private Fields
        public GameObject Person;
        #endregion

        #region Public Fields
        public IControllableCamera CCamera 
        {
            get => _controllableCamera;
            set
            {
                _controllableCamera = value;
                _controllableCamera.ResetCamera();
            } 
        }
        private IControllableCamera _controllableCamera;
        #endregion

        #region Serialized Private Fields
        [SerializeField, Tooltip("ここからIControllableCameraを設定することもできます（デバッグ用）")] private GameObject _controllableCameraObject;
        #endregion

        #region private Fields
        private PersonManager _personManager;
        private PlayerInput _playerInput;
        private InputActionMap _personAction;
        #endregion

        #region MonoBehaviour Callbacks
        private void Awake()
        {
            _personManager = Person.GetComponent<PersonManager>();
            _playerInput = GetComponent<PlayerInput>();

            //インターフェースの取得
            if(_controllableCameraObject != null)
            {
                _controllableCamera = _controllableCameraObject.GetComponent<IControllableCamera>();
            }

            _personAction = _playerInput.actions.FindActionMap("Person");

            //viewmodeが変わった時にstop処理をする
            ViewMode.NowViewMode
                .Where(mode => mode != ViewMode.ViewModeCategory.PERSON_TPV)
                .Subscribe(_ => _personManager.StopMove())
                .AddTo(this);
        }

        private void LateUpdate()
        {
            if (_playerInput.currentActionMap.name != "Person") return;

            //カメラの回転
            _controllableCamera.FollowTarget();

            var move = _personAction["Look"].ReadValue<Vector2>();
            _controllableCamera.RotateCamera(move.x, move.y);
        }

        private void FixedUpdate()
        {
            if (_playerInput.currentActionMap.name != "Person") return;

            //移動
            var move = _personAction["Move"].ReadValue<Vector2>();
            _personManager.Move(move.x, move.y, _controllableCamera.Camera.transform);
        }
        #endregion

    }
}