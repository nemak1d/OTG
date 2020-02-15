//-------------------------------------------
//
//  GameObjectPool
//
//-------------------------------------------
using System.Collections.Generic;
using UnityEngine;

namespace N1D.Framework.Core
{
    public class GameObjectPool<T> : MonoBehaviour where T : MonoBehaviour
    {
        //-----------------------------------
        // MonoBehaviour
        //-----------------------------------
		void Awake()
		{
			// プールオブジェクトとして運用するので、オブジェクト自身は何もしない
			gameObject.SetActive(false);
		}
		//-----------------------------------
		// Method (public)
		//-----------------------------------
#if false	// 生成する際は派生先で以下のように実装する.
		public static GameObjectPool<T> Create(int size, T original)
		{	
			var poolObject = new GameObject(typeof(T).Name + "Pool");
			var pool = poolObject.AddComponent<GameObjectPool<T>>();
			pool.Initialize(size, original);
			return pool;
		}
#endif
		public void Dispose()
		{
			if (m_Pool != null)
			{
				foreach (var instance in m_Pool)
				{
					Object.Destroy(instance);
				}
				m_Pool.Clear();
				m_Pool = null;
			}
			Size = 0;
		}

		public T Pull()
		{
			Debug.Assert(m_Pool.Count > 0);

			var instance = m_Pool.Dequeue();
			instance.gameObject.SetActive(true);
			return instance;
		}

		public void Push(T obj)
		{
			Debug.Assert(!m_Pool.Contains(obj));
			obj.gameObject.SetActive(false);
			obj.transform.SetParent(transform, true);
			m_Pool.Enqueue(obj);
		}

		//-----------------------------------
		// Method (protected)
		//-----------------------------------
		protected void Initialize(int size, T original)
		{
			Debug.Assert(size > 0);

			m_Pool = new Queue<T>(size);
			for (var i = 0; i < size; ++i)
			{
				var instance = Instantiate<T>(original);
				Push(instance);
			}
			Size = size;
		}
		//-----------------------------------
		// Property
		//-----------------------------------
		public int Size { private set; get; } = 0;
		public bool IsEmpty => (m_Pool == null || m_Pool.Count <= 0);

		//-----------------------------------
		// Field
		//-----------------------------------
		private Queue<T> m_Pool = null;
	}
} // N1D.Framework.Core