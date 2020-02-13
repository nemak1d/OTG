//-------------------------------------------
//
//  TempoLine
//
//-------------------------------------------
using UnityEngine;
using TMPro;

namespace N1D.App.UI
{
    public class TempoLine : MonoBehaviour
    {
        //-----------------------------------
        // MonoBehaviour
        //-----------------------------------
        void Start()
        {
			m_Text = GetComponent<TextMeshProUGUI>();
        }

		//-----------------------------------
		// Method (public)
		//-----------------------------------
		public void SetTime(int time)
		{
			if (m_Text == null)
			{
				m_Text.text = time.ToString();
			}
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
		private TextMeshProUGUI m_Text = null;
        //-----------------------------------
        // Internal Class / Struct
        //-----------------------------------
    }
} // N1D.App.UI