using N1D.Framework.Core;
using N1D.Framework.Dbg;
using N1D.Framework.Sound;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace N1D.App
{
	public class TimingLine : MonoBehaviour, IGameEventReceivable
	{
		IEnumerator Start()
		{
			this.enabled = false;
			Addressables.LoadAssetAsync<TrackSheet>("Data/TrackSheet01.asset")
				.Completed += (h) =>
				{
					m_TrackSheet = h.Result;
				};
		
			while (m_TrackSheet == null)
			{
				yield return null;
			}

			GameEventManager.instance.Add(gameObject);
			Metronome.instance.Initialize();
			// TODO:トラックデータにBPMも含まれると思うけどどう反映させるか…（調整とか面倒になる的な意味で）
			//Metronome.instance.SetBPM(m_TrackSheet.Bpm);

			InitializeRhythm();
			InitializeNote();
			this.enabled = true;
		}

		void Update()
		{
			Metronome.instance.SetBPM(GameConfig.instance.Bpm);

			var currentSpeed = Metronome.instance.Speed;
			Metronome.instance.SetSpeed(GameConfig.instance.Speed);
			if (currentSpeed != Metronome.instance.Speed)
			{
				OnChangeSpeed();
			}

			m_DestinationTime = CalculateDestinationTime(GameConfig.instance.TimingTime);

			UpdateInput();
			UpdateMusic();

			MightCreateBeats();
			UpdateLine();
			UpdateTrackSheet();

			MightJudge();
		}

		private void InitializeRhythm()
		{
			var eventSettings = new BeatEvent();
			eventSettings.onUpdate = DrawRhythmLine;

			m_Rhythm.Initialize(m_RhythmBufferSize, eventSettings);
		}

		private void InitializeNote()
		{
			var eventSettings = new BeatEvent();
			eventSettings.onUpdate = OnUpdateNote;

			m_Note.Initialize(m_NoteBufferSize, eventSettings);
		}

		private void UpdateInput()
		{
			if (Input.anyKeyDown)
			{
				m_IsInput = true;
				Debug.LogWarning("Input:" + Metronome.instance.Time);
			}
		}
		private void UpdateMusic()
		{
			if (GameConfig.instance.StartPlayTime <= Metronome.instance.Time)
			{
				if (m_MusicHandle == null)
				{
					m_MusicHandle = SoundManager.instance.PlayBGM("popepope");
				}
			}
		}

		private void UpdateTrackSheet()
		{
			if (m_TrackSheet == null || m_TrackSheet.Settings.Length <= 0)
			{
				return;
			}

			for (var i = processTimingCount; i < m_TrackSheet.Settings.Length; ++i)
			{
				// 表示開始は曲の時間 - 表示してから実際に入力するまでの時間 + 曲再生開始時間
				var endTime = CalculateActiveEndTime(m_TrackSheet.Settings[i].time);
				var startTime = endTime - CalculateActiveTime();
				if (startTime <= Metronome.instance.Time)
				{
					m_Note.Add(m_TrackSheet.Settings[i].time);
					++processTimingCount;
				}
			}

			m_Note.Update();
		}

		private void UpdateLine()
		{
			m_Rhythm.Update();

			var endPoint = m_Direction * m_Length + m_StartPoint;
			var timingPoint = Vector3.Lerp(m_StartPoint, endPoint, GameConfig.instance.TimingLineRate);
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

			var earlyPerfect = beat.GetProgress(-GameConfig.instance.PerfectDelta);
			var latePerfect = beat.GetProgress(GameConfig.instance.PerfectDelta);
			var earlyGreat = beat.GetProgress(-GameConfig.instance.GreatDelta);
			var lateGreat = beat.GetProgress(GameConfig.instance.GreatDelta);
			var earlyGood = beat.GetProgress(-GameConfig.instance.GoodDelta);
			var lateGood = beat.GetProgress(GameConfig.instance.GoodDelta);

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
			Debug.Assert(!GameConfig.instance.TimingLineRate.IsZero());
			return (int)((60 * arriveTimeAtTimingLine) / (GameConfig.instance.TimingLineRate * Metronome.instance.Bpm));
		}

		public void OnReceiveGameEvent(GameEventVariant eventVariant)
		{
			switch (eventVariant.id)
			{
				case GameEventId.Beat:
				{
					//OnBeat(eventVariant);
					break;
				}
			}
		}

		public void MightCreateBeats()
		{
			// 生成しようとしたビートの開始時間が現在時刻を下回ってる限り生成し続ける
			while (true)
			{
				var time = Metronome.instance.CalculateBeatTime(m_BeatCount);
				time = 1000 * m_BeatCount;
				var endTime = CalculateActiveEndTime(time);
				var startTime = endTime - CalculateActiveTime();
				if (startTime <= Metronome.instance.Time)
				{
					Debug.LogFormat("start:{0}, end:{1}", startTime, endTime);
					m_Rhythm.Add(time);
					++m_BeatCount;
				}
				else
				{
					break;
				}
			}
		}

		// ノートの有効時間
		private int CalculateActiveTime()
		{
			return (int)(CalculateScaledTimingTime() / GameConfig.instance.TimingLineRate);
		}
		private int CalculateActiveAfterTime()
		{
			return (int)(CalculateActiveTime() * (1.0f - GameConfig.instance.TimingLineRate));
		}
		private int CalculateActiveEndTime(int targetTime)
		{
			return targetTime + CalculateActiveAfterTime();
		}
		private int CalculateScaledTimingTime()
		{
			return GameConfig.instance.TimingTime * 60 / Metronome.instance.Bpm;
		}

		private void OnChangeSpeed()
		{

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
				if (beat.TargetTime + GameConfig.instance.GoodDelta < Metronome.instance.Time
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
			var delta = Mathf.Abs(m_JudgeNote.TargetTime - Metronome.instance.Time);

			if (delta <= GameConfig.instance.PerfectDelta)
			{
				Debug.Log("Perfect!");
			}
			else if (delta <= GameConfig.instance.GreatDelta)
			{
				Debug.Log("Great!");
			}
			else if (delta <= GameConfig.instance.GoodDelta)
			{
				Debug.Log("Good!");
			}
			else if (delta <= GameConfig.instance.IgnoreJudgeDelta)
			{
				m_IsInput = false;
				return;
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

		// visual
		public float m_LineWidth = 3.0f;

		[SerializeField, Uneditable]
		private int m_DestinationTime = 0;


		[SerializeField]
		private int[] timing = null;
		private int processTimingCount = 0;

		private bool m_IsInput = false;

		private AudioHandler m_MusicHandle = null;

		// well-formed parameters
		[SerializeField]
		private int m_RhythmBufferSize = 200;
		[SerializeField]
		private int m_NoteBufferSize = 100;

		// well-formed objects
		private BeatManager m_Rhythm = new BeatManager();
		private BeatManager m_Note = new BeatManager();
		private TrackSheet m_TrackSheet = null;

		private Beat m_JudgeNote = null;
		private int m_BeatCount = 0;
		

	}
}