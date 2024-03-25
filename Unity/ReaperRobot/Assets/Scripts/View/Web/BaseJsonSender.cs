using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace Plusplus.ReaperRobot.Scripts.View.Web
{
    public abstract class BaseJsonSender : MonoBehaviour
    {
        protected IEnumerator SendJsonData(string url, string json)
        {
            UnityWebRequest request = new UnityWebRequest(url, "POST");
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.SetRequestHeader("Content-Type", "application/json");

            // リクエストを送信して待機
            yield return request.SendWebRequest();

            // エラーがないか確認して処理
            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("JSONデータの送信が成功しました");
            }
            else
            {
                Debug.LogError("JSONデータの送信が失敗しました: " + request.error);
            }
        }
    }
}
