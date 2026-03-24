using UnityEditor;
using UnityEngine;

namespace Justin
{
	public enum Background { None, A, B, C}

	public abstract class Action : MonoBehaviour
	{
		//public System.Action updateCustomInspector;
		[HideInInspector]
		public Background backGround = Background.None;

		//public Editor customInspector;
	}
}

