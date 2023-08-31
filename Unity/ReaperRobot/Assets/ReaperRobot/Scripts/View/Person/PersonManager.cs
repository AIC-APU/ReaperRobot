using UnityEngine;

namespace Plusplus.ReaperRobot.Scripts.View.Person
{
    [RequireComponent(typeof(Rigidbody))]
    public class PersonManager : MonoBehaviour
    {
        #region Private Fields
        private Rigidbody _rigidBody;
        private Animator _animator;
        #endregion

        #region Readonly Fields
        readonly float moveSpeed = 1.5f;
        #endregion

        #region MonoBehaviour Callbacks
        private void Awake()
        {
            _rigidBody = GetComponent<Rigidbody>();
            _animator = GetComponent<Animator>();
        }

        private void Update()
        {
            //アニメーション操作
            var speed = new Vector2(_rigidBody.velocity.x, _rigidBody.velocity.z).magnitude;
            _animator.SetFloat("Speed", speed, 0.05f, Time.deltaTime);
        }
        #endregion

        #region Public Method
        public void Move(float horizontal, float vertical, Transform camera)
        {
            var cameraForward = Vector3.Scale(camera.forward, new Vector3(1, 0, 1)).normalized;

            var moveForward = moveSpeed * vertical * cameraForward + moveSpeed * horizontal * camera.right;

            _rigidBody.velocity = moveForward * moveSpeed + new Vector3(0, _rigidBody.velocity.y, 0);

            if (moveForward == Vector3.zero)
            {
                _rigidBody.angularVelocity = Vector3.zero;
            }
            else
            {
                transform.rotation = Quaternion.LookRotation(moveForward, Vector3.up);
            }
        }

        public void StopMove()
        {
            _rigidBody.velocity = new Vector3(0, _rigidBody.velocity.y, 0);
            _rigidBody.angularVelocity = Vector3.zero;
        }
        #endregion
    }
}
