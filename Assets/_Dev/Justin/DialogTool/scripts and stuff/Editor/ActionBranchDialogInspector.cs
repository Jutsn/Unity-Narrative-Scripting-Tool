
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Justin
{
	[CustomEditor(typeof(ActionBranchDialog))]
	public class ActionBranchDialogInspector : ActionCustomInspectorDefault
	{
		private ObjectField objectFieldA;
		private ObjectField objectFieldB;
		private VisualElement boxA;
		private VisualElement boxB;

		
		protected override void OnDisable()
		{
			base.OnDisable();
			if (objectFieldA != null)
			{
				objectFieldA.UnregisterCallback<ChangeEvent<UnityEngine.Object>>(OnOptionFieldAChanged);
			}	
			if (objectFieldB != null)
			{
				objectFieldB.UnregisterCallback<ChangeEvent<UnityEngine.Object>>(OnOptionFieldBChanged);
			}	
		}

		public override VisualElement CreateInspectorGUI()
		{
			VisualElement root = new VisualElement();
			//Add all the UI Builder stuff
			if (visualTree != null)
			{
				visualTree.CloneTree(root);

				pane = root.Q<VisualElement>("pane");
				multiColumnListView = root.Q<MultiColumnListView>("MultiColumnListView1");


				boxA = root.Q<VisualElement>("boxA");
				boxB = root.Q<VisualElement>("boxB");
				
				objectFieldA = root.Q<ObjectField>("ObjectField1");
				objectFieldB = root.Q<ObjectField>("ObjectField2");
				objectFieldA.RegisterCallback<ChangeEvent<UnityEngine.Object>>(OnOptionFieldAChanged);
				objectFieldB.RegisterCallback<ChangeEvent<UnityEngine.Object>>(OnOptionFieldBChanged);

				PaintRowInColumn();
				PaintBackGround();

			}
			return root;

		}

		public virtual void OnOptionFieldAChanged(ChangeEvent<UnityEngine.Object> evt)
		{
			serializedObject.Update();
			// Nutze evt.previousValue und evt.newValue direkt aus dem Event
			Action oldAction = evt.previousValue as Action;
			Action newAction = evt.newValue as Action;
			if (oldAction != null && oldAction != newAction)
			{
				boxA.RemoveFromClassList("backGroundA");
				oldAction.backGround = Background.None;
				//oldAction.updateCustomInspector();
				//EditorUtility.SetDirty(oldAction);
				CustomEditorRegistry.RepaintAll();
			}
			
			if (newAction != null)
			{
				boxA.AddToClassList("backGroundA");
				newAction.backGround = Background.A;
				//newAction.updateCustomInspector();
				//EditorUtility.SetDirty(newAction);
				CustomEditorRegistry.RepaintAll();
			}
			
			
		}
		public virtual void OnOptionFieldBChanged(ChangeEvent<UnityEngine.Object> evt)
		{
			serializedObject.Update();
			Action oldAction = evt.previousValue as Action;
			Action newAction = evt.newValue as Action;

			if (oldAction != null && oldAction != newAction)
			{
				boxB.RemoveFromClassList("backGroundB");
				oldAction.backGround = Background.None;
				//oldAction.updateCustomInspector();
				//EditorUtility.SetDirty(oldAction);
				CustomEditorRegistry.RepaintAll();
				//UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
			}
			

			if (newAction != null)
			{
				boxB.AddToClassList("backGroundB");
				newAction.backGround = Background.B;
				//newAction.updateCustomInspector();
				//EditorUtility.SetDirty(newAction);
				CustomEditorRegistry.RepaintAll();
				//UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
			}


		}



	}
}

