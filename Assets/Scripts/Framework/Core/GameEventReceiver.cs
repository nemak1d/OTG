using UnityEngine;
using UnityEngine.EventSystems;

namespace N1D.Framework.Core
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

		Beat,
	}

	public struct GameEventVariant
	{
		public GameEventId id;
		public int intValue;
		public float floatValue;
		public bool boolValue;
		public Vector3 vector3Value;
	}


	public interface IGameEventReceivable : IEventSystemHandler
	{
		void OnReceiveGameEvent(GameEventVariant id);
	}
} // N1D.Framework.Core