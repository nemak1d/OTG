//-------------------------------------------
//
//  Track
//	
//	Laneをまとめた位置づけ.
//
//-------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using N1D.Framework.Core;
using N1D.Framework.Sound;
using N1D.App.UI;

namespace N1D.App
{
    public class Track : MonoBehaviour, IGameEventReceivable
    {
        //-----------------------------------
        // MonoBehaviour
        //-----------------------------------
        void Start()
        {
			Boot boot;
			boot.sheet = m_Sheet;
			Initialize(boot);
        }

        void Update()
        {
			ManualUpdate();
        }

		//-----------------------------------
		// Method (public)
		//-----------------------------------
		public void Initialize(Boot boot)
		{
			m_Sheet = boot.sheet;
			GameEventManager.instance.Add(gameObject);
			Metronome.instance.Initialize();
			// TODO:トラックデータにBPMも含まれると思うけどどう反映させるか…（調整とか面倒になる的な意味で）
			//Metronome.instance.SetBPM(m_TrackSheet.Bpm);

			InitializeEvents();
			InitializeTempoLine();
			InitializeNote();
			InitializeTimingLine();
		}

		public void ManualUpdate()
		{
			Metronome.instance.SetBPM(GameConfig.instance.Bpm);
			Metronome.instance.SetSpeed(GameConfig.instance.Speed);

			UpdateMusic();
			UpdateTimingLine();
			MightCreateTempoLines();
			m_TempoLineBeat.Update();
			MightCreateNotes();
			m_NoteBeat.Update();
		}

		public void OnReceiveGameEvent(GameEventVariant eventVariant)
		{
			Action<GameEventVariant> onEvent;
			
			if (m_EventDictionary.TryGetValue(eventVariant.id, out onEvent))
			{
				onEvent?.Invoke(eventVariant);
			}
		}
		//-----------------------------------
		// Method (private)
		//-----------------------------------

		// events
		private void InitializeEvents()
		{
			m_EventDictionary.Add(GameEventId.ChangeSpeed, OnChangeSpeed);
			m_EventDictionary.Add(GameEventId.ChangeBpm, OnChangeBpm);
		}

		private void OnChangeSpeed(GameEventVariant eventVariant)
		{

		}
		private void OnChangeBpm(GameEventVariant eventVariant)
		{

		}

		// music
		private void UpdateMusic()
		{
			if (m_MusicHandle != null)
			{
				return;
			}

			if (GameConfig.instance.StartPlayTime <= Metronome.instance.Time)
			{
				// TODO: 事前ロード
				m_MusicHandle = SoundManager.instance.PlayBGM("popepope");
			}
		}

		// timingline
		private void InitializeTimingLine()
		{
			var position = Vector3.Lerp(CalculateStartPoint(), CalculateEndPoint(), GameConfig.instance.TimingLineRate);

			m_TimingLine.transform.localPosition = position;
		}
		[Conditional("DEBUG")]
		private void UpdateTimingLine()
		{
			var position = Vector3.Lerp(CalculateStartPoint(), CalculateEndPoint(), GameConfig.instance.TimingLineRate);

			m_TimingLine.transform.localPosition = position;
		}

