using System.Collections.Generic;
using UnityEngine;

namespace Justin
{
	public class ActionQuestionWithInput : Action
	{
		public List<DialogEntry> dialogList = new List<DialogEntry> { new DialogEntry() };
		public string variableToSaveTo;
		public bool showAreYouSurePopUp;
		public string popUpText;

		public bool jumpToAction;
		public Action nextAction;

		
	}
	
}

