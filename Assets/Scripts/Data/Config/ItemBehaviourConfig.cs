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
	[CreateAssetMenu(menuName = "Data/ItemBehaviourConfig" , fileName = "ItemBehaviourConfig.asset")]
	public class ItemBehaviourConfig:ScriptableObject
	{
		[Header("Выбираем тип поведение для каждого типа предмета(стекуемый или нет) ")]
		[SerializeField]
		private List<RowItem> _itemList;

		public ItemBehaviour GetBehaviourType( ItemType type )
			=> _itemList.First(x => x.SelfType == type).SelfBehaviour;

		[Serializable]
		public class RowItem
		{
			public RowItem( ItemType selfType )
			{
				_selfType = selfType;
			}
			[SerializeField]
			private ItemType _selfType;
			public ItemType SelfType => _selfType;

			[SerializeField]
			private ItemBehaviour _selfBehaviour;
			public ItemBehaviour SelfBehaviour => _selfBehaviour;
		}

		public void Validate( Action onComplete , Action<string> onFail )
		{
			int count = Enum.GetValues(typeof(ItemType)).Length;
			if( count != _itemList.Count )
			{
				onFail?.Invoke("ItemType count != _itemList.Count");
				return;
			}
			if( _itemList.Count != _itemList.Distinct().Count() )
			{
				onFail?.Invoke("_itemList.Count has duplicates");
				return;
			}
			onComplete?.Invoke();
		}


#if UNITY_EDITOR

		public void CheckState()
		{
			foreach( ItemType type in Enum.GetValues(typeof(ItemType)) )
			{
				if( _itemList.Any(x => x.SelfType == type) )
					continue;
				_itemList.Add(new RowItem(type));
			}
			for( int i = _itemList.Count - 1; i >= 0; i-- )
			{
				if( Enum.IsDefined(typeof(ItemType) , _itemList[i].SelfType) )
					continue;
				_itemList.Remove(_itemList[i]);
			}

		}

		[CustomEditor(typeof(ItemBehaviourConfig))]
		class ItemBehaviourConfigCustomizer:Editor
		{
			public override void OnInspectorGUI()
			{
				if( GUI.changed )
				{
					EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
				}
				DrawDefaultInspector();
				ItemBehaviourConfig selfTarget = (ItemBehaviourConfig)target;

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

				width = 120.0f;
				Rect valueRect = new Rect(px , py , width , height);
				int k = property.FindPropertyRelative("_selfType").enumValueIndex;
				EditorGUI.LabelField(valueRect , $"{(ItemType)k}");
				px += width;



				width = 120.0f;
				valueRect = new Rect(px , py , width , height);
				EditorGUI.LabelField(valueRect , "ItemBehaviour -> ");
				px += width;


				width = 120.0f;
				valueRect = new Rect(px , py , width , height);
				EditorGUI.PropertyField(valueRect , property.FindPropertyRelative("_selfBehaviour") , GUIContent.none);
			}
		}


#endif
	}
}
