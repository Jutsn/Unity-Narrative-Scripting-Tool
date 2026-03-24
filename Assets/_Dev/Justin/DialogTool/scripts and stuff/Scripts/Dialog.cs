using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Collections;

namespace Justin
{
	public class Dialog : MonoBehaviour
	{
		private DialogManager dM;
		private Action[] actionArray;
		private int actionCount = 0;
		private int lineCount = 0;
		private bool hasPlayerBeenAsked = false;
		private bool isPlayerSure = false;
		private Dictionary<string, string> varDic;
		bool firstTime = true;
		bool linesProcessed = false;
		bool isWaiting;
		//Derzeit nur f�r branching relevant
		private Action nextAction;
		private void Awake()
		{
			dM = DialogManager.Instance;
			varDic = dM.GetVariablesDictionary();
		}

		private void Start()
		{
			actionArray = gameObject.GetComponentsInChildren<Action>();
		}
		public void ResetDialog()
		{
			firstTime = true;
			actionCount = 0;
			lineCount = 0;
		}


		//Diese Funktion cycelt durch ein Array der angeh�ngten ActionSkripte und f�hrt Aktionen basierend auf der Art des ActionSkripts aus.
		//Sie wird aktiv über den DialogManager und passiv durch das DialogSkript selbst nach Ausführung jedes ActionSkripts gecallt.
		public void ExecuteActions()
		{
			if (firstTime)
			{
				dM.ShowDialogPanel();
				dM.ShowContinueButton();
				firstTime = false;
			}

			if (actionArray[actionCount] is ActionDialog dialog)
			{
				HandleNormalDialog(dialog);
			}
			else if (actionArray[actionCount] is ActionQuestionWithInput question)
			{
				HandleInputQuestion(question);
			}
			else if (actionArray[actionCount] is ActionBranchDialog branch)
			{
				HandleBranchingOfDialog(branch);
			}
			else if (actionArray[actionCount] is ActionChangeBackground backgroundChanger)
			{
				backgroundChanger.SwitchBackground();
				ExecuteNextLineOrActionOrReportDialogAsProcessed(backgroundChanger);
			}
			else if (actionArray[actionCount] is ActionPlaySFX sfxPlayer)
			{
				sfxPlayer.PlaySFX();
				ExecuteNextLineOrActionOrReportDialogAsProcessed(sfxPlayer);
			}
			else if (actionArray[actionCount] is ActionTriggerAnimation animationPlayer)
			{
				animationPlayer.TriggerAnimation();
				ExecuteNextLineOrActionOrReportDialogAsProcessed(animationPlayer);
			}
			else if (actionArray[actionCount] is ActionSceneLoad sceneLoader)
			{
				sceneLoader.LoadScene();
			}
			else if (actionArray[actionCount] is ActionEndDialog dialogEnder)
			{
				dM.DialogProcessed();
			}
			else if (actionArray[actionCount] is ActionStartOtherDialog otherDialogStarter)
			{
				dM.DialogProcessed();
				dM.StartDialogNumber(otherDialogStarter.dialogIndex);
			}
			else if (actionArray[actionCount] is ActionEvent unityEventAction)
			{
				unityEventAction.InvokeEvent();
				ExecuteNextLineOrActionOrReportDialogAsProcessed(unityEventAction);
			}
			else if (actionArray[actionCount] is ActionWait waiter)
			{
				if (isWaiting)
				{
					return;
				}
				isWaiting = true;	
				StartCoroutine(WaitCoroutine(waiter));
			}
		}

		public void SetPlayerIsSure()
		{
			isPlayerSure = true;
			ExecuteActions();
		}

		public void SetPlayerNotSure()
		{
			dM.HideAreYouSurePanel();
			dM.ShowInputField();
			dM.SetCursorInInputField();
		}

		public void SetDialogOptionA()
		{
			if (actionArray[actionCount] is ActionBranchDialog branch)
			{
				nextAction = branch.optionA;
				ExecuteActions();
			}
		}
		public void SetDialogOptionB()
		{
			if (actionArray[actionCount] is ActionBranchDialog branch)
			{
				nextAction = branch.optionB;
				ExecuteActions();
			}
		}

		#region NonCallableFunctions
		private void HandleNormalDialog(ActionDialog dialog)
		{
			DialogEntry currentLine = dialog.dialogList[lineCount];
			CheckForActionsOnLine(currentLine);
			//Speaker-Name suchen und im UI anzeigen, wenn neuer Dialog begonnen wird
			string speakerName = currentLine.speaker;
			string replacedSpeakerName = ReplaceVariablesInText(speakerName);
			dM.SetNameTextTo(replacedSpeakerName); 
			
			//Variablen in Textzeile ersetzen
			string text = currentLine.text; 
			string replacedText = ReplaceVariablesInText(text);
			dM.SetDialogTextTo(replacedText);

			ExecuteNextLineOrActionOrReportDialogAsProcessed(dialog);
		}

