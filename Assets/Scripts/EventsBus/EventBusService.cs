using System;
using System.Collections.Generic;

using System.Linq;

using Inventory.Events.Signals;

using UnityEngine;

namespace Inventory.Events
{
	public class EventBusService:IEventBusService 
	{
		public EventBusService( )
		{
		}

		private Dictionary<string , List<object>> _signalCallbacks = new Dictionary<string , List<object>>();

		public void Subscribe<T>( Action<T> callback ) where T : IEventBusSignal
		{
			string key = typeof(T).Name;
			if( _signalCallbacks.ContainsKey(key) )
			{
				_signalCallbacks[key].Add(callback);
			}
			else
			{
				_signalCallbacks.Add(key , new List<object>() { callback });
			}

		}

		public void Unsubscribe<T>( Action<T> callback ) where T : IEventBusSignal
		{
			string key = typeof(T).Name;
			if( _signalCallbacks.ContainsKey(key) )
			{
				var callbackToDelete = _signalCallbacks[key].FirstOrDefault(x => x.Equals(callback));
				if( callbackToDelete != null )
				{
					_signalCallbacks[key].Remove(callbackToDelete);
				}
			}
			else
			{
				Debug.LogError($"Trying to unsubscribe for not existing key! {key} ");
			}
		}

		public void Invoke<T>( T signal ) where T : IEventBusSignal
		{
			string key = typeof(T).Name;
			if( _signalCallbacks.ContainsKey(key) )
			{
				for( int i = _signalCallbacks[key].Count - 1; i >= 0; i-- )
				{
					var callback = _signalCallbacks[key][i] as Action<T>;
					callback?.Invoke(signal);
				}
			}
		}
	}
}