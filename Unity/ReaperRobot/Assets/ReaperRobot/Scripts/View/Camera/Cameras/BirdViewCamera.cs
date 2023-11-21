using UnityEngine;

namespace Plusplus.ReaperRobot.Scripts.View.Camera
{
    public class BirdViewCamera : BaseCamera
    {
        #region Readonly Field
        readonly float zoomSpeed = 1f;
        readonly float rotateSpeed = 0.5f;
        readonly float hightSpeed = 4f;
        readonly float maxHeight = 10f;
        readonly float minHeight = 0.5f;
        readonly float defaultFOV = 60f;
        #endregion

        #region Public method
        /// <summary>
        /// LateUpdateなどで毎回呼ぶことで、カメラが追従する
        /// </summary>
        public override void FollowTarget()
        {
            _camera.transform.LookAt(_target.transform.position);
        }

        public override void ResetCamera()
        {
            _camera.transform.position = _target.transform.TransformPoint(_cameraDefaultOffsetPos);
            _camera.transform.eulerAngles = _target.transform.eulerAngles + _cameraDefaultOffsetRot;
            _camera.transform.LookAt(_target.transform.position);
            _camera.fieldOfView = defaultFOV;
        }

        public override void MoveCamera(float horizontal, float vertical)
        {
            //vertical...ロボットに近づく
            var distance = Vector3.Distance(_target.transform.position, _camera.transform.position);
            if ((distance > zoomSpeed * 2f && vertical > 0) || (distance < zoomSpeed * 9f && vertical < 0))
            {
                _camera.transform.position += zoomSpeed * vertical * _camera.transform.forward;

                var cameraPos = _camera.transform.position;
                cameraPos.y = Mathf.Clamp(cameraPos.y, minHeight, maxHeight);
                _camera.transform.position = cameraPos;
            }
        }

        public override void RotateCamera(float horizontal, float vertical)
        {
            //vertical...高さを変更
            var cameraPos       = _camera.transform.position;      

            cameraPos.y += vertical * hightSpeed * Time.deltaTime;
            cameraPos.y  = Mathf.Clamp(cameraPos.y, minHeight, maxHeight);
            _camera.transform.position = cameraPos;        

            //horizontal...gameobjectを中心に回転
            var center = new Vector3(_target.transform.position.x, _camera.transform.position.y, _target.transform.position.z);
            _camera.transform.RotateAround(center, Vector3.up,  -1 * horizontal * rotateSpeed);
            _camera.transform.LookAt(_target.transform.position);
        }
        #endregion
    }

}
