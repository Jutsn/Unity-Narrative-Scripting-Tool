using UnityEngine;
namespace Justin
{
	public class ActionTriggerAnimation : Action
	{
		public string objectToAnimate;
		public string trigger;
		public float animationSpeed = 1;
		public string object2ToAnimate;
		public string trigger2;
		public float animation2Speed = 1;

		public void TriggerAnimation()
		{
			if (objectToAnimate != null && trigger != null)
			{
				DialogManager.Instance.SetAnimatorTrigger(objectToAnimate, trigger, animationSpeed);
			}
			if (object2ToAnimate != null && trigger2 != null)
			{
				DialogManager.Instance.SetAnimatorTrigger(object2ToAnimate, trigger2, animation2Speed);
			}
			else
			{
				Debug.Log("dsdfsdsds");
			}

		}
	}
}
