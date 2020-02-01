//-------------------------------------------
//
//  Beatマネージャー.
//
//-------------------------------------------
using System;
using System.Collections.Generic;
using UnityEngine;
using N1D.Framework.Core;

namespace N1D.App
{
	public struct BeatEvent
	{
		public Action<Beat> onStart;
		public Action<Beat> onUpdate;
		public Action<Beat> onStop;
	}
    public class BeatManager
    {
		//-----------------------------------
		// Method (public)
		//-----------------------------------
		public void Initialize(int size, BeatEvent eventSettings)
		{
			m_Pool = new ObjectPool<Beat>(size);
			m_Actives = new Queue<Beat>(size);
			m_EventSettings = eventSettings;
		}
		public void Reset()
		{
			foreach (var obj in m_Actives)
			{
				obj.Stop();
				m_EventSettings.onStop?.Invoke(obj);
				m_Pool.Push(obj);
			}
			m_Actives.Clear();
		}
		public void Update(int destinationTime)
		{
			foreach (var obj in m_Actives)
			{
				obj.Update(destinationTime);
				m_EventSettings.onUpdate?.Invoke(obj);
			}
		}

		public void Add(int startTime, int targetTime)
		{
			Beat obj = null;
			if (m_Pool.IsEmpty)
			{
				if (m_Actives.Count > 0)
				{
					obj = m_Actives.Dequeue();
					// 到達していない場合はそもそも足りない説ある
					Debug.Assert(obj.Progress >= 1.0f);
					obj.Stop();
					m_EventSettings.onStop?.Invoke(obj);
				}
			}
			else
			{
				obj = m_Pool.Pull();
			}

			obj.Start(startTime, targetTime);
			m_EventSettings.onStart?.Invoke(obj);
			m_Actives.Enqueue(obj);
		}
		//-----------------------------------
		// Field
		//-----------------------------------
		private ObjectPool<Beat> m_Pool = null;
		private Queue<Beat> m_Actives = null;
		private BeatEvent m_EventSettings;
    }
} // N1D.App
