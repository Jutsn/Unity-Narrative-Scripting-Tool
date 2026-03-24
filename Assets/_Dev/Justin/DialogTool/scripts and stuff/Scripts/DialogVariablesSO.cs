using System.Collections.Generic;
using UnityEngine;

namespace Justin
{
	[CreateAssetMenu(fileName = "Unnamed VariableSafe", menuName = "New VariableSafe")]
	public class DialogVariablesSO : ScriptableObject
	{
		public List<string> variables;

		public Dictionary<string, string> dic = new Dictionary<string, string>();
		//Befüllt das Dictionary zum Spielstart mit den Variablen aus der Inspector-Liste und gibt ihnen allen den Pseudeo-Value "null."
		private void OnEnable()
		{
			for (int i = 0; i < variables.Count; i++)
			{
				dic.TryAdd(variables[i], "null");
			}

		}
	}
}

