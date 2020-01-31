//-------------------------------------------
//
//  ビート
//
//-------------------------------------------
using UnityEngine;

namespace N1D.App
{
    public class Beat
    {
		//-----------------------------------
		// Method (public)
		//-----------------------------------
		public void Start(int startTime, int targetTime)
		{
			m_StartTime = startTime;
			TargetTime = targetTime;
			Progress = 0.0f;
			IsActive = true;
		}
		public void Update(int destinationTime)
		{
			if (!IsActive)
			{
				return;
			}

			Progress = CalculateProgress(Metronome.instance.Time, destinationTime);

			if (Progress >= 1.0f)
			{
				Stop();
			}
		}
		public void Stop()
		{
			Progress = 1.0f;
			IsActive = false;
		}

		public float CalculateProgress(int time, int destinationTime)
		{
			var delta = time - m_StartTime;
			if (delta >= destinationTime)
			{
				return 1.0f;
			}
			return (float)delta % (float)destinationTime / (float)destinationTime;
		}

		//-----------------------------------
		// Method (private)
		//-----------------------------------

		//-----------------------------------
		// Property
		//-----------------------------------
		public bool IsActive { private set; get; } = false;
		public float Progress { private set; get; } = 0.0f;
		public int TargetTime { private set; get; } = 0;

		//-----------------------------------
		// Define
		//-----------------------------------

		//-----------------------------------
		// Field
		//-----------------------------------
		private int m_StartTime = 0;

		//-----------------------------------
		// Internal Class / Struct
		//-----------------------------------
	}
} // N1D.App