		private void HandleInputQuestion(ActionQuestionWithInput question)
		{   //Falls Spieler noch nicht gefragt wurde, Frage stellen
			if (!hasPlayerBeenAsked) 
			{
				AskQuestionAndShowInputField(question);
			}
			//Falls Spieler bereits gefragt wurde und Inputfeld nicht leer ist YesNoPanel je nach Einstellung anzeigen oder nicht
			else if (hasPlayerBeenAsked && dM.CheckIfInputFieldIsFilled()) 
			{
				HandleAreYouSurePanel(question);
			}
		}

		private void AskQuestionAndShowInputField(ActionQuestionWithInput question)
		{
			DialogEntry currentLine = question.dialogList[0];
			CheckForActionsOnLine(currentLine);

			//Speaker-Name setzen
			string speakerName = currentLine.speaker;
			string replacedSpeakerName = ReplaceVariablesInText(speakerName);
			dM.SetNameTextTo(replacedSpeakerName);
			//Textzeile setzen
			string text = currentLine.text; 
			dM.SetDialogTextTo(text);
			//InputFeld anzeigen und Cursor automatisch hinein setzen
			dM.ShowInputField();
			dM.SetCursorInInputField();

			if (!question.showAreYouSurePopUp)
			{
				dM.DesubsribeInputFieldEvent();
			}
			hasPlayerBeenAsked = true;
		}

		private void HandleAreYouSurePanel(ActionQuestionWithInput question)
		{   //YesNoPanel anzeigen falls im Inspector aktiviert
			if (question.showAreYouSurePopUp && !isPlayerSure)
			{   //Input speichern
				string var = dM.GetInputFromInputField();
				HandleVariableSave(question, var);
				//Variablen in Text ersetzen
				string text = question.popUpText;
				string replacedText = ReplaceVariablesInText(text);
				//QuestionPanel bef�llen und anzeigen
				dM.SetQuestionPopUpTextTo(replacedText);
				dM.HideInputField();
				dM.ShowAreYouSurePanel();
			}
			//Fortfahren, falls Spieler im Pop-Up best�tigt hat, dass er sich sicher mit der Eingabe ist.
			else if (question.showAreYouSurePopUp && isPlayerSure)
			{
				dM.HideAreYouSurePanel();
				dM.HideInputField();
				dM.EmptyInputField();
				hasPlayerBeenAsked = false;
				isPlayerSure = false;
				ExecuteNextLineOrActionOrReportDialogAsProcessed(question);
			}
			//YesNoPanel �berspringen, falls im Inspector deaktiviert
			else if (!question.showAreYouSurePopUp)
			{
				string var = dM.GetInputFromInputField();
				HandleVariableSave(question, var);
				dM.HideInputField();
				dM.EmptyInputField();
				hasPlayerBeenAsked = false;
				dM.SubscribeInputFieldEvent();
				ExecuteNextLineOrActionOrReportDialogAsProcessed(question);
			}
		}
		//Zeigt Antwortm�glichkeiten, wenn das erste mal gecallt.
		//Beim zweiten Mal springt es je nach Antwort zur im Inspector angegebenen Action.
		private void HandleBranchingOfDialog(ActionBranchDialog branch)
		{
			if (branch.optionA != null && branch.optionB != null)
			{
				if (!hasPlayerBeenAsked)
				{
					DialogEntry currentLine = branch.dialogList[0];
					CheckForActionsOnLine(currentLine);

					string speakerName = currentLine.speaker;
					string replacedSpeakerName = ReplaceVariablesInText(speakerName);
					dM.SetNameTextTo(replacedSpeakerName);
					//Variablen in Text ersetzen
					string text = currentLine.text;
					string replacedText = ReplaceVariablesInText(text);
					dM.SetDialogTextTo(replacedText);
					//Variablen in Button Texts setzen
					string textButtonA = branch.buttonTextA;
					string replacedButtonTextA = ReplaceVariablesInText(textButtonA);
					dM.SetButtonATextTo(replacedButtonTextA);

					string textButtonB = branch.buttonTextB;
					string replacedButtonTextB = ReplaceVariablesInText(textButtonB);
					dM.SetButtonBTextTo(replacedButtonTextB);

					dM.SetDialogPanelHeightToBig();
					dM.ShowDialogOptionsPanel();
					dM.HideContinueButton();
					hasPlayerBeenAsked = true;
				}
				else if (hasPlayerBeenAsked)
				{
					if (nextAction != null)
					{
						int i = System.Array.IndexOf(actionArray, nextAction);
						actionCount = i;
						nextAction = null;
					}
					dM.SetDialogPanelHeightToNormal();
					dM.HideDialogOptionsPanel();
					dM.ShowContinueButton();
					hasPlayerBeenAsked = false;
					ExecuteActions();
				}
			}
			else
			{
				Debug.Log("Option A and B need to be filled in ActionBranchDialog Skript");
			}
		}

