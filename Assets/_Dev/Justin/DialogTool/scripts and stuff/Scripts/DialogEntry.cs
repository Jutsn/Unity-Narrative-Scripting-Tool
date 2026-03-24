using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class DialogEntry
{
	public string speaker;
	public string text;
	public Sprite speakerImage;
	public Sprite backgroundImage;
	public string objectToAnimate;
	public string object2ToAnimate;
	public string trigger;
	public string trigger2;
	public float animationSpeed = 1f;
	public float animation2Speed = 1f;
	public AudioClip clip;
	public AudioClip clip2;

	public bool HasAction()
	{
		if(!string.IsNullOrEmpty(objectToAnimate) || !string.IsNullOrEmpty(object2ToAnimate) || !string.IsNullOrEmpty(trigger) || !string.IsNullOrEmpty(trigger2) || clip != null || clip2 != null || backgroundImage != null)
		{
			return true;
		}
		else
		{
			return false;
		}

	}
}

