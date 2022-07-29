using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


namespace smart3tene.Reaper
{
    //まだあくまでデバック用です
    public class PictureController : MonoBehaviour
    {
        #region Private Fields
        [SerializeField] private GUIManager _GuiManager;
        private PictureTaker _pictureTaker;        
        #endregion

        #region MonoBehaviour Callbacks
        void Start()
        {
            _pictureTaker = GetComponent<PictureTaker>();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                _ = _pictureTaker.TakeColorPicture(_GuiManager.NowUsingCamera);

                _ = _pictureTaker.TakeTagPicture(_GuiManager.NowUsingCamera);
            }
        }
        #endregion
    }

}
