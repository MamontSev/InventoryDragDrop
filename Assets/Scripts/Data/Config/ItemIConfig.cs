using System;
using System.Collections.Generic;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

using UnityEngine;

using static Inventory.Data.Static.Enums;

namespace Inventory.Data.Config
{
	[CreateAssetMenu(menuName = "Data/ItemIConfig" , fileName = "ItemIConfig.asset")]
	public class ItemIConfig:ScriptableObject
	{
		[SerializeField]
		private List<TypeItem> _itemListType = new();

		public Sprite GetIconSprite( ItemType itemType , int index )
		{
			TypeItem typeItem = _itemListType.First(x => x.SelfType == itemType);
			return typeItem.ItemList.First(x => x.Index == index).Icon;
		}
		public string GetName( ItemType itemType , int index )
		{
			TypeItem typeItem = _itemListType.First(x => x.SelfType == itemType);
			return typeItem.ItemList.First(x => x.Index == index).ItemName;
		}
		public string GetDescription( ItemType itemType , int index )
		{
			TypeItem typeItem = _itemListType.First(x => x.SelfType == itemType);
			return typeItem.ItemList.First(x => x.Index == index).ItemDescription;
		}

		[Serializable]
		public class TypeItem
		{
			public TypeItem( ItemType selfType )
			{
				_selfType = selfType;

			}

			[SerializeField]
			private ItemType _selfType;

			public ItemType SelfType => _selfType;

			[SerializeField]
			private List<RowItem> _itemList = new();
			public List<RowItem> ItemList => _itemList;


		}

		[Serializable]
		public class RowItem
		{
			public RowItem( int index , string name )
			{
				_index = index;
				_name = name;
			}
			[SerializeField]
			private int _index;
			public int Index => _index;

			[SerializeField]
			private string _name;

			[SerializeField]
			private Sprite _icon;
			public Sprite Icon => _icon;

			[SerializeField]
			private string _itemName;
			public string ItemName => _itemName;

			[SerializeField]
			private string _itemDescription;
			public string ItemDescription => _itemDescription;



		}

#if UNITY_EDITOR

		public void CheckState()
		{
			foreach( ItemType type in Enum.GetValues(typeof(ItemType)) )
			{
				if( _itemListType.Any(x => x.SelfType == type) == false )
				{
					_itemListType.Add(new TypeItem(type));
				}
				Type EnumType = GetItemEnumType(type);
				foreach( var enumType in Enum.GetValues(EnumType) )
				{
					TypeItem item = _itemListType.First(x => x.SelfType == type);
					if( item.ItemList.Any(x => x.Index == (int)enumType) == false )
					{
						item.ItemList.Add(new RowItem((int)enumType , enumType.ToString()));
					}
				}
			}
		}

		public void Validate( Action onComplete , Action<string> onFail )
		{
			int count = Enum.GetValues(typeof(ItemType)).Length;
			if( count != _itemListType.Count )
			{
				onFail?.Invoke("ItemType count != _itemListType.Count");
				return;
			}
			foreach( ItemType type in Enum.GetValues(typeof(ItemType)) )
			{
				if( _itemListType.Any(x => x.SelfType == type) )
					continue;
				onFail?.Invoke("_itemListType not Contains {type}");
				return;
			}

			foreach( ItemType type in Enum.GetValues(typeof(ItemType)) )
			{
				TypeItem typeItem = _itemListType.First(x => x.SelfType == type);
				Type EnumType = GetItemEnumType(type);
				int enumTypeCount = Enum.GetValues(EnumType).Length;
				if( enumTypeCount != typeItem.ItemList.Count )
				{
					onFail?.Invoke($"ItemType count != _itemListType.Count {type}");
					return;
				}
				foreach( var enumType in Enum.GetValues(EnumType) )
				{
					TypeItem item = _itemListType.First(x => x.SelfType == type);
					if( item.ItemList.Any(x => x.Index == (int)enumType) == false )
					{
						onFail?.Invoke($"Not Contains {type} {enumType.ToString()}");
						return;
					}
					foreach( var obj in item.ItemList )
					{
						if( obj.Icon == null )
						{
							onFail?.Invoke($"Sprite is null {type} {enumType.ToString()}");
							return;
						}
					}
				}
			}
			onComplete?.Invoke();
		}

		[CustomEditor(typeof(ItemIConfig))]
		class ItemIconConfigCustomizer:Editor
		{
			public override void OnInspectorGUI()
			{
				if( GUI.changed )
				{
					EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
				}
				DrawDefaultInspector();
				ItemIConfig selfTarget = (ItemIConfig)target;

				GUILayout.Label("Инициализировать Item List ");
				if( GUILayout.Button("Init Types" , GUILayout.ExpandWidth(false)) )
				{
					selfTarget.CheckState();
				}
				GUILayout.Label("Проверить валидность данных");
				if( GUILayout.Button("Validate data" , GUILayout.ExpandWidth(false)) )
				{
					selfTarget.Validate(
					() => { EditorUtility.DisplayDialog("OK" , "It's OK" , "OK"); } ,
					( s ) => { EditorUtility.DisplayDialog("Fail" , s , "OK"); });
				}
			}
		}

		[CustomPropertyDrawer(typeof(RowItem))]
		public class RowItemDrawer:PropertyDrawer
		{

			// Draw the property inside the given rect
			public override void OnGUI( Rect position , SerializedProperty property , GUIContent label )
			{
				float x = position.x;
				float y = position.y;
				float w = position.width;
				float h = position.height;
				Rect r = new Rect(x , y , w , h);

				EditorGUI.indentLevel = 1;

				float px = position.x;
				float py = position.y;
				float width = 100.0f;
				float height = position.height;

				width = 30.0f;
				Rect valueRect = new Rect(px , py , width , height);
				int index = property.FindPropertyRelative("_index").intValue;
				EditorGUI.LabelField(valueRect , $"{index}");
				px += width;

				width = 80.0f;
				valueRect = new Rect(px , py , width , height);
				string name = property.FindPropertyRelative("_name").stringValue;
				EditorGUI.LabelField(valueRect , name);
				px += width;

				width = 80.0f;
				valueRect = new Rect(px , py , width , height);
				EditorGUI.LabelField(valueRect , "Sprite -> ");
				px += width;

				width = 120;
				valueRect = new Rect(px , py , width , height);
				EditorGUI.PropertyField(valueRect , property.FindPropertyRelative("_icon") , GUIContent.none);
				px += width;

				width = 80.0f;
				valueRect = new Rect(px , py , width , height);
				EditorGUI.LabelField(valueRect , "Name -> ");
				px += width;

				width = 120;
				valueRect = new Rect(px , py , width , height);
				EditorGUI.PropertyField(valueRect , property.FindPropertyRelative("_itemName") , GUIContent.none);
				px += width;

				width = 100.0f;
				valueRect = new Rect(px , py , width , height);
				EditorGUI.LabelField(valueRect , "Decription -> ");
				px += width;

				width = 250;
				valueRect = new Rect(px , py , width , height);
				EditorGUI.PropertyField(valueRect , property.FindPropertyRelative("_itemDescription") , GUIContent.none);
			}
		}


#endif
	}
}
