using TMPro;

using UnityEngine;

namespace Inventory.Core.View.Panel
{
	public class ItemTooltip:MonoBehaviour
	{
		[SerializeField]
		private GameObject InGo;

		[SerializeField]
		private TextMeshProUGUI TooltipText;
															  
		private void Start()
		{
			InGo.SetActive(false);
		}

		private bool _isPointerOver;
		private bool _isShow = false;
		private float _timeEnter = 0.0f;
		public void EnterPointer(Transform itemTransform,string itemName)
		{
			if( _isBlock )
				return;
			_timeEnter = Time.time;
			_isPointerOver = true;
			TooltipText.text = itemName;
			transform.position = itemTransform.position;
		}
		public void ExitPointer()
		{
			_isPointerOver = false;
		}

		private bool _isBlock = false;
		public void Block()
		{
			_isBlock = true;
			_isShow = false;
			InGo.SetActive(false);
		}
		public void UnBlock()
		{
			_isBlock = false;
		}

		private void Update()
		{
			if( _isShow == false )
			{
				if( _isPointerOver == true )
				{
					if( ( _timeEnter + 0.2f ) < Time.time )
					{
						_isShow = true;
						InGo.SetActive(true);
					}
				}
			}
			else
			{
				if( _isPointerOver == false )
				{
					_isShow = false;
					InGo.SetActive(false);
				}
			}
		}
	}
}
