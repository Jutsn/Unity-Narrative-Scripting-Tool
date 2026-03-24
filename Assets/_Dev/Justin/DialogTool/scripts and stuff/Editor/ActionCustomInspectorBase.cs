

using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
namespace Justin
{
	
	public abstract class ActionCustomInspectorBase : Editor
	{
		
		public VisualTreeAsset visualTree;

		public MultiColumnListView multiColumnListView;
		public VisualElement pane;

		public VisualElement allElementsToHide;
		
		public PropertyField toggleField;
		public SerializedProperty boolProperty;
		public SerializedProperty backGround;

		public VisualElement nextActionBox;
		public ObjectField nextActionField;
		 protected virtual void OnEnable()
		{
			CustomEditorRegistry.Register(this);
			boolProperty = serializedObject.FindProperty("jumpToAction");
			backGround = serializedObject.FindProperty("backGround");
			//((Action)target).updateCustomInspector += OnUpdateCustomInspector;
		}

		protected virtual void OnDisable()
		{
			CustomEditorRegistry.Unregister(this);
			//((Action)target).updateCustomInspector -= OnUpdateCustomInspector;
			if (nextActionField != null)
			{
				nextActionField.UnregisterCallback<ChangeEvent<UnityEngine.Object>>(OnNextActionFieldChanged);
			}
			if(toggleField != null)
			{
				toggleField.RegisterCallback<ChangeEvent<bool>>(OnBoolChanged);
			}
		}

		public virtual void FindBasicReferences(VisualElement root)
		{
			pane = root.Q<VisualElement>("pane");
			multiColumnListView = root.Q<MultiColumnListView>("MultiColumnListView1");
			toggleField = root.Q<PropertyField>("ToggleBool");
			allElementsToHide = root.Q<VisualElement>("AllElementsToHide");
			nextActionBox = root.Q<VisualElement>("nextActionBox");
			nextActionField = root.Q<ObjectField>("nextActionField");
		}
	
		protected virtual void PaintBackGround()
		{
			switch (backGround.enumValueIndex)
			{
				case 0: //BackGround.None
					pane.RemoveFromClassList("backGroundA");
					pane.RemoveFromClassList("backGroundB");
					pane.RemoveFromClassList("backGroundC");
					break;
				case 1: //BackGround.A
					pane.RemoveFromClassList("backGroundB");
					pane.RemoveFromClassList("backGroundC");
					pane.AddToClassList("backGroundA");
					break;
				case 2: //BackGround.B
					pane.RemoveFromClassList("backGroundA");
					pane.RemoveFromClassList("backGroundC");
					pane.AddToClassList("backGroundB");
					break;
				case 3: //BackGround.C
					pane.RemoveFromClassList("backGroundA");
					pane.RemoveFromClassList("backGroundB");
					pane.AddToClassList("backGroundC");
					break;
				default:
					pane.RemoveFromClassList("backGroundA");
					pane.RemoveFromClassList("backGroundB");
					pane.RemoveFromClassList("backGroundC");
					break;
			}
			
		}

		protected virtual void PaintRowInColumn()
		{

			// Nur die dritte färben
			var lastColumn = multiColumnListView.columns[3];
			lastColumn.bindCell = (cell, rowIndex) =>
			{

				// Färbe die gesamte ZEILE über das Parent-Element
				var row = cell.parent; // Das ist die gesamte Zeile
				if (row != null)
				{
					if (IsLineSpecial(rowIndex))
					{
						row.AddToClassList("row-action");
					}
					else
					{
						row.RemoveFromClassList("row-action");
					}
				}
			};
		}

		protected virtual bool IsLineSpecial(int rowIndex)
		{
			var action = (Action) target;
			switch (action)
			{
				case ActionDialog actionDialog:
					if (rowIndex >= 0 && rowIndex < actionDialog.dialogList.Count)
					{
						return actionDialog.dialogList[rowIndex].HasAction();
					}
					break;
				case ActionBranchDialog actionBranchDialog:
					if (rowIndex >= 0 && rowIndex < actionBranchDialog.dialogList.Count)
					{
						return actionBranchDialog.dialogList[rowIndex].HasAction();
					}
					break;
				case ActionQuestionWithInput actionQuestion:
					if (rowIndex >= 0 && rowIndex < actionQuestion.dialogList.Count)
					{
						return actionQuestion.dialogList[rowIndex].HasAction();
					}
					break;
				default:
					break;
			}
			return false;
		}

		protected virtual void OnBoolChanged(ChangeEvent<bool> evt)
		{
			CheckForDisplayType();
		}

		protected virtual void CheckForDisplayType()
		{
			if (boolProperty.boolValue == true)
			{
				allElementsToHide.style.display = DisplayStyle.Flex;
			}
			else
			{
				allElementsToHide.style.display = DisplayStyle.None;
			}
		}

		protected virtual void OnNextActionFieldChanged(ChangeEvent<UnityEngine.Object> evt)
		{
			serializedObject.Update();
			// Nutze evt.previousValue und evt.newValue direkt aus dem Event
			Action oldAction = evt.previousValue as Action;
			Action newAction = evt.newValue as Action;
			if (oldAction != null && oldAction != newAction)
			{
				nextActionBox.RemoveFromClassList("backGroundC");
				oldAction.backGround = Background.None;
				//oldAction.updateCustomInspector();
				//EditorUtility.SetDirty(oldAction);
				CustomEditorRegistry.RepaintAll();

			}

			if (newAction != null)
			{
				nextActionBox.AddToClassList("backGroundC");
				newAction.backGround = Background.C;
				//newAction.updateCustomInspector();
				//EditorUtility.SetDirty(newAction);
				CustomEditorRegistry.RepaintAll();
			}
			
		}

		//public virtual void OnUpdateCustomInspector()
		//{
		//	if (this == null || target == null)
		//		return;
		//	Debug.LogWarning("sdsdds");
		//	serializedObject.Update();
		//}

	}



}
