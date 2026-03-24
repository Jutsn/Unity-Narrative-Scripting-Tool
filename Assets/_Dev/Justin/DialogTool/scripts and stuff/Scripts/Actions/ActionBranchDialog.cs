using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Justin
{
	public class ActionBranchDialog : Action
	{
		public List<DialogEntry> dialogList = new List<DialogEntry> { new DialogEntry() };
		public Action optionA;
		public Action optionB;
		public string buttonTextA;
		public string buttonTextB;
	}
}

