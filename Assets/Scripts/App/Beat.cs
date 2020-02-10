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
		public void Start(int targetTime)
		{
			TargetTime = targetTime;
			Progress = 0.0f;
			IsActive = true;
		}
		public void Update()
		{
			if (!IsActive)
			{
				return;
			}

			Progress = GetProgress();
		}

		public void Stop()
		{
			Progress = 1.0f;
			IsActive = false;
		}

		public float GetProgress(int delta = 0)
		{
			// TargetTime = タイミング時間の場合
			var endTime = CalculateActiveEndTime(TargetTime + delta);
			var startTime = endTime - CalculateActiveTime();

			return (float)(Metronome.instance.Time - startTime) / (float)(endTime - startTime);
		}



		//-----------------------------------
		// Method (private)
		//-----------------------------------

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

		//-----------------------------------
		// Internal Class / Struct
		//-----------------------------------
	}
} // N1D.App