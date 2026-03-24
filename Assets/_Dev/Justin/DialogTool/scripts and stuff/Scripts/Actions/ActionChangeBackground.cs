using System;
using UnityEngine;
using UnityEngine.UIElements;
namespace Justin
{
	public class ActionChangeBackground : Action
	{
		public Sprite backgroundToShow;
		public bool justShowNextBackground = true;

		public void SwitchBackground()
		{
			if (!justShowNextBackground)
			{
				BackGroundManager.Instance.SwitchBackGroundTo(backgroundToShow);
			}
			else
			{
				BackGroundManager.Instance.ShowNextBackground();
			}

		}
	}
}

