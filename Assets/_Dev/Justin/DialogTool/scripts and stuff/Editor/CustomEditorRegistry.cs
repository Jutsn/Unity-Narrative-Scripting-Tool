using Justin;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;
[InitializeOnLoad]
public class CustomEditorRegistry
{
	private static readonly HashSet<Editor> activeEditors = new();

	static CustomEditorRegistry()
	{
		EditorApplication.quitting += Clear;
	}

	public static void Register(Editor editor)
	{
		if (editor == null) return;
		activeEditors.Add(editor);
		
	}

	public static void Unregister(Editor editor)
	{
		if (editor == null) return;
		activeEditors.Remove(editor);
	}

	public static void RepaintAll()
	{
		foreach (var editor in activeEditors)
		{
			if (editor != null)
			{
				editor.serializedObject.UpdateIfRequiredOrScript();

				
			}
		}
	}

	private static void Clear()
	{
		activeEditors.Clear();
	}
}
	


