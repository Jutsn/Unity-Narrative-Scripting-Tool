using Justin;
using UnityEditor;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(Action), true)]
public class ActionCustomInspectorDefault : ActionCustomInspectorBase
{
	
	public override VisualElement CreateInspectorGUI()
	{
		VisualElement root = new VisualElement();
		visualTree.CloneTree(root);
		FindBasicReferences(root);

		// Standard-Inspector hinzufügen
		var defaultInspectorContainer = root.Q<IMGUIContainer>("StandardInspectorContainer");
		defaultInspectorContainer.onGUIHandler = () =>
		{
			DrawDefaultInspector(); // wird innerhalb des Containers gezeichnet
		};

		PaintBackGround();

		return root;
	}
}
