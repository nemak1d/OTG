using UnityEngine;

namespace N1D.Framework.Core
{
	public class Singleton<T> : MonoBehaviour where T : Component
	{
		public static T instance
		{
			get
			{
				if (_instance == null)
				{
					var previous = FindObjectOfType(typeof(T));
					if (previous)
					{
						Debug.LogWarningFormat("Initialized twice. Don't use {0} in the scene hierarchy.", typeof(T));
						_instance = (T)previous;
					}
					else
					{
						var go = new GameObject(typeof(T).Name);
						_instance = go.AddComponent<T>();
						DontDestroyOnLoad(go);
						//go.hideFlags = HideFlags.HideInHierarchy;
					}
				}
				return _instance;
			}
		}

		static T _instance = null;
	}
} // N1D.Framework.Core