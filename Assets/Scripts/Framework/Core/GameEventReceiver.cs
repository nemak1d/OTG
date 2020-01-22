using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


namespace N1D.Core
{
	public enum GameEventId
	{
		Ready,
		Tutorial,
		ReplayTutorial,
		GameStart,
		OnPlayStart,
		AddBlock,
		ReloadBlock,
		ReleaseBlock,
		OnChangedCount,
		OnStartRotateBlock,
		OnEndRotateBlock,
		OutOfScreen,
		InOfScreen,
		GameOver,
		EndGame,
		RequestRetry,
		Capture,
		Tweet,
	}


	public interface IGameEventReceivable : IEventSystemHandler
	{
		void OnReceiveGameEvent(GameEventId id);
	}
} // N1D.Core