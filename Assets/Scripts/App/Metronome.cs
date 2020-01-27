//-------------------------------------------
//
//  メトロノーム
//
//-------------------------------------------
using UnityEngine;
using N1D.Framework.Core;

namespace N1D.App
{
	[DefaultExecutionOrder(-1)]
    public class Metronome : Singleton<Metronome>
    {
        //-----------------------------------
        // MonoBehaviour
        //-----------------------------------
        void Awake()
        {
			Initialize(m_Bpm);
        }

        void Update()
        {
			UpdateInterval();
		}

		//-----------------------------------
		// Method (public)
		//-----------------------------------
		public void Initialize(float bpm)
		{
			Bpm = bpm;
			StartTime = TimeManager.instance.GameTimeMs;
			Count = 0;
		}
		public int CalculateInterval()
		{
			Debug.Assert(!Bpm.IsZero());

			return (int)(60000.0f / Bpm);
		}
		public int CalculateBeatTime(int count)
		{
			return CalculateInterval() * count;
		}

		//-----------------------------------
		// Method (private)
		//-----------------------------------

		private void UpdateInterval()
		{
			var interval = CalculateInterval();
			Time = TimeManager.instance.GameTimeMs - StartTime;
			var count = Time / interval;
			if (count > Count)
			{
				var deltaCount = count - Count;
				for (var i = 0; i < deltaCount; ++i)
				{
					++Count;
					Debug.Log("count -> " + Count);
					var eventVariant = new GameEventVariant();
					eventVariant.id = GameEventId.Beat;
					eventVariant.intValue = Count;
					GameEventManager.instance.Send<IGameEventReceivable>(
						(target, e) => target.OnReceiveGameEvent(eventVariant));
				}
			}
		}


		//-----------------------------------
		// Property
		//-----------------------------------
		public int Time { private set; get; } = 0;
		public int StartTime { private set; get; } = 0;
		public int Count { private set; get; } = 0;
		public float Bpm
		{
			set { m_Bpm = value; }
			get { return m_Bpm; }
		}

		//-----------------------------------
		// Define
		//-----------------------------------

		//-----------------------------------
		// Field
		//-----------------------------------
		[SerializeField]
		private float m_Bpm = 120.0f;

        //-----------------------------------
        // Internal Class / Struct
        //-----------------------------------
    }
} // N1D.App