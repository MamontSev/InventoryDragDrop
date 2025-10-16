using System;

using Cysharp.Threading.Tasks;

using DG.Tweening;

using TMPro;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using Zenject;

namespace Inventory.Core.View.Panel
{
	public class Item:MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
	{
		private LevelState _levelState;
		[Inject]
		private void Construct( LevelState _levelState )
		{
			this._levelState = _levelState;
		}

		private void Awake()
		{
			_showScale = transform.localScale;
		}

		[SerializeField]
		private Image Icon;

		[SerializeField]
		private GameObject SelectedGo;

		[SerializeField]
		private GameObject UseGo;

		[SerializeField]
		private GameObject CountCont;
		[SerializeField]
		private TextMeshProUGUI CountText;

		private bool IsActive => _isMove == false && _isShow == true && _levelState.IsPlayimg;

		#region Init

		private int _index;
		private string _nameText;
		public void InitState( int index , Sprite iconSprite , string name , bool needCount )
		{
			_index = index;
			Icon.sprite = iconSprite;
			_nameText = name;
			CountCont.SetActive(needCount);
			SetSelected(false);
			SetUse(false);
		}

		private Action<int> _onPressed;
		private Action<int> _onDoublePressed;
		private Action<int> _onStartDrag;
		private Action<int> _onEndDrag;
		public void InitCallBack(
			Action<int> onPressed , Action<int> onDoublePressed ,
			Action<int> onStartDrag , Action<int> onEndDrag )
		{
			_onPressed = onPressed;
			_onDoublePressed = onDoublePressed;
			_onStartDrag = onStartDrag;
			_onEndDrag = onEndDrag;
		}

		private ItemTooltip _tooltip;
		public void InitTooltip( ItemTooltip tooltip )
		{
			_tooltip = tooltip;
		}

		#endregion

		#region Public

		public int FieldIndex
		{
			get; set;
		}

		public (int Type, int Key) SelfType
		{
			get;set;
		}


		public void SetCount( int count )
		{
			CountText.text = count.ToString();
		}

		public void SetSelected( bool state )
		{
			SelectedGo.SetActive(state);
		}

		public void SetUse( bool state )
		{
			UseGo.SetActive(state);
		}

		#endregion

		#region Show_Hide

		private bool _isShow = false;
		private Vector3 _showScale;
		public async UniTaskVoid Show()
		{
			await transform
		   .DOScale(_showScale , 0.2f)
		   .SetEase(Ease.OutBack)
		   .From(Vector3.zero)
		   .ToUniTask(TweenCancelBehaviour.Kill , destroyCancellationToken);
			_isShow = true;
		}
		public async UniTask Hide()
		{
			_isShow = false;
			await transform
		   .DOScale(Vector3.zero , 0.2f)
		   .SetEase(Ease.OutBack)
		   .From(_showScale)
		   .ToUniTask(TweenCancelBehaviour.Kill , destroyCancellationToken);
		}

		#endregion

		#region Move

		private readonly float _moveSpped = 10000.0f;
		private bool _isMove = false;
		public async void MoveToPlace( Vector3 pos )
		{
			float dist = Vector3.Distance(transform.localPosition, pos);
			float moveTime = dist / _moveSpped;
			_isMove = true;
			await transform
			.DOMove(pos , moveTime)
			.SetEase(Ease.OutBack)
			.From(transform.position)
			.ToUniTask();
			_isMove = false;
		}

		#endregion

		#region IPointer

		private Vector3 _pointerDownPos;
		private bool _isDrag = false;
		private float _lastClickTime = 0.0f;
		public void OnPointerDown( PointerEventData eventData )
		{
			if( IsActive == false )
				return;
			_pointerDownPos = eventData.position;
			
			_tooltip.ExitPointer();
		}
		public void OnPointerUp( PointerEventData eventData )
		{
			if( _isDrag == false )
				return;
			_isDrag = false;
			_onEndDrag?.Invoke(_index);
		}
	
		public void OnDrag( PointerEventData eventData )
		{
			if( _isDrag == false )
			{
				float dist = Vector3.Distance(eventData.position , _pointerDownPos);
				if( dist > 10.0f )
				{
					_isDrag = true;
					_onStartDrag?.Invoke(_index);
				}
			}
			else
			{
				transform.position += (Vector3)eventData.delta;
			}
		}


		public void OnPointerEnter( PointerEventData eventData )
		{
			if( IsActive == false )
				return;
			_tooltip.EnterPointer(transform , _nameText);
		}

		public void OnPointerExit( PointerEventData eventData )
		{
			_tooltip.ExitPointer();

		}
		public void OnPointerClick( PointerEventData eventData )
		{
			if( IsActive == false )
				return;
			if( ( _lastClickTime + 0.3f ) > Time.time )
				_onDoublePressed?.Invoke(_index);
			else
				_onPressed?.Invoke(_index);
			_lastClickTime = Time.time;
		}

		#endregion
	}
}
