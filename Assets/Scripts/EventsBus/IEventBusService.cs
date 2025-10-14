using System;

using Inventory.Events.Signals;

namespace Inventory.Events
{
	public interface IEventBusService
	{
		void Subscribe<T>( Action<T> callback ) where T : IEventBusSignal;
		void Invoke<T>( T signal ) where T : IEventBusSignal;
		void Unsubscribe<T>( Action<T> callback ) where T : IEventBusSignal; 
	}
}
