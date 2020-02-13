//-------------------------------------------
//
//  Evaluation
//
//-------------------------------------------
using UnityEngine;
using TMPro;
using N1D.Framework.Core;

namespace N1D.App.UI
{
    public class Evaluation : MonoBehaviour
    {
        //-----------------------------------
        // MonoBehaviour
        //-----------------------------------
        void Update()
        {
            if (!m_Timer.IsTimeOut())
			{
				Stop();
			}
        }

        //-----------------------------------
        // Method (public)
        //-----------------------------------
		public void Play(EvaluationGrade grade, int combo)
		{
			m_Timer.Start(m_ShowTime);

			if (m_GradeText != null)
			{
				m_GradeText.text = GetGradeMessage(grade);
				m_GradeText.gameObject.SetActive(true);
			}
			if (m_ComboText != null)
			{
				m_ComboText.text = combo.ToString();
				m_ComboText.gameObject.SetActive(true);
			}
		}
		public void Stop()
		{
			m_Timer.Stop();

			if (m_GradeText != null)
			{
				m_GradeText.gameObject.SetActive(false);
			}
			if (m_ComboText != null)
			{
				m_ComboText.gameObject.SetActive(false);
			}
		}

		public void SetShowTime(float time)
		{
			m_ShowTime = time;
		}
		//-----------------------------------
		// Method (private)
		//-----------------------------------
		private string GetGradeMessage(EvaluationGrade grade)
		{
			switch (grade)
			{
				case EvaluationGrade.Perfect:
					return m_PerfectMessage;

				case EvaluationGrade.Great:
					return m_GreatMessage;

				case EvaluationGrade.Good:
					return m_GoodMessage;

				default:
					break;
			}
			return m_MissMessage;
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
		private string m_PerfectMessage = "PERFECT!!";
		[SerializeField]
		private string m_GreatMessage = "GREAT!";
		[SerializeField]
		private string m_GoodMessage = "GOOD";
		[SerializeField]
		private string m_MissMessage = "MISS...";

		[SerializeField]
		private TextMeshProUGUI m_GradeText = null;
		[SerializeField]
		private TextMeshProUGUI m_ComboText = null;

		private Timer m_Timer = new Timer();
		private float m_ShowTime = 1.0f;

        //-----------------------------------
        // Internal Class / Struct
        //-----------------------------------
    }
} // N1D.App.UI