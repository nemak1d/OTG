using N1D.Framework.Core;
using N1D.Framework.Dbg;
using N1D.Framework.Sound;
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
			for (var i = 0; i < 100; ++i)
			{
				var note = new Beat();
				m_Notes.Add(note);
			}
		}

		void Update()
		{
			Metronome.instance.Bpm = m_Bpm;
			m_DestinationTime = CalculateDestinationTime(m_TimingTime, m_Speed);

			UpdateInput();
			UpdateMusic();
			UpdateLine();
			UpdateTrackSheet();

			MightJudge();
		}

		private void UpdateInput()
		{
			if (Input.anyKeyDown)
			{
				m_IsInput = true;
			}
		}
		private void UpdateMusic()
		{
			if (m_StartPlayTime <= Metronome.instance.Time)
			{
				if (m_MusicHandle == null)
				{
					m_MusicHandle = SoundManager.instance.PlayBGM("popepope");
				}
			}
		}

		private void UpdateTrackSheet()
		{
			if (timing == null)
			{
				return;
			}

			for (var i = processTimingCount; i < timing.Length; ++i)
			{
				// 表示開始は曲の時間 - 表示してから実際に入力するまでの時間 + 曲再生開始時間
				var startTime = timing[i] - m_TimingTime + m_StartPlayTime;
				if (startTime <= Metronome.instance.Time)
				{
					foreach (var note in m_Notes)
					{
						if (note.IsActive)
						{
							continue;
						}
						note.Start(startTime, timing[i]);
						++processTimingCount;
						break;
					}
				}
			}
			var endPoint = m_Direction * m_Length + m_StartPoint;
			var timingPoint = Vector3.Lerp(m_StartPoint, endPoint, m_TimingLineRate);
			foreach (var note in m_Notes)
			{
				if (!note.IsActive)
				{
					continue;
				}
				note.Update(m_DestinationTime);
				var point = Vector3.Lerp(m_StartPoint, endPoint, note.Progress);
				DrawCircle(point, m_LineWidth * 0.3f, Color.red);

				// 前後のラインを測るには内部の式出さないといかんなーとか思っちゃうところ。
				var earlyPerfect = note.CalculateProgress(Metronome.instance.Time - m_PerfectDelta, m_DestinationTime);
				var latePerfect = note.CalculateProgress(Metronome.instance.Time + m_PerfectDelta, m_DestinationTime);
				var earlyGreat = note.CalculateProgress(Metronome.instance.Time - m_GreatDelta, m_DestinationTime);
				var lateGreat = note.CalculateProgress(Metronome.instance.Time + m_GreatDelta, m_DestinationTime);
				var earlyGood = note.CalculateProgress(Metronome.instance.Time - m_GoodDelta, m_DestinationTime);
				var lateGood = note.CalculateProgress(Metronome.instance.Time + m_GoodDelta, m_DestinationTime);

				point = Vector3.Lerp(m_StartPoint, endPoint, earlyPerfect);
				DrawLine(point, Vector3.right, m_LineWidth * 0.3f, Color.yellow);
				point = Vector3.Lerp(m_StartPoint, endPoint, latePerfect);
				DrawLine(point, Vector3.right, m_LineWidth * 0.3f, Color.yellow);

				point = Vector3.Lerp(m_StartPoint, endPoint, earlyGreat);
				DrawLine(point, Vector3.right, m_LineWidth * 0.3f, Color.cyan);
				point = Vector3.Lerp(m_StartPoint, endPoint, lateGreat);
				DrawLine(point, Vector3.right, m_LineWidth * 0.3f, Color.cyan);

				point = Vector3.Lerp(m_StartPoint, endPoint, earlyGood);
				DrawLine(point, Vector3.right, m_LineWidth * 0.3f, Color.blue);
				point = Vector3.Lerp(m_StartPoint, endPoint, lateGood);
				DrawLine(point, Vector3.right, m_LineWidth * 0.3f, Color.blue);
			}
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

		private float CalculateProgress(int startTime, int currentTime)
		{
			var delta = currentTime - startTime; ;
			return Mathf.Clamp01((float)delta % (float)m_DestinationTime / (float)m_DestinationTime);
		}

		private void MightJudge()
		{
			if (m_IsInput)
			{
				Beat next = null;
				foreach (var note in m_Notes)
				{
					if (!note.IsActive)
					{
						continue;
					}

					if (next == null)
					{
						next = note;
					}
					// 最遅判定時間をオーバーせず先頭なら
					else if (note.TargetTime + m_GoodDelta < Metronome.instance.Time
						&& note.TargetTime < next.TargetTime)
					{
						next = note;
					}
				}

				if (next != null)
				{
					var time = Metronome.instance.Time;
					if (Mathf.Abs(next.TargetTime - time) <= m_PerfectDelta)
					{
						Debug.Log("Perfect!");
					}
					else if (Mathf.Abs(next.TargetTime - time) <= m_GreatDelta)
					{
						Debug.Log("Great!");
					}
					else if (Mathf.Abs(next.TargetTime - time) <= m_GoodDelta)
					{
						Debug.Log("Good!");
					}
					else
					{
						Debug.Log("Miss...");
					}
					next.Stop();
				}
			}

			m_IsInput = false;
		}

		private void DrawLine(Vector3 point, Vector3 direction, float length, Color color)
		{
			var half = direction * length;
			var from = point - half;
			var to = point + half;

			Draw.instance.Line(from, to, color);
		}
		private void DrawCircle(Vector3 point, float radius, Color color)
		{
			Draw.instance.Circle(point, radius, color);
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
				var time = Metronome.instance.CalculateBeatTime(eventVariant.intValue);
				guide.Start(time, time);
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
		public int m_StartPlayTime = 3000;
		public float m_Speed = 1.0f;
		public int m_TimingTime = 5000;

		// visual
		public float m_LineWidth = 3.0f;

		[SerializeField, Range(0.001f, 300.0f)]
		private float m_Bpm = 120.0f;

		[SerializeField, Uneditable]
		private int m_DestinationTime = 0;

		List<Beat> m_Guides = new List<Beat>();
		List<Beat> m_Notes = new List<Beat>();

		[SerializeField]
		private int[] timing = null;
		private int processTimingCount = 0;


		[SerializeField]
		private int m_PerfectDelta = 33;
		[SerializeField]
		private int m_GreatDelta = 99;
		[SerializeField]
		private int m_GoodDelta = 264;
		[SerializeField]

		private bool m_IsInput = false;

		private AudioHandler m_MusicHandle = null;
		

	}
}