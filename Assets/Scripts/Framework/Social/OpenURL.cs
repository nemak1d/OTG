using UnityEngine;


namespace N1D.Social
{
	public class OpenURL : MonoBehaviour
	{
		public void Open(string url)
		{
#if UNITY_EDITOR
			Application.OpenURL(url);
#elif UNITY_WEBGL
			// WebGLの場合は、ゲームプレイ画面と同じウィンドウでツイート画面が開かないよう、処理を変える
			Application.ExternalEval(string.Format("window.open('{0}','_blank')", url));
#else
			  Application.OpenURL(url);
#endif
		}
	}
} // N1D.Social