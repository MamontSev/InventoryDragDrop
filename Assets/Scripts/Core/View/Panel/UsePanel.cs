using System;

using Cysharp.Threading.Tasks;

using DG.Tweening;

using Inventory.Events;
using Inventory.Events.Signals;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

using Zenject;

namespace Inventory.Core.View.Panel
{
	public class UsePanel:MonoBehaviour
	{
		private LevelState _levelState;
		private IEventBusService _eventBusService;
		[Inject]
		private void Construct
		(
			LevelState _levelState ,
			IEventBusService _eventBusService
		)
		{
			this._levelState = _levelState;
			this._eventBusService = _eventBusService;
		}
		[SerializeField]
		private GameObject InGo;
		[SerializeField]
		private Transform HidePos;

		[SerializeField]
		private Transform ShowPos;

		[SerializeField]
		private Button PutOnButton;

		[SerializeField]
		private Button TakeOffButton;

		[SerializeField]
		private Button DropButton;

		[SerializeField]
		private GameObject ButtonsCont;

		[SerializeField]
		private TextMeshProUGUI DescriptionText;

		[SerializeField]
		private Button SortButton;

		private void Start()
		{
			InGo.transform.localPosition = HidePos.position;
			SetDescriptionText("");
			HideItemCont();
		}

		private void OnEnable()
		{
			PutOnButton.onClick.AddListener(() =>
			{
				if( _levelState.IsPlayimg == false )
					return;
				OnPutOn?.Invoke();
			});
			TakeOffButton.onClick.AddListener(() =>
			{
				if( _levelState.IsPlayimg == false )
					return;
				OnTakeOff?.Invoke();
			});
			DropButton.onClick.AddListener(() =>
			{
				if( _levelState.IsPlayimg == false )
					return;
				OnDrop?.Invoke();
			});
			SortButton.onClick.AddListener(() =>
			{
				if( _levelState.IsPlayimg == false )
					return;
				OnSort?.Invoke();
			});

			_eventBusService.Subscribe<GamePlayPauseSignal>(OnGamePlayPauseSignal);
		}

		private void OnDisable()
		{
			PutOnButton.onClick.RemoveAllListeners();
			TakeOffButton.onClick.RemoveAllListeners();
			DropButton.onClick.RemoveAllListeners();
			SortButton.onClick.RemoveAllListeners();
			_eventBusService.Subscribe<GamePlayPauseSignal>(OnGamePlayPauseSignal);
		}

		public async UniTask Show()
		{
			await InGo.transform
			.DOMove(ShowPos.position , 0.5f)
			.SetEase(Ease.OutBack)
			.From(HidePos.position)
			.ToUniTask(TweenCancelBehaviour.Kill , destroyCancellationToken);
		}

		private Action OnPutOn;
		private Action OnTakeOff;
		private Action OnDrop;
		private Action OnSort;
		public void InitCallBack( Action OnPutOn, Action OnTakeOff, Action OnDrop , Action OnSort )
		{
			this.OnPutOn = OnPutOn;
			this.OnTakeOff = OnTakeOff;
			this.OnDrop = OnDrop;
			this.OnSort = OnSort;
		}

		public void ShowItemCont( bool isPutOn, bool isIakeOff )
		{
			ButtonsCont.SetActive(true);
			PutOnButton.gameObject.SetActive(isPutOn);
			TakeOffButton.gameObject.SetActive(isIakeOff);
		}

		public void HideItemCont()
		{
			ButtonsCont.SetActive(false);
			SetDescriptionText("");
		}
		public void SetDescriptionText(string description)
		{
			DescriptionText.text = description;
		}

		private void OnGamePlayPauseSignal( GamePlayPauseSignal signal )
		{
			PutOnButton.interactable = !signal.IsPause;
			TakeOffButton.interactable = !signal.IsPause;
			DropButton.interactable = !signal.IsPause;
			SortButton.interactable = !signal.IsPause;
		}

	}
}
