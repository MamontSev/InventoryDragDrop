using Cysharp.Threading.Tasks;

using UnityEngine.SceneManagement;

namespace Inventory.SceneControl
{
	public class SceneControlService: ISceneControlService
	{
		public void LoadLevel( )
		{
			LoadScene("Level");
		}
		private async void LoadScene( string name )
		{
			await SceneManager.LoadSceneAsync(name , LoadSceneMode.Single);
		}
	}
}
