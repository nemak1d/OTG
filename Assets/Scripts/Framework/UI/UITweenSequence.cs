using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace N1D.UI
{

	public class UITweenSequence : MonoBehaviour
	{
		// Start is called before the first frame update
		void Start()
		{
			if (m_Order.Length <= 0)
			{
				return;
			}

			for (var i = 0; i < m_Order.Length - 1; ++i)
			{
				m_Order[i].OnComplete += m_Order[i + 1].Play;
			}
			m_Order[m_Order.Length - 1].OnComplete += OnComplete;

			if (m_Loop)
			{
				OnComplete += m_Order[0].Play;
			}
		}

		// Update is called once per frame
		void Update()
		{
			if (Input.GetKeyDown(KeyCode.S))
			{
				Play();
			}
		}

		public void Play()
		{
			if (m_Order.Length <= 0)
			{
				return;
			}

			foreach (var order in m_Order)
			{
				order.Ready();
			}

			m_Order[0].Play();
		}

		public OnCompleteDelegate OnComplete = null;

		[SerializeField]
		UITween[] m_Order = null;
		[SerializeField]
		bool m_Loop = false;
	}
} // N1D.UI