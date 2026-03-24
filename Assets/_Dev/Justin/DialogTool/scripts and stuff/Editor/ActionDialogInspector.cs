using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Justin
{
	[CustomEditor(typeof(ActionDialog))]
	public class ActionDialogInspector : ActionCustomInspectorDefault
	{
		
		
		public override VisualElement CreateInspectorGUI()
		{
			VisualElement root = new VisualElement();
			//Add all the UI Builder stuff
			if (visualTree != null)
			{
				visualTree.CloneTree(root);
				FindBasicReferences(root);

				
				toggleField.RegisterCallback<ChangeEvent<bool>>(OnBoolChanged);
				nextActionField.RegisterCallback<ChangeEvent<UnityEngine.Object>>(OnNextActionFieldChanged);

				PaintRowInColumn();
				PaintBackGround();
				CheckForDisplayType();
			}
			return root;
		}

		

		
	}
}
