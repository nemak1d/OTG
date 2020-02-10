//-------------------------------------------
//
//  GameConfig
//
//-------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using N1D.Framework.Core;

namespace N1D.App
{
    public class GameConfig : Singleton<GameConfig>
	{
		//-----------------------------------
		// MonoBehaviour
		//-----------------------------------
		void Awake()
		{
			
		}
		void Update()
		{
			Application.targetFrameRate = m_Fps;
		}

		//-----------------------------------
		// Method (private)
		//-----------------------------------

		//-----------------------------------
		// Property
		//-----------------------------------
		public float TimingLineRate => m_TimingLineRate;
		public int StartPlayTime => m_StartPlayTime;
		public int Speed => (int)(m_Speed * Metronome.SpeedScale);
		public int TimingTime => m_TimingTime;
		public int Bpm => m_Bpm;
		public int PerfectDelta => m_PerfectDelta;
		public int GreatDelta => m_GreatDelta;
		public int GoodDelta => m_GoodDelta;
		public int IgnoreJudgeDelta => m_IgnoreJudgeDelta;

		//-----------------------------------
		// Define
		//-----------------------------------

		//-----------------------------------
		// Field
		//-----------------------------------
		[SerializeField]
		private int m_Fps = 120;
		[SerializeField, Tooltip("レーン上の入力タイミング位置割合")]
		private float m_TimingLineRate = 0.7f;

		[SerializeField, Tooltip("ゲーム開始から曲再生までの時間")]
		private int m_StartPlayTime = 3000;
		[SerializeField, Tooltip("再生速度")]
		private float m_Speed = 1.0f;
		[SerializeField, Tooltip("表示してから入力タイミングに到達するまでの時間")]
		private int m_TimingTime = 5000;

		[SerializeField, Range(10, 999), Tooltip("BPM")]
		private int m_Bpm = 120;

		[SerializeField]
		private int m_PerfectDelta = 33;
		[SerializeField]
		private int m_GreatDelta = 99;
		[SerializeField]
		private int m_GoodDelta = 264;
		[SerializeField]
		private int m_IgnoreJudgeDelta = 500;

		//-----------------------------------
		// Internal Class / Struct
		//-----------------------------------
	}
} // N1D.App