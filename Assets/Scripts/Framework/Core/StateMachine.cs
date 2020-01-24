using System;
using System.Collections.Generic;

namespace N1D.Framework.Core
{
	public class StateMachine<T>
	{
		public void Initialize()
		{
			m_States.Clear();
			m_Current = default(T);
			m_CurrentState = null;
		}

		public void Add(T key, Action onEnter, Action onUpdate, Action onExit)
		{
			var state = new State();
			state.onEnter = onEnter;
			state.onUpdate = onUpdate;
			state.onExit = onExit;
			m_States.Add(key, state);
		}

		public void Change(T state)
		{
			if (m_CurrentState != null)
			{
				m_CurrentState.onExit();
			}

			m_Current = state;
			m_CurrentState = m_States[m_Current];

			m_CurrentState.onEnter();
		}

		public void Update()
		{
			if (m_CurrentState == null)
			{
				return;
			}

			m_CurrentState.onUpdate();
		}

		class State
		{
			public Action onEnter = null;
			public Action onUpdate = null;
			public Action onExit = null;
		}

		public T current { get { return m_Current; } }

		T m_Current = default(T);
		State m_CurrentState = null;
		Dictionary<T, State> m_States = new Dictionary<T, State>();

	}
} // N1D.Framework.Core