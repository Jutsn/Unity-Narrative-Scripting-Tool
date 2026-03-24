using UnityEngine;
namespace Justin
{
	public class ActionPlaySFX : Action
	{
		public AudioClip clipToPlay;
		public AudioClip clipToPlay2;

		public void PlaySFX()
		{
			if (clipToPlay != null)
			{
				SFXManager.Instance.PlaySFX(clipToPlay);
			}
			if (clipToPlay2 != null)
			{
				SFXManager.Instance.PlaySFX(clipToPlay2);
			}
		}
	}
}

