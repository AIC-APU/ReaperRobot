using Cysharp.Threading.Tasks;
using System.IO;
using UnityEngine;


namespace smart3tene.Reaper
{
    //デバック用です
    public class PictureController : MonoBehaviour
    {
        #region Private Fields
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
            var colorFilePath = await _pictureTaker.TakeColorPicture(Camera.main, _fileName);
            var colorFileName = Path.GetFileName(colorFilePath);

            var colorText = $"{colorFileName} was saved";
            ReaperEventManager.InvokeTextPopupEvent(colorText);

            var tagFilePath = await _pictureTaker.TakeTagPicture(Camera.main, _fileName);
            var tagFileName = Path.GetFileName(tagFilePath);

            var tagText = $"{tagFileName} was saved";
            ReaperEventManager.InvokeTextPopupEvent(tagText);
        }
    }

}
