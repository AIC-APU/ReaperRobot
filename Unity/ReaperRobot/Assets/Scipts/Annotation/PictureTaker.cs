using Cysharp.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

namespace smart3tene
{
    public class PictureTaker : MonoBehaviour
    {
        #region Private Fields
        [SerializeField] private Camera _colorCamera;
        [SerializeField] private Camera _tagCamera;
        #endregion

        #region MonoBehaviour Callbacks
        private void Awake()
        {
            _tagCamera.SetReplacementShader(Shader.Find("IndexTexture"), null);
        }
        private void OnDestroy()
        {
            _tagCamera.ResetReplacementShader();
        }
        #endregion

        #region Public method
        public async UniTask<string> TakeColorPicture(Camera camera, int imageWidth, int imageHeigth, string fileName = "" , int fileNum = 0, string directory = "")
        {
            if (fileName == "") fileName = "picture";
            if (directory == "") directory = Application.streamingAssetsPath + "/../../../AnnotationImages/images";
            var fileNumString = fileNum.ToString("D4");

            if (fileName.EndsWith(".png")) fileName = fileName.Remove(fileName.Length - 4, 4);

            var filePath = Path.GetFullPath($"{directory}/{fileName}_color_{fileNumString}.png");
            filePath = NumberingFilePath(filePath);

            //指定されたカメラと同じ位置に撮影用カメラを移動させる
            _colorCamera.transform.position = camera.transform.position;
            _colorCamera.transform.rotation = camera.transform.rotation;

            //撮影
            await CaptureFromCamera.Capture(imageWidth, imageHeigth, _colorCamera, TextureFormat.RGB24, filePath);

            Debug.Log($"{filePath} にファイルを保存しました。");

            return filePath;
        }

        public async UniTask<string> TakeTagPicture(Camera camera, int imageWidth, int imageHeigth, string fileName = "" , int fileNum = 0, string directory = "")
        {
            if (fileName == "") fileName = "picture";
            if (directory == "") directory = Application.streamingAssetsPath + "/../../../AnnotationImages/tags";
            var fileNumString = fileNum.ToString("D4");

            if (fileName.EndsWith(".png")) fileName = fileName.Remove(fileName.Length - 4, 4);

            var filePath = Path.GetFullPath($"{directory}/{fileName}_tag_{fileNumString}.png");
            filePath = NumberingFilePath(filePath);

            //指定されたカメラと同じ位置に撮影用カメラを移動させる
            _tagCamera.transform.position = camera.transform.position;
            _tagCamera.transform.rotation = camera.transform.rotation;

            //撮影
            await CaptureFromCamera.Capture(imageWidth, imageHeigth, _tagCamera, TextureFormat.R8, filePath);

            Debug.Log($"{filePath} にファイルを保存しました。");

            return filePath;
        }
        #endregion

        #region Private method
        /// <summary>
        /// ディレクトリに同名のファイルがあれば、ファイル名に番号を付けたパスを返す。
        /// </summary>
        private string NumberingFilePath(string filePath)
        {
            //ディレクトリに同名のパスがあれば、それに番号を付けた物を返す。
            var directry = Path.GetDirectoryName(filePath);
            var fileName = Path.GetFileNameWithoutExtension(filePath);
            var extension = Path.GetExtension(filePath);
            
            while (File.Exists($"{directry}/{fileName}{extension}"))
            {
                //同名のファイルが存在した場合、ファイル名の末尾の数字を足す
                fileName = IncrementStringEnd(fileName);
            }

            return $"{directry}/{fileName}{extension}";
        }

        private string IncrementStringEnd(string str)
        {
            //ファイルネームの最後に数字がついているかを判定
            if (EndWithNumber(str))
            {
                var end = str.Substring(str.Length - 1);
                var others = str.Substring(0, str.Length - 1);
     
                if (Regex.IsMatch(end, "[0-8]"))
                {
                    //末尾が0-8であればその数に1を足す
                    end = (int.Parse(end) + 1).ToString();
                }
                else if(end == "9")
                {
                    //末尾が9であれば上の位の数を増やして、9を0にする
                    others = IncrementStringEnd(others);
                    end = "0";
                }
                str = others + end;
            }
            else
            {
                //ついていないなら1を付ける
                str += "1";
            }

            return str;
        }

        private bool EndWithNumber(string str)
        {
            var pattern = "([0-9]+$)";
            return Regex.IsMatch(str, pattern);
        }
        #endregion
    }
}