		// tempoline
		private void MightCreateTempoLines()
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
					UnityEngine.Debug.LogFormat("start:{0}, end:{1}", startTime, endTime);
					m_TempoLineBeat.Add(time);
					++m_BeatCount;
				}
				else
				{
					break;
				}
			}
		}
		private void InitializeTempoLine()
		{
			if (m_TempoLinePool == null)
			{
				m_TempoLinePool = TempoLinePool.Create(m_RhythmBufferSize, m_TempoLinePrefab);
			}
			if (m_TempoLineBeat == null)
			{
				m_TempoLineBeat = new BeatManager();
				var eventSettings = new BeatEvent();
				eventSettings.onStart = OnStartTempoLine;
				eventSettings.onUpdate = OnUpdateTempoLine;
				eventSettings.onStop = OnStopTempoLine;
				m_TempoLineBeat.Initialize(m_RhythmBufferSize, eventSettings);
			}
		}
		private void OnStartTempoLine(Beat beat)
		{
			var view = m_TempoLinePool.Pull();
			view.transform.SetParent(transform, false);
			view.SetTime(beat.TargetTime);

			m_TempoLines.Add(beat, view);
		}
		private void OnUpdateTempoLine(Beat beat)
		{
			TempoLine view;
			if (m_TempoLines.TryGetValue(beat, out view))
			{
				Vector3 position;
				if (CalculatePosition(beat, out position))
				{
					view.transform.localPosition = position;
					view.gameObject.SetActive(true);
				}
				else
				{
					view.gameObject.SetActive(false);
				}
			}
		}
		private void OnStopTempoLine(Beat beat)
		{
			TempoLine view;
			if (m_TempoLines.TryGetValue(beat, out view))
			{
				m_TempoLinePool.Push(view);
				m_TempoLines.Remove(beat);
			}
		}

		// notes
		private void MightCreateNotes()
		{
			if (m_Sheet == null || m_Sheet.Settings.Length <= 0)
			{
				return;
			}

			for (var i = m_ProcessNoteCount; i < m_Sheet.Settings.Length; ++i)
			{
				// 表示開始は曲の時間 - 表示してから実際に入力するまでの時間 + 曲再生開始時間
				var endTime = CalculateActiveEndTime(m_Sheet.Settings[i].time);
				var startTime = endTime - CalculateActiveTime();
				if (startTime <= Metronome.instance.Time)
				{
					m_NoteBeat.Add(m_Sheet.Settings[i].time);
					++m_ProcessNoteCount;
				}
			}
		}
		private void InitializeNote()
		{
			if (m_NotePool == null)
			{
				m_NotePool = ImagePool.Create(m_NoteBufferSize, m_NotePrefab);
			}
			if (m_NoteBeat == null)
			{
				m_NoteBeat = new BeatManager();
				var eventSettings = new BeatEvent();
				eventSettings.onStart = OnStartNote;
				eventSettings.onUpdate = OnUpdateNote;
				eventSettings.onStop = OnStopNote;
				m_NoteBeat.Initialize(m_NoteBufferSize, eventSettings);
			}
		}
		private void OnStartNote(Beat beat)
		{
			var view = m_NotePool.Pull();
			view.transform.SetParent(transform, false);

			m_Notes.Add(beat, view);
		}
		private void OnUpdateNote(Beat beat)
		{
			Image view;
			if (m_Notes.TryGetValue(beat, out view))
			{
				Vector3 position;
				if (CalculatePosition(beat, out position))
				{
					view.transform.localPosition = position;
					view.gameObject.SetActive(true);
				}
				else
				{
					view.gameObject.SetActive(false);
				}
			}
		}
		private void OnStopNote(Beat beat)
		{
			Image view;
			if (m_Notes.TryGetValue(beat, out view))
			{
				m_NotePool.Push(view);
				m_Notes.Remove(beat);
			}
		}


		// times
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

		// positions
		private bool CalculatePosition(Beat beat, out Vector3 position)
		{
			return beat.CalculateDrawPosition(0, CalculateStartPoint(), CalculateEndPoint(), out position);
		}
		private Vector3 CalculateStartPoint()
		{
			if (m_UseLocator)
			{
				return m_StartLocator.transform.localPosition;
			}
			return m_StartPoint;
		}
		private Vector3 CalculateEndPoint()
		{
			if (m_UseLocator)
			{
				return m_EndLocator.transform.localPosition;
			}
			return m_Direction * m_Length + m_StartPoint;
		}


		//-----------------------------------
		// Property
		//-----------------------------------
		public bool IsInitialized => m_Sheet != null;

		//-----------------------------------
		// Define
		//-----------------------------------

		//-----------------------------------
		// Field
		//-----------------------------------
		[SerializeField]
		private bool m_UseLocator = false;
		[SerializeField]
		private GameObject m_StartLocator = null;
		[SerializeField]
		private GameObject m_EndLocator = null;
		[SerializeField]
		private Vector3 m_StartPoint = Vector3.zero;
		[SerializeField]
		private Vector3 m_Direction = Vector3.down;
		[SerializeField]
		private float m_Length = 5.0f;

		// well-formed parameters

		// resources
		[SerializeField]
		private TempoLine m_TempoLinePrefab = null;
		[SerializeField]
		private Image m_NotePrefab = null;
		[SerializeField]
		private TrackSheet m_Sheet = null;
		[SerializeField]
		private Image m_TimingLine = null;

		[SerializeField]
		private int m_RhythmBufferSize = 200;
		[SerializeField]
		private int m_NoteBufferSize = 100;

		private Dictionary<GameEventId, Action<GameEventVariant>> m_EventDictionary 
			= new Dictionary<GameEventId, Action<GameEventVariant>>();

		// music
		private AudioHandler m_MusicHandle = null;

		// tempoline
		private int m_BeatCount = 0;
		private GameObjectPool<TempoLine> m_TempoLinePool = null;
		private BeatManager m_TempoLineBeat = null;
		private Dictionary<Beat, TempoLine> m_TempoLines = new Dictionary<Beat, TempoLine>();

		// notes
		private int m_ProcessNoteCount = 0;
		private GameObjectPool<Image> m_NotePool = null;
		private BeatManager m_NoteBeat = null;
		private Dictionary<Beat, Image> m_Notes = new Dictionary<Beat, Image>();

		//-----------------------------------
		// Internal Class / Struct
		//-----------------------------------
		public struct Boot
		{
			public TrackSheet sheet;
		}
	}
} // N1D.App