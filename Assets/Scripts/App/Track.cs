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
using UnityEngine;
using N1D.Framework.Core;
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
			GameEventManager.instance.Add(gameObject);
			Metronome.instance.Initialize();
			// TODO:トラックデータにBPMも含まれると思うけどどう反映させるか…（調整とか面倒になる的な意味で）
			//Metronome.instance.SetBPM(m_TrackSheet.Bpm);

			InitializeEvents();
			InitializeTempoLine();
        }

        void Update()
        {
			ManualUpdate();
        }

		//-----------------------------------
		// Method (public)
		//-----------------------------------
		public void Initialize(TrackSheet sheet)
		{
			m_Sheet = sheet;
		}

		public void ManualUpdate()
		{
			Metronome.instance.SetBPM(GameConfig.instance.Bpm);
			Metronome.instance.SetSpeed(GameConfig.instance.Speed);

			MightCreateTempoLines();
			m_TempoLineBeat.Update();
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
					Debug.LogFormat("start:{0}, end:{1}", startTime, endTime);
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
		[SerializeField]
		private TempoLine m_TempoLinePrefab = null;

		[SerializeField]
		private int m_RhythmBufferSize = 200;
		[SerializeField]
		private int m_NoteBufferSize = 100;

		private TrackSheet m_Sheet = null;

		private Dictionary<GameEventId, Action<GameEventVariant>> m_EventDictionary 
			= new Dictionary<GameEventId, Action<GameEventVariant>>();

		// tempoline
		private int m_BeatCount = 0;
		private GameObjectPool<TempoLine> m_TempoLinePool = null;
		private BeatManager m_TempoLineBeat = null;
		private Dictionary<Beat, TempoLine> m_TempoLines = new Dictionary<Beat, TempoLine>();

		//-----------------------------------
		// Internal Class / Struct
		//-----------------------------------
	}
} // N1D.App