		//Falls DialogLines �brig sind -> n�chste DialogZeile anzeigen
		//Falls Actions �brig sind -> n�chste Action ausf�hren
		//Falls nichts zutrifft -> Dialog als Processed an DialogManager melden
		private void ExecuteNextLineOrActionOrReportDialogAsProcessed(Action action)
		{
			if (action is ActionDialog dialog)
			{
				if (lineCount < dialog.dialogList.Count - 1)
				{
					lineCount++;
				}
				else if (lineCount == dialog.dialogList.Count - 1 && linesProcessed == false)
				{
					linesProcessed = true;

					if (actionCount < actionArray.Length - 1)
					{
						if (!dialog.jumpToAction)
						{
							actionCount++;
						}
						else if (dialog.jumpToAction)
						{
							nextAction = dialog.nextAction;
							actionCount = System.Array.IndexOf(actionArray, nextAction);
						}
						linesProcessed = false;
						lineCount = 0;
					}
				}
				else if (actionCount >= actionArray.Length - 1 && linesProcessed)
				{
					dM.DialogProcessed();
				}
			}
			else
			{
				if (actionCount < actionArray.Length - 1)
				{
					actionCount++;
					ExecuteActions();
				}
				else if (actionCount >= actionArray.Length - 1)
				{
					dM.DialogProcessed();
				}
			}

			
		}

		//Speichere Variable in Scriptable Objekt Dictionary
		private void HandleVariableSave(ActionQuestionWithInput question,string var)
		{
			//Im Inspector bestimmte VariableToSave als Key und PlayerInput als Value ins Dictionary setzen
			if (varDic.ContainsKey(question.variableToSaveTo))
			{
				varDic[question.variableToSaveTo] = var; 
			}
			else
			{
				Debug.LogWarning("Variable kann nicht gespeichert werden, da der Name nicht mit dem im Scriptable Object �bereinstimmt oder dieser Variablenname gar nicht erst erstellt wurde");
			}
		}
		//Ersetze Variablen durch RegularExpressions
		private string ReplaceVariablesInText(string text)
		{
			MatchCollection matchCollection = Regex.Matches(text, @"_\w+", RegexOptions.None); //Variablennamen aus Text filtern

			string replacedText = text;

			foreach (Match m in matchCollection)
			{
				bool isMatchVariableInDictionary = varDic.TryGetValue(m.Value, out string value); // mit herausgefilterter Variable als Key den Value im variablenDictionary suchen

				if (isMatchVariableInDictionary) 
				{
					replacedText = Regex.Replace(replacedText, m.Value, value); //herausgefilterte Variable im Text durch Variablen-Value ersetzen
				}
				else if(!isMatchVariableInDictionary)
				{
					Debug.LogWarning("Variable kann nicht genutzt werden, da der Name nicht mit dem im Scriptable Object �bereinstimmt oder dieser Variablenname gar nicht erst erstellt wurde");
				}
			}

			return replacedText;
		}


		private void CheckForActionsOnLine(DialogEntry currentLine)
		{
			if (currentLine.speakerImage != null)
			{
				dM.SetSpeakerImageTo(currentLine.speakerImage);
			}
			if (currentLine.backgroundImage != null)
			{
				BackGroundManager.Instance.SwitchBackGroundTo(currentLine.backgroundImage);
			}
			if (currentLine.objectToAnimate != "")
			{
				TriggerAnimation1(currentLine);
			}
			if (currentLine.object2ToAnimate != "")
			{
				TriggerAnimation2(currentLine);
			}
			if (currentLine.clip != null)
			{
				SFXManager.Instance.PlaySFX(currentLine.clip);
			}
			if (currentLine.clip2 != null)
			{
				SFXManager.Instance.PlaySFX(currentLine.clip2);
			}
		}

		private void TriggerAnimation1(DialogEntry currentLine)
		{
			if (currentLine.trigger != null)
			{
				dM.SetAnimatorTrigger(currentLine.objectToAnimate, currentLine.trigger, currentLine.animationSpeed);
			}
			else
			{
				Debug.LogWarning("String for AnimationTrigger is missing in line: " + currentLine.text);
			}
		}
		private void TriggerAnimation2(DialogEntry currentLine)
		{
			if (currentLine.trigger2 != null)
			{
				dM.SetAnimatorTrigger(currentLine.object2ToAnimate, currentLine.trigger2, currentLine.animation2Speed);
			}
			else
			{
				Debug.LogWarning("String for AnimationTrigger is missing in line: " + currentLine.text);
			}
		}

		IEnumerator WaitCoroutine(ActionWait waiter)
		{
			yield return new WaitForSeconds(waiter.seconds);
			ExecuteNextLineOrActionOrReportDialogAsProcessed(waiter);
			isWaiting = false;
		}
		#endregion NonCallableFunctions
	}


	
}