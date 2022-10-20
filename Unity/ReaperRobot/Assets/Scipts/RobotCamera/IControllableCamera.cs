using UnityEngine;

namespace smart3tene
{
    public interface  IControllableCamera
    {
        public Transform Target { get; set; }
        public Camera Camera { get; set; }
        public void FollowTarget();
        public void MoveCamera(float horizontal, float vertical); //←もっといい名前を付けた
        public void RotateCamera(float horizontal, float vertical);
        public void ResetCamera();
    }
}

