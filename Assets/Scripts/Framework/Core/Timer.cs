using UnityEngine;

namespace N1D.Framework.Core
{
	public class Timer
	{
		public void Start(float time)
		{
			m_StartTime = time;
			m_LeftTime = m_StartTime;
			m_IsTimeOutNow = false;
		}

		public void Stop()
		{
			m_StartTime = 0.0f;
			m_LeftTime = 0.0f;
			m_IsTimeOutNow = false;
		}

		public void Update()
		{
			m_IsTimeOutNow = false;
			if (m_LeftTime <= 0.0f)
			{
				return;
			}

			m_LeftTime -= Time.deltaTime;
			if (m_LeftTime <= 0.0f)
			{
				m_LeftTime = 0.0f;
				m_IsTimeOutNow = true;
			}
		}

		public bool IsTimeOutNow()
		{
			return m_IsTimeOutNow && IsTimeOut();
		}

		public bool IsTimeOut()
		{
			return m_LeftTime <= 0.0f;
		}

		float m_StartTime = 0.0f;
		float m_LeftTime = 0.0f;
		bool m_IsTimeOutNow = false;

	}
} // N1D.Framework.Core