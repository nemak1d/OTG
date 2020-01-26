//-------------------------------------------
//
//  TimeManager
//
//-------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using N1D.Framework.Core;

namespace N1D.App
{
	[DefaultExecutionOrder(-100)]
    public class TimeManager : Singleton<TimeManager>
    {
        //-----------------------------------
        // MonoBehaviour
        //-----------------------------------
        void Awake()
        {
            
        }

        void Update()
        {
            
        }

		//-----------------------------------
		// Method (private)
		//-----------------------------------

		//-----------------------------------
		// Property
		//-----------------------------------
		public float GameTime => Time.time;
		public float RealTime => Time.realtimeSinceStartup;
		public float DeltaTime => Time.deltaTime;

		public int GameTimeMs => (int)(GameTime * MilliSecondScale);
		public int RealTimeMs => (int)(RealTime * MilliSecondScale);
		

		//-----------------------------------
		// Define
		//-----------------------------------
		private const float MilliSecondScale = 1000.0f;

        //-----------------------------------
        // Field
        //-----------------------------------

        //-----------------------------------
        // Internal Class / Struct
        //-----------------------------------
    }
} // N1D.App