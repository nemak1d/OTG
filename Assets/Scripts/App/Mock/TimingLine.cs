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
			Metronome.instance.Initialize();

			InitializeRhythm();
			InitializeNote();
		}

		void Update()
		{
			Metronome.instance.SetBPM(Bpm);
			Metronome.instance.SetSpeed(Speed);
			m_DestinationTime = CalculateDestinationTime(m_TimingTime);

			UpdateInput();
			UpdateMusic();
			UpdateLine();
			UpdateTrackSheet();

			MightJudge();
		}

		private void InitializeRhythm()
		{
			var eventSettings = new BeatEvent();
			eventSettings.onUpdate = DrawRhythmLine;

			m_Rhythm.Initialize(m_RhythmSize, eventSettings);
		}

		private void InitializeNote()
		{
			var eventSettings = new BeatEvent();
			eventSettings.onUpdate = OnUpdateNote;

			m_Note.Initialize(m_NoteSize, eventSettings);
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
					m_Note.Add(startTime, timing[i]);
					++processTimingCount;
				}
			}

			m_Note.Update(m_DestinationTime);
		}

		private void UpdateLine()
		{
			m_Rhythm.Update(m_DestinationTime);

			var endPoint = m_Direction * m_Length + m_StartPoint;
			var timingPoint = Vector3.Lerp(m_StartPoint, endPoint, m_TimingLineRate);
			DrawLine(m_StartPoint, Vector3.right, m_LineWidth, Color.yellow);
			DrawLine(endPoint, Vector3.right, m_LineWidth, Color.yellow);
			DrawLine(timingPoint, Vector3.right, m_LineWidth, Color.red);
		}

		private void DrawRhythmLine(Beat beat)
		{
			if (!beat.IsActive || beat.Progress <= 0.0f || beat.Progress >= 1.0f)
			{
				return;
			}
			var endPoint = m_Direction * m_Length + m_StartPoint;
			var point = Vector3.Lerp(m_StartPoint, endPoint, beat.Progress);
			DrawLine(point, Vector3.right, m_LineWidth, Color.green);
		}

		private void DrawNote(Beat beat)
		{
			if (!beat.IsActive || beat.Progress <= 0.0f || beat.Progress >= 1.0f)
			{
				return;
			}
			var endPoint = m_Direction * m_Length + m_StartPoint;
			var point = Vector3.Lerp(m_StartPoint, endPoint, beat.Progress);
			DrawCircle(point, m_LineWidth * 0.3f, (m_JudgeNote == beat) ? Color.red : Color.cyan);

			// 前後のラインを測るには内部の式出さないといかんなーとか思っちゃうところ。
			var earlyPerfect = beat.CalculateProgress(Metronome.instance.Time - m_PerfectDelta, m_DestinationTime);
			var latePerfect = beat.CalculateProgress(Metronome.instance.Time + m_PerfectDelta, m_DestinationTime);
			var earlyGreat = beat.CalculateProgress(Metronome.instance.Time - m_GreatDelta, m_DestinationTime);
			var lateGreat = beat.CalculateProgress(Metronome.instance.Time + m_GreatDelta, m_DestinationTime);
			var earlyGood = beat.CalculateProgress(Metronome.instance.Time - m_GoodDelta, m_DestinationTime);
			var lateGood = beat.CalculateProgress(Metronome.instance.Time + m_GoodDelta, m_DestinationTime);

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
		private int CalculateDestinationTime(int arriveTimeAtTimingLine)
		{
			Debug.Assert(!m_TimingLineRate.IsZero());
			return (int)(arriveTimeAtTimingLine / m_TimingLineRate);
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
			var time = Metronome.instance.CalculateBeatTime(eventVariant.intValue);
			m_Rhythm.Add(Metronome.instance.Time, time);
		}

		private void OnUpdateNote(Beat beat)
		{
			UpdateJudgeNote(beat);
			DrawNote(beat);
		}

		private void UpdateJudgeNote(Beat beat)
		{
			if (!beat.IsActive || beat.Progress <= 0.0f || beat.Progress >= 1.0f)
			{
				if (m_JudgeNote == beat)
				{
					m_JudgeNote = null;
				}
				return;
			}

			if (m_JudgeNote == null)
			{
				m_JudgeNote = beat;
			}
			else
			{
#if false
				var toJudgeNote = Mathf.Abs(m_JudgeNote.Progress - m_TimingLineRate);
				var toCurrentNote = Mathf.Abs(beat.Progress - m_TimingLineRate);
				if (toJudgeNote >= toCurrentNote)
				{
					m_JudgeNote = beat;
				}
#else
				// 最遅判定時間をオーバーせず先頭なら
				if (beat.TargetTime + m_GoodDelta < Metronome.instance.Time
					&& beat.TargetTime < m_JudgeNote.TargetTime)
				{
					m_JudgeNote = beat;
				}
#endif
			}
		}



		private void MightJudge()
		{
			if (!m_IsInput || m_JudgeNote == null)
			{
				return;
			}
			var delta = Mathf.Abs(m_JudgeNote.TargetTime - (Metronome.instance.Time - m_StartPlayTime));
			if (delta > m_IgnoreJudgeDelta)
			{
				m_IsInput = false;
				return;
			}

			if (delta <= m_PerfectDelta)
			{
				Debug.Log("Perfect!");
			}
			else if (delta <= m_GreatDelta)
			{
				Debug.Log("Great!");
			}
			else if (delta <= m_GoodDelta)
			{
				Debug.Log("Good!");
			}
			else
			{
				Debug.Log("Miss...");
			}
			m_JudgeNote.Stop();
			m_IsInput = false;
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

		[SerializeField, Range(1, 999)]
		private int m_Bpm = 120;

		public int Bpm => m_Bpm;
		public int Speed => (int)(m_Speed * Metronome.SpeedScale);

		[SerializeField, Uneditable]
		private int m_DestinationTime = 0;


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
		private int m_IgnoreJudgeDelta = 500;

		private bool m_IsInput = false;

		private AudioHandler m_MusicHandle = null;

		// well-formed parameters
		[SerializeField]
		private int m_RhythmSize = 200;
		[SerializeField]
		private int m_NoteSize = 100;

		// well-formed objects
		private BeatManager m_Rhythm = new BeatManager();
		private BeatManager m_Note = new BeatManager();

		private Beat m_JudgeNote = null;
		

	}
}