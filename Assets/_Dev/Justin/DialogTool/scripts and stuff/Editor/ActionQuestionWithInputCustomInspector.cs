using UnityEditor;

using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
namespace Justin
{
	[CustomEditor(typeof(ActionQuestionWithInput))]
	public class ActionQuestionWithInputCustomInspector : ActionCustomInspectorDefault
	{
		private VisualElement popUpElementsToHide;
		private PropertyField popUpToggleField;
		private SerializedProperty popUpBoolProperty;

		protected override void OnDisable()
		{
			base.OnDisable();
			popUpToggleField.UnregisterCallback<ChangeEvent<bool>>(OnBoolChanged);
		}
		protected override void OnEnable()
		{
			base.OnEnable();
			popUpBoolProperty = serializedObject.FindProperty("showAreYouSurePopUp");
		}
		public override VisualElement CreateInspectorGUI()
		{
			VisualElement root = new VisualElement();
			//Add all the UI Builder stuff
			if (visualTree != null)
			{
				visualTree.CloneTree(root);
				FindBasicReferences(root);
				
				
				popUpToggleField = root.Q<PropertyField>("PopUpToggleBool");
				popUpElementsToHide = root.Q<VisualElement>("PopUpElementsToHide");
				

				popUpToggleField.RegisterCallback<ChangeEvent<bool>>(OnBoolChanged);
				toggleField.RegisterCallback<ChangeEvent<bool>>(OnBoolChanged);
				nextActionField.RegisterCallback<ChangeEvent<UnityEngine.Object>>(OnNextActionFieldChanged);

				PaintRowInColumn();
				PaintBackGround();
				CheckForDisplayType();

			}
			
			return root;
		}

		

		protected override void CheckForDisplayType()
		{
			base.CheckForDisplayType();

			if (popUpBoolProperty.boolValue == true)
			{
				popUpElementsToHide.style.display = DisplayStyle.Flex;
			}
			else
			{
				popUpElementsToHide.style.display = DisplayStyle.None;
			}
		}

	}
}

