using UnityEngine;

namespace ReaperRobot.Scripts.UnityComponent.Camera
{
    public class BirdViewCamera : BaseCamera
    {
        #region Readonly Field
        readonly float zoomSpeed = 1f;
        readonly float rotateSpeed = 0.5f;
        readonly float hightSpeed = 4f;
        readonly float maxHeight = 20f;
        readonly float minHeight = 0.5f;
        readonly float defaultFOV = 60f;
        #endregion

        #region Public method
        /// <summary>
        /// LateUpdateなどで毎回呼ぶことで、カメラが追従する
        /// </summary>
        public override void FollowTarget()
        {
            Camera.transform.LookAt(_target.transform.position);
        }

        public override void ResetCamera()
        {
            Camera.transform.position = _target.transform.TransformPoint(_cameraDefaultOffsetPos);
            Camera.transform.eulerAngles = _target.transform.eulerAngles + _cameraDefaultOffsetRot;
            Camera.transform.LookAt(_target.transform.position);
            Camera.fieldOfView = defaultFOV;
        }

        public override void MoveCamera(float horizontal, float vertical)
        {
            //vertical...ロボットに近づく
            var distance = Vector3.Distance(_target.transform.position, Camera.transform.position);
            if ((distance > zoomSpeed * 2f && vertical > 0) || (distance < zoomSpeed * 9f && vertical < 0))
            {
                Camera.transform.position += zoomSpeed * vertical * Camera.transform.forward;

                var cameraPos = Camera.transform.position;
                cameraPos.y = Mathf.Clamp(cameraPos.y, minHeight, maxHeight);
                Camera.transform.position = cameraPos;
            }
        }

        public override void RotateCamera(float horizontal, float vertical)
        {
            //vertical...高さを変更
            var cameraPos       = Camera.transform.position;      

            cameraPos.y += vertical * hightSpeed * Time.deltaTime;
            cameraPos.y  = Mathf.Clamp(cameraPos.y, minHeight, maxHeight);
            Camera.transform.position = cameraPos;        

            //horizontal...gameobjectを中心に回転
            var center = new Vector3(_target.transform.position.x, Camera.transform.position.y, _target.transform.position.z);
            Camera.transform.RotateAround(center, Vector3.up,  -1 * horizontal * rotateSpeed);
            Camera.transform.LookAt(_target.transform.position);
        }
        #endregion
    }

}
