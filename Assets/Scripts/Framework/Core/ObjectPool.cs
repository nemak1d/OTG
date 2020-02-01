//-------------------------------------------
//
//  オブジェクトプール.
//
//-------------------------------------------
using System.Collections.Generic;
using UnityEngine;

namespace N1D.Framework.Core
{
    public class ObjectPool<T> where T : class, new()
    {
		public ObjectPool(int size)
		{
			Debug.Assert(size > 0);
			m_Pool = new Queue<T>(size);
			for (var i = 0; i < size; ++i)
			{
				m_Pool.Enqueue(new T());
			}
			Size = size;
		}

		public void Dispose()
		{
			if (m_Pool != null)
			{
				m_Pool.Clear();
				m_Pool = null;
			}
			Size = 0;
		}

		public T Pull()
		{
			Debug.Assert(m_Pool.Count > 0);

			return m_Pool.Dequeue();
		}

		public void Push(T obj)
		{
			Debug.Assert(!m_Pool.Contains(obj));

			m_Pool.Enqueue(obj);
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
