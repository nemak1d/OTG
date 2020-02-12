//-------------------------------------------
//
//  Score
//
//-------------------------------------------
using TMPro;
using UnityEngine;
using N1D.Framework.Core;

namespace N1D.App.UI
{
    public class Score : MonoBehaviour
    {
        //-----------------------------------
        // MonoBehaviour
        //-----------------------------------
        void Start()
        {
			m_Text = GetComponent<TextMeshProUGUI>();
			
			Debug.Assert(m_Text != null);
        }

		void Update()
		{
			if (m_Text == null || m_Timer.IsTimeOut())
			{
				return;
			}
			m_Timer.Update();

			m_CurrentScore = (int)Mathf.Lerp(m_FromScore, m_ToScore, m_Timer.GetRateToOne());
			m_Text.SetText(m_CurrentScore.ToString(m_ScoreFormat));
		}

		//-----------------------------------
		// Method (private)
		//-----------------------------------
		public void SetScore(int score)
		{
			m_Timer.Start(m_CountTime);
			m_FromScore = m_CurrentScore;
			m_ToScore = score;
		}
		public void SetTime(float time)
		{
			m_CountTime = time;
		}

		//-----------------------------------
		// Property
		//-----------------------------------

		//-----------------------------------
		// Define
		//-----------------------------------

		//-----------------------------------
		// Field
		//-----------------------------------
		[SerializeField]
		private string m_ScoreFormat = "#,0000000";

		private TextMeshProUGUI m_Text = null;
		private Timer m_Timer = new Timer();
		private float m_CountTime = 1.0f;
		private int m_FromScore = 0;
		private int m_ToScore = 0;
		private int m_CurrentScore = 0;

        //-----------------------------------
        // Internal Class / Struct
        //-----------------------------------
    }
} // N1D.App.UI