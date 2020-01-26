using System.Collections.Generic;
using UnityEngine;
using N1D.Framework.Core;
using N1D.Framework.Dbg;

namespace N1D.App
{
	// ターゲットラインを指定時間に合わせて通過する
	// 合わせて通過するには→何秒前から表示するか
	// 
	public class TimingLine : MonoBehaviour, IGameEventReceivable
	{
		// Start is called before the first frame update
		void Start()
		{
			GameEventManager.instance.Add(gameObject);
			Metronome.instance.Initialize(m_Bpm);

			for (var i = 0; i < 100; ++i)
			{
				var guide = new Guide();
				guide.id = i;
				m_Guides.Add(guide);
			}
		}

		// Update is called once per frame
		void Update()
		{
			m_Interval = CalculateInterval(m_Bpm);
			m_DestinationTime = CalculateDestinationTime(m_Speed, m_Length);

			DrawLine(m_TimingPoint + (m_ToFromDirection * m_Length), Vector3.right, m_LineWidth, Color.yellow);
			DrawLine(m_TimingPoint, Vector3.right, m_LineWidth, Color.red);
			DrawLine(m_TimingPoint + (m_ToFromDirection * -m_BackLength), Vector3.right, m_LineWidth, Color.yellow);

			UpdateLine();
		}

		private void UpdateLine()
		{
#if false
			var delta = Time.realtimeSinceStartup - m_StartTime;
			m_Delta = delta;
			var t = delta % m_DestinationTime / m_DestinationTime;
			m_Rate = t;
			var point = Vector3.Lerp(m_TimingPoint + (m_ToFromDirection * m_Length), m_TimingPoint, t);
			DrawLine(point, Vector3.right, m_LineWidth, Color.green);
#else
			/*
			var delta = (Time.realtimeSinceStartup - m_StartTime);
			var count = (int)(delta / m_Interval);
			
			if (count > m_ProceededCount)
			{
				foreach (var guide in m_Guides)
				{
					if (guide.isActive)
					{
						continue;
					}
					Debug.Log("start->" + delta);
					guide.Start(m_Interval * ++m_ProceededCount + m_StartTime);
					if (count <= m_ProceededCount)
					{
						break;
					}
				}
			}
			*/

			var i = 0;
			foreach (var guide in m_Guides)
			{
				++i;
				if (!guide.isActive)
				{
					continue;
				}
				guide.Update(m_DestinationTime);
				var point = Vector3.Lerp(m_TimingPoint + (m_ToFromDirection * m_Length), m_TimingPoint, guide.t);
				DrawLine(point, Vector3.right, m_LineWidth, new Color(0.0f, 0.1f * guide.id % 1.0f, 0.0f));
				Debug.Log(i + "->" + guide.t);
			}

#endif
		}

		private void DrawLine(Vector3 point, Vector3 direction, float length, Color color)
		{
			var half = direction * length;
			var from = point - half;
			var to = point + half;

			Draw.instance.Line(from, to, color);
		}

		private float CalculateInterval(float bpm)
		{
			Debug.Assert(!bpm.IsZero());

			return	60.0f / bpm;
		}

		private float CalculateDestinationTime(float speed, float length)
		{
			Debug.Assert(!speed.IsZero());

			return length / speed;
		}

		public void OnReceiveGameEvent(GameEventVariant eventVariant)
		{
			switch (eventVariant.id)
			{
				case GameEventId.Beat:
				{
					OnBeat(eventVariant);
					break;
				}
			}
		}

		private void OnBeat(GameEventVariant eventVariant)
		{
			var isStarted = false;
			foreach (var guide in m_Guides)
			{
				if (guide.isActive)
				{
					continue;
				}
				guide.Start(Metronome.instance.CalculateBeatTime(eventVariant.intValue));
				isStarted = true;
				break;
			}

			Debug.Assert(isStarted, "非アクティブなガイドが見つからなかった");
		}

		// positions
		public Vector3 m_TimingPoint = Vector3.zero;
		public Vector3 m_ToFromDirection = Vector3.up;
		public float m_Length = 5.0f;
		public float m_BackLength = 3.0f;

		// parameter
		public float m_Speed = 1.0f;

		// visual
		public float m_LengthMoveTime = 1.0f;
		public float m_LineWidth = 3.0f;

		[SerializeField, Range(0.001f, 9999.0f)]
		private float m_Bpm = 120.0f;

		[SerializeField, Uneditable]
		private float m_Interval = 0.0f;
		[SerializeField, Uneditable]
		private float m_DestinationTime = 0.0f;
		[SerializeField, Uneditable]
		private float m_Delta = 0.0f;
		[SerializeField, Uneditable]
		private float m_Rate = 0.0f;
		[SerializeField, Uneditable]
		private int m_Count = 0;
		private int m_ProceededCount = 0;

		List<Guide> m_Guides = new List<Guide>();

		private class Guide
		{
			public void Start(float startTime)
			{
				m_StartTime = startTime;
				isActive = true;
			}

			public void Update(float destinationTime)
			{
				if (!isActive)
				{
					return;
				}

				var delta = Time.realtimeSinceStartup - m_StartTime;
				t = delta % destinationTime / destinationTime;

				if (delta >= destinationTime)
				{
					isActive = false;
				}
			}
			public int id { set; get; }
			public float t { private set; get; } = 0.0f;
			public bool isActive { private set; get; } = false;

			private float m_StartTime = 0.0f;
		}
	}
}