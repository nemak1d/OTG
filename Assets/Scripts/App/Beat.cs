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
		public void Start(float startTime)
		{
			m_StartTime = startTime;
			Progress = 0.0f;
			IsActive = true;
		}
		public void Update(float destinationTime)
		{
			if (!IsActive)
			{
				return;
			}

			var delta = Time.realtimeSinceStartup - m_StartTime;
			Progress = delta % destinationTime / destinationTime;

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
		private float m_StartTime = 0.0f;

		//-----------------------------------
		// Internal Class / Struct
		//-----------------------------------
	}
} // N1D.App