using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public static class Util
{
    public static IEnumerator LoadImageFromURL(string URL, Action<Sprite> callback)
    {
        if (!string.IsNullOrEmpty(URL))
        {
            // Check internet connection
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                yield return null;
            }

            UnityWebRequest www = UnityWebRequestTexture.GetTexture(URL);
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log("NetworkError: " + www.error);
                callback(null);
            }
            else
            {
                Texture2D loadedTexture = DownloadHandlerTexture.GetContent(www);
                callback(Sprite.Create(loadedTexture, new Rect(0f, 0f, loadedTexture.width, loadedTexture.height), Vector2.zero, 16, 0, SpriteMeshType.FullRect));
            }
        }
    }

    public static  IEnumerator ApiRequest(string url, Action<string> callback)
    {
        UnityWebRequest www = new UnityWebRequest();
        www = UnityWebRequest.Get(url);

        //auth
        www.SetRequestHeader("Authorization", "Bearer e05079bc-8457-4452-9882-57240276e829");
        //www.SetRequestHeader("x-api-key", "e05079bc-8457-4452-9882-57240276e829");

        www.downloadHandler = new DownloadHandlerBuffer();

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log("NetworkError: " + www.error);
            callback(null);
        }
        else
        {
            string rawJson = Encoding.Default.GetString(www.downloadHandler.data);
            callback(rawJson);
        }
    }
}
