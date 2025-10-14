using System;
using System.Linq;
using System.Net.NetworkInformation;

using Cysharp.Threading.Tasks;

using DG.Tweening;

using Inventory.Core.View.Panel;
using Inventory.Events;
using Inventory.Events.Signals;
using Inventory.State;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

using Zenject;

using static Inventory.Data.Static.Enums;

namespace Inventory.Core.View
{
	public class AddItemSimulator:MonoBehaviour
	{
		private IInventoryStateService _state;
		private IEventBusService _eventBusService;
		private InventoryView _inventoryView;
		private LevelState _levelState;
		private LevlUI _levlUI;
		[Inject]
		private void Construct
		(
			IInventoryStateService _state ,
			IEventBusService _eventBusService,
			InventoryView _inventoryView ,
			LevelState _levelState ,
			LevlUI _levlUI
		)
		{
			this._state = _state;
			this._eventBusService = _eventBusService;
			this._inventoryView = _inventoryView;
			this._levelState = _levelState;
			this._levlUI = _levlUI;
		}

		[SerializeField]
		private GameObject InGo;
		[SerializeField]
		private Transform HidePos;

		[SerializeField]
		private Transform ShowPos;

		[SerializeField]
		private TMP_Dropdown TypeDropDown;

		[SerializeField]
		private TMP_Dropdown KeyDropDown;

		[SerializeField]
		private Button AddButton;

		private void Start()
		{
			InGo.transform.localPosition = HidePos.position;
			TypeDropDown.ClearOptions();
			KeyDropDown.ClearOptions();

			TypeDropDown.AddOptions(Enum.GetNames(typeof(ItemType)).ToList());

			DropTypeChanged(TypeDropDown.value);

			TypeDropDown.onValueChanged.AddListener(DropTypeChanged);
		}

		private void OnEnable()
		{
			AddButton.onClick.AddListener(PressedAdd);
			_eventBusService.Subscribe<GamePlayPauseSignal>(OnGamePlayPauseSignal);
		}

		private void OnDisable()
		{
			AddButton.onClick.RemoveAllListeners();
			_eventBusService.Subscribe<GamePlayPauseSignal>(OnGamePlayPauseSignal);
		}

		private void DropTypeChanged( int value )
		{
			Type EnumType = GetItemEnumType((ItemType)TypeDropDown.value);
			KeyDropDown.ClearOptions();
			KeyDropDown.AddOptions(Enum.GetNames(EnumType).ToList());
		}

		private async void PressedAdd()
		{
			if( _levelState.IsPlayimg == false )
				return;
			
			if( _inventoryView.HaveFreeCell )
			{
				_state.AddItem((ItemType)TypeDropDown.value , KeyDropDown.value);
			}
			else
			{
				await ShowMsg();
			}

		}

		private async UniTask ShowMsg()
		{
			_levelState.SetToPaused();
			await _levlUI.ShowMessage("Нет пустых ячеек");
			_levelState.SetToGameLoop();
		}

		private void OnGamePlayPauseSignal( GamePlayPauseSignal signal )
		{
			AddButton.interactable = !signal.IsPause;
		}




		public async UniTask Show()
		{
			await InGo.transform
			.DOMove(ShowPos.position , 0.5f)
			.SetEase(Ease.OutBack)
			.From(HidePos.position)
			.ToUniTask();

		}




	}
}
