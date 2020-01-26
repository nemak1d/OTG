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
			StartTime = Time.realtimeSinceStartup;
			Count = 0;
		}
		public float CalculateInterval()
		{
			Debug.Assert(!Bpm.IsZero());

			return 60.0f / Bpm;
		}
		public float CalculateBeatTime(int count)
		{
			return CalculateInterval() * count + StartTime;
		}

		//-----------------------------------
		// Method (private)
		//-----------------------------------

		private void UpdateInterval()
		{
			var interval = CalculateInterval();
			var deltaTime = (Time.realtimeSinceStartup - StartTime);
			var count = (int)(deltaTime / interval);
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
		public float StartTime { private set; get; } = 0.0f;
		public int Count { private set; get; } = 0;
		public float Bpm
		{
			private set { m_Bpm = value; }
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