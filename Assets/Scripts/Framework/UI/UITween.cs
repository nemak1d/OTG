#define ENABLE_EVENT_LOG

using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


namespace N1D.UI
{

	public delegate void OnCompleteDelegate();
	public delegate void OnPhaseCompleteDelegate(string keyword);

	public class UITween : MonoBehaviour
	{
		private void Awake()
		{
			if (m_Target == null)
			{
				m_Target = gameObject;
			}

#if ENABLE_EVENT_LOG
			OnPhaseComplete += (keyword) =>
			{
				Debug.Log("end keyword -> " + keyword + " from -> " + m_Target.name);
			};
			OnComplete += () =>
			{
				Debug.Log("On complete -> " + m_Target.name);
			};
#endif
		}
		private void Start()
		{
			if (m_StartOnPlay)
			{
				Play();
			}
		}

		void Update()
		{

			if (Input.GetKeyDown(KeyCode.P))
			{
				Play();
			}
		}

		void OnDestroy()
		{
			if (m_Sequence != null)
			{
				m_Sequence.Kill();
				m_Sequence = null;
			}
		}

		public void Ready()
		{
			m_Target.transform.localPosition = m_FirstLocalPosition;
			m_Target.transform.localScale = m_FirstLocalScale;
		}
		public void Play()
		{
			Ready();

			if (m_Sequence != null)
			{
				m_Sequence.Kill();
			}

			if (m_IsLoop && m_LoopHeadPhase > 0)
			{
				m_Sequence = BuildSequence(-1, m_LoopHeadPhase);
				m_Sequence.Play().OnComplete(() =>
				{

#if ENABLE_EVENT_LOG
					Debug.Log("Start Loop -> " + m_Target.name);
#endif
					m_Sequence = BuildSequence(m_LoopHeadPhase, -1);
					m_Sequence.Play().OnComplete(() => OnComplete()).SetLoops(-1);
				});
			}
			else
			{
				m_Sequence = BuildSequence();
				m_Sequence.Play().OnComplete(() => OnComplete());
				if (m_IsLoop)
				{
					m_Sequence.SetLoops(-1);
				}
			}
		}
		public void Stop()
		{
			if (m_Sequence != null)
			{
				m_Sequence.Kill();
				m_Sequence = null;
			}
		}

		public bool IsPlaying()
		{
			if (m_Sequence != null)
			{
				m_Sequence.IsPlaying();
			}
			return false;
		}

		private DG.Tweening.Sequence BuildSequence(int phaseStartIndex = -1, int phaseEndIndex = -1)
		{
			if (phaseStartIndex < 0 || phaseStartIndex > m_Phases.Length)
			{
				phaseStartIndex = 0;
			}
			if (phaseEndIndex < 0 || phaseEndIndex > m_Phases.Length)
			{
				phaseEndIndex = m_Phases.Length;
			}

			var sequence = DOTween.Sequence();
			for (var phaseIndex = phaseStartIndex; phaseIndex < phaseEndIndex; ++phaseIndex)
			{
				var phase = m_Phases[phaseIndex];
				if (phase.variants.Length <= 0)
				{
					continue;
				}
				var variantIndex = 0;
				do
				{
					var command = SelectTween(phase.variants[variantIndex], phase.duration)
						.SetEase(phase.variants[variantIndex].ease);

					if (phase.variants[variantIndex].relative)
					{
						command.SetRelative();
					}

					if (variantIndex == 0)
					{
						sequence.Append(command);
					}
					else
					{
						sequence.Join(command);
					}

				} while (phase.variants.Length > ++variantIndex);

				if (!string.IsNullOrEmpty(phase.onCompleteKeyword))
				{
					// Appendの後に追加すればいいと思ったけどそうでもないらしく...
					// フェーズ終わりで追加することにしてみる.
					sequence.AppendCallback(() => OnPhaseComplete(phase.onCompleteKeyword));
				}

			}
			return sequence;
		}


		private Tween SelectTween(TweenVariant variant, float duration)
		{
			switch (variant.function)
			{
				case TweenFunction.Scale:
				{
					return m_Target.transform.DOScale(variant.value, duration);
				}

				case TweenFunction.LocalMove:
				{
					return m_Target.transform.DOLocalMove(variant.value, duration);
				}

				case TweenFunction.Move:
				{
					return m_Target.transform.DOMove(variant.value, duration);
				}

				case TweenFunction.LocalRotate:
				{
					return m_Target.transform.DOLocalRotate(variant.value, duration);
				}

				case TweenFunction.Punch:
				{
					return m_Target.transform.DOPunchScale(variant.value, duration);
				}
				default:
					break;
			}
			return null;
		}


		[Serializable]
		public enum TweenFunction
		{
			Scale,
			LocalMove,
			Move,
			LocalRotate,
			Punch,
		}

		[Serializable]
		public struct TweenPhase
		{
			public TweenVariant[] variants;
			public float duration;
			public string onCompleteKeyword;
		}

		[Serializable]
		public struct TweenVariant
		{
			public TweenFunction function;
			public bool relative;
			public Vector3 value;
			public float arg0;
			public float arg1;
			public Ease ease;
		}

		public Vector3 FirstLocalPosition
		{
			get { return m_FirstLocalPosition; }
			set { m_FirstLocalPosition = value; }
		}
		public Vector3 FirstLocalScale
		{
			get { return m_FirstLocalScale; }
			set { m_FirstLocalScale = value; }
		}


		public OnCompleteDelegate OnComplete = null;
		public OnPhaseCompleteDelegate OnPhaseComplete = null;

		[SerializeField]
		private GameObject m_Target = null;

		[SerializeField]
		private Vector3 m_FirstLocalPosition = Vector3.zero;
		[SerializeField]
		private Vector3 m_FirstLocalScale = Vector3.one;

		[SerializeField]
		private TweenPhase[] m_Phases = null;
		[SerializeField]
		private bool m_StartOnPlay = false;
		[SerializeField]
		private bool m_IsLoop = false;
		[SerializeField]
		private int m_LoopHeadPhase = -1;

		private DG.Tweening.Sequence m_Sequence = null;
	}
} // N1D.UI