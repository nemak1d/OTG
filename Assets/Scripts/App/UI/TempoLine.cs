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
		// Method (public)
		//-----------------------------------
		public void SetTime(int time)
		{
			if (Text != null)
			{
				Text.text = time.ToString();
			}
		}

		//-----------------------------------
		// Property
		//-----------------------------------
		private TextMeshProUGUI Text
		{
			get
			{
				if (m_Text == null)
				{
					m_Text = GetComponentInChildren<TextMeshProUGUI>(true);
				}
				return m_Text;
			}
		}

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