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
			Initialize();
        }

        void Update()
        {
			UpdateInterval();
		}

		//-----------------------------------
		// Method (public)
		//-----------------------------------
		public void Initialize()
		{
			StartTime = TimeManager.instance.GameTimeMs;
			Count = -1;
		}
		public int CalculateInterval()
		{
			Debug.Assert(Bpm != 0);

			var interval = Minite / Bpm;
			m_Interval = interval;
			return interval;
		}
		public int CalculateBeatTime(int count)
		{
			return CalculateInterval() * count;
		}

		public void SetBPM(int bpm)
		{
			m_Bpm = bpm;
		}
		public void SetSpeed(int speed)
		{
			m_Speed = speed;
		}
		public void SetSpeed(float speed)
		{
			m_Speed = (int)(speed * SpeedScale);
		}

		//-----------------------------------
		// Method (private)
		//-----------------------------------

		private void UpdateInterval()
		{
			var interval = CalculateInterval();
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
		public int Bpm => m_Bpm;
		public int Speed => m_Speed;
		public int Time => (TimeManager.instance.GameTimeMs - StartTime) * Speed / SpeedScale;
		public int StartTime { private set; get; } = 0;
		public int Count { private set; get; } = -1;

		//-----------------------------------
		// Define
		//-----------------------------------
		public const int SpeedScale = 100; // = 1.00[x]
		public const int TimeScale = 1000;
		public const int Minite = 60 * TimeScale; // = 60.000[s]

		//-----------------------------------
		// Field
		//-----------------------------------
		[SerializeField, Uneditable]
		private float m_Interval = 0.0f;

		private int m_Bpm = 60;
		private int m_Speed = SpeedScale;

        //-----------------------------------
        // Internal Class / Struct
        //-----------------------------------
    }
} // N1D.App