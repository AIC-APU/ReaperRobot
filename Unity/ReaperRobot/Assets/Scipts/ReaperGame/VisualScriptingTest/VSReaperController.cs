using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;
using UnityEngine.InputSystem;

namespace smart3tene.Reaper
{
    public class VSReaperController : MonoBehaviour
    {
        //Visual Scriptingを試すためのテスト用スクリプトです。
        private void FixedUpdate()
        {
            float horizontal = 0f;
            float vertical = 0f;
            if (Keyboard.current.wKey.IsPressed())
            {
                vertical += 1f;
            }
            if(Keyboard.current.sKey.IsPressed())
            {
                vertical -= 1f;
            }
            if (Keyboard.current.aKey.IsPressed())
            {
                horizontal -= 1f;
            }
            if (Keyboard.current.dKey.IsPressed())
            {
                horizontal += 1f;
            }

            CustomEvent.Trigger(gameObject, "TestEvent", horizontal, vertical);
        }
    }

}
