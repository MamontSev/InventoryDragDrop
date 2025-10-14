namespace Inventory.Events.Signals
{
	public class GamePlayPauseSignal:IEventBusSignal
	{
		public readonly bool IsPause;
		public GamePlayPauseSignal( bool isPause )
		{
			IsPause = isPause;
		}
	}
}										  