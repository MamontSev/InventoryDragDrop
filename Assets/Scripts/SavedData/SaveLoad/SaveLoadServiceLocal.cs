using System;
using System.IO;

using Newtonsoft.Json;

using UnityEngine;

namespace Inventory.SaveData.SaveLoad
{
	public class SaveLoadServiceLocal:ISaveLoadService
	{

		public SaveLoadServiceLocal()
		{
		}
		public void Load<T>( string key , Action<T> onComplete , Action onNoExistData , Action<string> onFailLoad )
		{
			if( File.Exists(FullPath(key)) )
			{
				try
				{
					string json = "";
					using( FileStream stream = new FileStream(FullPath(key) , FileMode.Open) )
					{
						using( StreamReader reader = new StreamReader(stream) )
						{
							json = reader.ReadToEnd();
						}
					}
					json = DescryptEncrypt(json);
					var data = JsonConvert.DeserializeObject<T>(json);
					onComplete.Invoke(data);
				}
				catch( Exception ex )
				{
					onFailLoad.Invoke(ex.Message);
				}
			}
			else
			{
				onNoExistData.Invoke();
			}
		}

		public void Save( string key , object data )
		{
			try
			{
				string directory = Path.GetDirectoryName(FullPath(key));
				if( !Directory.Exists(directory) )
				{
					Directory.CreateDirectory(directory);
				}
				string json = JsonConvert.SerializeObject(data , Formatting.Indented);
				json = DescryptEncrypt(json);
				using FileStream stream = new FileStream(FullPath(key) , FileMode.Create);
				using StreamWriter writer = new StreamWriter(stream);
				writer.Write(json);

			}
			catch( Exception ex )
			{
				Debug.LogError($"Fail save: {ex.Message} ");
			}
		}
		private string FullPath( string dataFileName ) => Path.Combine(Application.persistentDataPath , dataFileName);

		private string DescryptEncrypt( string s )
		{
			return s;
			//string str = "gamedata";
			//string retVal = "";
			//for( int i = 0; i < s.Length; i++ )
			//{
			//	retVal += (char)( s[i] ^ str[i % str.Length] );
			//}
			//return retVal;
		}
	}
}
