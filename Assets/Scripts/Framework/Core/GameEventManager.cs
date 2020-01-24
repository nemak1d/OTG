using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace N1D.Framework.Core
{
	public class GameEventManager : Singleton<GameEventManager>
	{
		public void Add(GameObject handler)
		{
			Debug.Assert(handler != null);

			m_Handlers.Add(handler);
		}

		public bool Remove(GameObject handler)
		{
			return m_Handlers.Remove(handler);
		}

		public void Clear()
		{
			m_Handlers.Clear();
		}

		public void Send<T>(ExecuteEvents.EventFunction<T> functor) where T : IEventSystemHandler
		{
			//foreach (var handler in m_Handlers)
			for (var i = 0; i < m_Handlers.Count; ++i)
			{
				// ループ中にハンドラーの追加を許可するためforで回す
				var handler = m_Handlers[i];
				ExecuteEvents.Execute<T>(
					handler,
					null,
					functor);
			}
		}

		List<GameObject> m_Handlers = new List<GameObject>();
	}
} // N1D.Framework.Core