using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Justin
{
	public class ActionDialog : Action
	{
		public bool jumpToAction;
		public Action nextAction;
		public List<DialogEntry> dialogList = new List<DialogEntry> { new DialogEntry() };

	}
}

