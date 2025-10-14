using Inventory.Core;
using Inventory.Core.Model;
using Inventory.Core.View;
using Inventory.Core.View.Panel;
using Inventory.Data.Config;

using UnityEngine;

using Zenject;

namespace Inventory.Installers.Level
{
	public class LevelInstaller:MonoInstaller
	{
		public override void InstallBindings()
		{
			BindLevlUI();
			BindLevelState();
			BindFieldConfig();
			BindInventoryView();
			BindMessagePanel();
			BindAddItemSimulator();
			BindUsePanel();
		}

		private void BindLevlUI()
		{
			Container.Bind<LevlUI>().FromComponentInHierarchy().AsSingle().NonLazy();
		}
		private void BindLevelState()
		{
			Container.Bind<LevelState>().AsSingle().NonLazy();
		}
		
		[Header("InventoryView")]
		[SerializeField]
		private InventoryView InventoryView;
		private void BindInventoryView()
		{
			Container.Bind<InventoryView>().FromComponentInHierarchy().AsSingle().NonLazy();
		}

		[Header("InventoryView")]
		[SerializeField]
		private MessagePanel MessagePanel;
		private void BindMessagePanel()
		{
			Container.Bind<MessagePanel>().FromComponentInHierarchy().AsSingle().NonLazy();
		}

		[Header("FieldConfig")]
		[SerializeField]
		private FieldConfig FieldConfig;
		private void BindFieldConfig()
		{
			Container.Bind<FieldConfig>().FromInstance(FieldConfig).AsSingle();
		}

		[Header("AddItemSimulator")]
		[SerializeField]
		private AddItemSimulator AddItemSimulator;
		private void BindAddItemSimulator()
		{
			Container.Bind<AddItemSimulator>().FromComponentInHierarchy().AsSingle().NonLazy();
		}

		[Header("UsePanel")]
		[SerializeField]
		private UsePanel UsePanel;
		private void BindUsePanel()
		{
			Container.Bind<UsePanel>().FromComponentInHierarchy().AsSingle().NonLazy();
		}		
	}
}
