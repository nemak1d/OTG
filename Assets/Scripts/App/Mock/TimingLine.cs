using N1D.Framework.Core;
using N1D.Framework.Dbg;
using System.Collections.Generic;
using UnityEngine;

namespace N1D.App
{
	public class TimingLine : MonoBehaviour, IGameEventReceivable
	{
		void Start()
		{
			GameEventManager.instance.Add(gameObject);
			Metronome.instance.Initialize(m_Bpm);

			for (var i = 0; i < 100; ++i)
			{
				var guide = new Beat();
				m_Guides.Add(guide);
			}
		}

		void Update()
		{
			Metronome.instance.Bpm = m_Bpm;
			m_DestinationTime = CalculateDestinationTime(m_TimingTime, m_Speed);

			UpdateLine();
		}

		private void UpdateLine()
		{
			var endPoint = m_Direction * m_Length + m_StartPoint;
			var timingPoint = Vector3.Lerp(m_StartPoint, endPoint, m_TimingLineRate);
			DrawLine(m_StartPoint, Vector3.right, m_LineWidth, Color.yellow);
			DrawLine(endPoint, Vector3.right, m_LineWidth, Color.yellow);
			DrawLine(timingPoint, Vector3.right, m_LineWidth, Color.red);

			var i = 0;
			foreach (var guide in m_Guides)
			{
				++i;
				if (!guide.IsActive)
				{
					continue;
				}
				guide.Update(m_DestinationTime);
				var point = Vector3.Lerp(m_StartPoint, endPoint, guide.Progress);
				DrawLine(point, Vector3.right, m_LineWidth, Color.green);
			}
		}

		private void DrawLine(Vector3 point, Vector3 direction, float length, Color color)
		{
			var half = direction * length;
			var from = point - half;
			var to = point + half;

			Draw.instance.Line(from, to, color);
		}
		private int CalculateDestinationTime(int arriveTimeAtTimingLine, float speed)
		{
			Debug.Assert(!speed.IsZero());

			var time = arriveTimeAtTimingLine / (m_Length * m_TimingLineRate);
			return (int)((float)(time * m_Length) / speed);
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
				if (guide.IsActive)
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
		public Vector3 m_StartPoint = Vector3.zero;
		public Vector3 m_Direction = Vector3.down;
		public float m_Length = 5.0f;
		public float m_TimingLineRate = 0.7f;

		// parameter
		public float m_Speed = 1.0f;
		public int m_TimingTime = 5000;

		// visual
		public float m_LineWidth = 3.0f;

		[SerializeField, Range(0.001f, 300.0f)]
		private float m_Bpm = 120.0f;

		[SerializeField, Uneditable]
		private int m_DestinationTime = 0;

		List<Beat> m_Guides = new List<Beat>();
	}
}