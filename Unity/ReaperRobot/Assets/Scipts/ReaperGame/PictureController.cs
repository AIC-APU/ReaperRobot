using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using Cysharp.Threading.Tasks;


namespace smart3tene.Reaper
{
    //まだあくまでデバック用です
    public class PictureController : MonoBehaviour
    {
        #region Private Fields
        [SerializeField] private GUIManager _GuiManager;
        [SerializeField] private string _fileName = ""; //末に拡張子は付けない
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
                _ = TakePicture();   
            }
        }
        #endregion

        private async UniTaskVoid TakePicture()
        {
            var colorFilePath = await _pictureTaker.TakeColorPicture(_GuiManager.NowUsingCamera, _fileName);
            var colorFileName = Path.GetFileName(colorFilePath);
            GameSystem.Instance.InvokeSaveFileEvent(colorFileName);

            var tagFilePath = await _pictureTaker.TakeTagPicture(_GuiManager.NowUsingCamera, _fileName);
            var tagFileName = Path.GetFileName(tagFilePath);
            GameSystem.Instance.InvokeSaveFileEvent(tagFileName);
        }
    }

}
