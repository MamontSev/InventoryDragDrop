using Inventory.Events;
using Inventory.Events.Signals;

namespace Inventory.Core
{
	public class LevelState
	{
		enum SelfState
		{
			Paused = 0,
			GameLoop = 1
		}

		private IEventBusService _eventBusService;
		public LevelState( IEventBusService _eventBusService )
        {
			this._eventBusService = _eventBusService;

		}

        public void SetToPaused()
		{
			_selfState = SelfState.Paused;
			_eventBusService.Invoke(new GamePlayPauseSignal(true));
		}
		public void SetToGameLoop()
		{
			_selfState = SelfState.GameLoop;
			_eventBusService.Invoke(new GamePlayPauseSignal(false));
		}

		public bool IsPlayimg => _selfState == SelfState.GameLoop;

		private SelfState _selfState = SelfState.Paused;
	}
}
