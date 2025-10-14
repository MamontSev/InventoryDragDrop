using Inventory.Data.Config;
using Inventory.Data.Service;
using Inventory.Events;
using Inventory.SaveData;
using Inventory.SaveData.SaveLoad;
using Inventory.SceneControl;
using Inventory.State;

using UnityEngine;

using Zenject;

namespace Inventory.Installers.Bootstrap
{
	public class ServicesInstaller:MonoInstaller
	{
		public override void InstallBindings()
		{
			
			BindBusService();
			BindSaveService();
			BindSceneControlService();
			BindItemDataService();
			BindInventoryStateService();
		}

		private void BindBusService()
		{
			Container.Bind<IEventBusService>().To<EventBusService>().AsSingle().NonLazy();
		}

		private void BindSaveService()
		{
			Container.Bind<ISaveLoadService>().To<SaveLoadServiceLocal>().AsSingle().NonLazy();

			Container.Bind<ISavedDataService>().To<SavedDataService>().AsSingle().NonLazy();
		}

		private void BindSceneControlService()
		{
			Container.Bind<ISceneControlService>().To<SceneControlService>().AsSingle().NonLazy();
		}

		private void BindInventoryStateService()
		{
			Container.Bind<IInventoryStateService>().To<InventoryStateService>().AsSingle().NonLazy();
		}

		[Header("ItemBehaviourConfig")]
		[SerializeField]
		private ItemBehaviourConfig ItemBehaviourConfig;
		[Header("ItemIConfig")]
		[SerializeField]
		private ItemIConfig ItemIConfig;
		private void BindItemDataService()
		{
			Container
			.Bind<IItemDataService>()
			.To<ItemDataService>()
			.AsSingle()
			.WithArguments(ItemIConfig , ItemBehaviourConfig)
			.NonLazy();
		}
	}
}
