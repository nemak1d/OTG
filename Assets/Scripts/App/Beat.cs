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
		public void Start(int startTime)
		{
			m_StartTime = startTime;
			Progress = 0.0f;
			IsActive = true;
		}
		public void Update(int destinationTime)
		{
			if (!IsActive)
			{
				return;
			}

			var delta = TimeManager.instance.RealTimeMs - m_StartTime;
			Progress = (float)delta % (float)destinationTime / (float)destinationTime;

			if (delta >= destinationTime)
			{
				IsActive = false;
				Progress = 1.0f;
			}
		}

		//-----------------------------------
		// Method (private)
		//-----------------------------------

		//-----------------------------------
		// Property
		//-----------------------------------
		public bool IsActive { private set; get; } = false;
		public float Progress { private set; get; } = 0.0f;

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