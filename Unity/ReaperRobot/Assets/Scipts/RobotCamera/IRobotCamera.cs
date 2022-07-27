using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace smart3tene
{
    public interface  IRobotCamera
    {
        public Camera Camera { get; set; }
        public void FollowRobot();
        public void MoveCamera(float horizontal, float vertical); //←もっといい名前を付けた
        public void RotateCamera(float horizontal, float vertical);
        public void ResetCamera();
    }
}

