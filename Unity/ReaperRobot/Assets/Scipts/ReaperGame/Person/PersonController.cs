using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;

namespace smart3tene.Reaper
{
    [RequireComponent(typeof(PlayerInput))]
    public class PersonController : MonoBehaviour
    {
        #region Serialized Private Fields
        public GameObject Person;
        #endregion

        #region private Fields
        private PersonManager   _personManager;
        private PlayerInput     _playerInput;
        private InputActionMap  _personActionMap;
        #endregion

        #region MonoBehaviour Callbacks
        private void Awake()
        {
            if(Person == null)
            {
                Person = ReaperGameSystem.Instance.PersonInstance;
            }
            
            _personManager = Person.GetComponent<PersonManager>();
            _playerInput = GetComponent<PlayerInput>();

            _personActionMap = _playerInput.actions.FindActionMap("Person");
            _personActionMap["ChangeMode"].started += StopMove;
            _personActionMap["ChangeReaperAndPerson"].started += StopMove;
        }

        private void OnDisable()
        {
            _personActionMap["ChangeMode"].started -= StopMove;
            _personActionMap["ChangeReaperAndPerson"].started -= StopMove;
        }

        private void FixedUpdate()
        {
            if (_playerInput.currentActionMap.name != "Person") return;

            //移動
            var move = _personActionMap["Move"].ReadValue<Vector2>();
            _personManager.Move(move.x, move.y, Camera.main.transform);
        }
        #endregion

        #region Private Method
        private void StopMove(InputAction.CallbackContext obj)
        {
            _personManager.StopMove();
        }
        #endregion
    }
}