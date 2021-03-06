﻿//-------------------------------------------
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
		public float PerfectScoreScale => m_PerfectScoreScale;
		public float GreatScoreScale => m_GreatScoreScale;
		public float GoodScoreScale => m_GoodScoreScale;

		//-----------------------------------
		// Define
		//-----------------------------------

		//-----------------------------------
		// Field
		//-----------------------------------
		[SerializeField, Range(30, 300)]
		private int m_Fps = 120;
		[SerializeField, Range(0.01f, 0.99f), Tooltip("レーン上の入力タイミング位置割合")]
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


		// score
		// 1,000,000を上限としてnoteCount / 1,000,000で算出する
		[SerializeField, Range(0.0f, 1.0f)]
		private float m_PerfectScoreScale = 1.0f;
		[SerializeField, Range(0.0f, 1.0f)]
		private float m_GreatScoreScale = 0.8f;
		[SerializeField, Range(0.0f, 1.0f)]
		private float m_GoodScoreScale = 0.5f;


		//-----------------------------------
		// Internal Class / Struct
		//-----------------------------------
	}
} // N1D.App