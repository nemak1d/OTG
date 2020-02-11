using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Networking;


namespace N1D.Framework.Social
{
	public class Tweet : MonoBehaviour
	{
		public void RequestCapture()
		{
			StartCoroutine(UploadCapture());
		}

		public void ShowWindow(string body, string hashTag)
		{
			var builder = new StringBuilder();
			builder.Append("http://twitter.com/intent/tweet?text=" + WWW.EscapeURL(body));

			if (m_Progress == UploadProgress.Succeeded)
			{
				builder.Append("&url=" + m_UploadedURL);
			}
			builder.Append("&hashtags=" + hashTag);

#if UNITY_EDITOR
			Application.OpenURL(builder.ToString());
#elif UNITY_WEBGL
			// WebGLの場合は、ゲームプレイ画面と同じウィンドウでツイート画面が開かないよう、処理を変える
			Application.ExternalEval(string.Format("window.open('{0}','_blank')", builder.ToString()));
#else
			  Application.OpenURL(builder.ToString());
#endif
		}

		IEnumerator UploadCapture()
		{
			m_Progress = UploadProgress.Processing;
			m_UploadedURL = string.Empty;
#if true
			yield return new WaitForEndOfFrame();

			var texture = new Texture2D(Screen.width, Screen.height);
			texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
			texture.Apply();
			var image = texture.EncodeToJPG();
#else
			var fileName = string.Format("{0:yyyyMMddHmmss}", DateTime.Now);
			var filePath = Application.persistentDataPath + "/" + fileName + ".png";
			ScreenCapture.CaptureScreenshot(filePath);

			var startTime = Time.time;
			while (File.Exists(filePath) == false)
			{
				if (Time.time - startTime > 6.0f)
				{
					m_Progress = UploadProgress.Failed;
					yield break;
				}
				else
				{
					yield return null;
				}
			}
			var image = File.ReadAllBytes(filePath);
			File.Delete(filePath);
#endif
			WWWForm wwwForm = new WWWForm();
			wwwForm.AddField("image", Convert.ToBase64String(image));
			wwwForm.AddField("type", "base64");

			UnityWebRequest www;
			www = UnityWebRequest.Post("https://api.imgur.com/3/image.xml", wwwForm);
			www.SetRequestHeader("AUTHORIZATION", "Client-ID " + ClientID);
			yield return www.SendWebRequest();


			if (www.isNetworkError)
			{
				Debug.Log(www.error);
				m_Progress = UploadProgress.Failed;
			}
			else
			{
				XDocument xDoc = XDocument.Parse(www.downloadHandler.text);
				m_UploadedURL = xDoc.Element("data").Element("link").Value;
				m_Progress = UploadProgress.Succeeded;
			}
		}

		enum UploadProgress
		{
			Processing,
			Failed,
			Succeeded,
		}

		public bool IsCaptured { get { return m_Progress != UploadProgress.Processing; } }

		const string ClientID = "d6a20a7efcb2731";
		string m_UploadedURL = string.Empty;
		UploadProgress m_Progress = UploadProgress.Failed;
	}
} // N1D.Framework.Social