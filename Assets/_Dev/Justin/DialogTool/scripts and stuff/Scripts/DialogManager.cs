using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Justin
{
	public class DialogManager : MonoBehaviour
	{
		public static DialogManager Instance;
		[SerializeField] public DialogVariablesSO variablesSO;
		[SerializeField] private TextMeshProUGUI nameText;
		[SerializeField] private TextMeshProUGUI dialogText;
		[SerializeField] private Image speakerImage;
		[SerializeField] private TextMeshProUGUI questionText;
		[SerializeField] private TMP_InputField inputField;
		[SerializeField] private RectTransform dialogPanelRect;
		[SerializeField] private GameObject continueButton;
		[SerializeField] private GameObject dialogPanel;
		[SerializeField] private GameObject areYouSurePanel;
		[SerializeField] private GameObject dialogBranchPanel;
		[SerializeField] private TextMeshProUGUI branchButtonAText;
		[SerializeField] private TextMeshProUGUI branchButtonBText;
		

		[SerializeField] private Dialog[] dialogList;
		[SerializeField] private Animator[] animatorList;
		[SerializeField] public UnityEvent[] unityEventList;

		bool allDialogsInstanciated = false;
		bool isProcessingDialog = false;
		int activeDialog;

		int dialogPanelNormalHeight = 130;
		int dialogPanelBigHeight = 220;
		#if UNITY_ANDROID
		int dialogPanelNormalHeightMobile = 238;
		int dialogPanelBigHeightMobile = 255;
		#endif

		private void Awake()
		{
			if (Instance == null)
			{
				Instance = this;
			}
			else if (Instance != null)
			{
				Destroy(gameObject);
			}
			
			for (int i = 0; i < dialogList.Length; i++)
			{
				Dialog d = Instantiate(dialogList[i]);
				d.gameObject.transform.SetParent(this.transform.Find("Dialogues"));
				dialogList[i] = d;
				allDialogsInstanciated = true;
			}
			SubscribeInputFieldEvent();
			#if UNITY_ANDROID
			dialogPanelNormalHeight = dialogPanelNormalHeightMobile;
			dialogPanelBigHeight = dialogPanelBigHeightMobile;
			#endif
		}

		public void StartDialogNumber(int i)
		{

			if (!isProcessingDialog && allDialogsInstanciated)
			{
				activeDialog = i;
				isProcessingDialog = true;
				dialogList[activeDialog].ResetDialog();
				dialogList[activeDialog].ExecuteActions();
			}
			else
			{
				Debug.LogWarning("Another Dialog is processed right now!");
			}
		}

		public void DialogProcessed()
		{
			HideContinueButton();
			HideDialogPanel();
			isProcessingDialog = false;
		}

		public void SetAnimatorTrigger(string objectToAnimate, string trigger, float animationSpeed)
		{
			bool animationPlayed = false;
			foreach (Animator a in animatorList)
			{
				if (a.gameObject.name == objectToAnimate)
				{
					animationPlayed = true;
					a.speed = animationSpeed;
					a.SetTrigger(trigger);
					break;
				}
			}
			if (!animationPlayed)
			{
				Debug.LogWarning("Object To Animate or Trigger not found in AnimatorList of DialogManager");
			}
		}

		public void InvokeUnityEvent(int eventNumber)
		{
			foreach(UnityEvent a in unityEventList)
			{
				if(a == unityEventList[eventNumber])
				{
					a.Invoke();
					
					break;
				}
			}
		}

		#region pure button functions

		//Callt ExecuteActions()-Function in aktuellem Dialog-Prefab 
		//UI-Buttons m�ssen entweder die ExecuteActionsForButtons-Funktion oder eine darunter liegende Set-Funktion callen.
		//InputFields d�rfen keine Funktion aus dem Inspector callen.Sie werden mit dem Skript gebunden.
		public void ExecuteActionsForButtons()
		{
			dialogList[activeDialog].ExecuteActions();
		}
		private void ExecuteActionsForInputFields(string text)
		{
			dialogList[activeDialog].ExecuteActions();
		}
		//F�r Buttons vom AreYouSure-PopUp
		public void SetPlayerIsSure()
		{
			dialogList[activeDialog].SetPlayerIsSure();
		}
		public void SetPlayerNotSure()
		{
			dialogList[activeDialog].SetPlayerNotSure();
		}
		//F�r Buttons vom DialogBranch-PopUp
		public void SetDialogOptionA()
		{
			dialogList[activeDialog].SetDialogOptionA();
		}
		public void SetDialogOptionB()
		{
			dialogList[activeDialog].SetDialogOptionB();
		}

		#endregion

		#region helper and UI functions for dialogues
		public Dictionary<string, string> GetVariablesDictionary()
		{
			return variablesSO.dic;
		}

		//Dialog Panel
		public void ShowDialogPanel()
		{
			dialogPanel.gameObject.SetActive(true);
		}
		public void HideDialogPanel()
		{
			dialogPanel.SetActive(false);
		}
		public void ShowContinueButton()
		{
			continueButton.gameObject.SetActive(true);
		}
		public void HideContinueButton()
		{
			continueButton.SetActive(false);
		}
		public void ShowSpeakerImage()
		{
			speakerImage.enabled = true;
		}
		public void HideSpeakerImage()
		{
			speakerImage.enabled = false;
		}

		
		//Dialog Branch Panel
		public void ShowDialogOptionsPanel()
		{
			dialogBranchPanel.SetActive(true);
		}
		public void HideDialogOptionsPanel()
		{
			dialogBranchPanel.SetActive(false);
		}
		public void SetDialogPanelHeightToNormal()
		{
			dialogPanelRect.sizeDelta = new Vector2(dialogPanelRect.sizeDelta.x, dialogPanelNormalHeight);
		}
		public void SetDialogPanelHeightToBig()
		{
			dialogPanelRect.sizeDelta = new Vector2(dialogPanelRect.sizeDelta.x, dialogPanelBigHeight);
		}

		//Input-Field
		public void ShowInputField()
		{
			inputField.gameObject.SetActive(true);
		}
		public void HideInputField()
		{
			inputField.gameObject.SetActive(false);
		}
		public void ShowAreYouSurePanel()
		{
			areYouSurePanel.SetActive(true);
		}
		public void HideAreYouSurePanel()
		{
			areYouSurePanel.SetActive(false);
		}
		public void SetCursorInInputField()
		{
			// Cursor ins Input-Fenster setzen
			inputField.OnPointerClick(new PointerEventData(EventSystem.current));
		}
		public bool CheckIfInputFieldIsFilled()
		{
			bool isFilled = inputField.text.Length > 0;
			return isFilled;
		}
		public string GetInputFromInputField()
		{
			return inputField.text;
		}
		public void EmptyInputField()
		{
			inputField.text = "";
		}
		public void DesubsribeInputFieldEvent()
		{
			inputField.onEndEdit.RemoveListener(ExecuteActionsForInputFields);
		}
		public void SubscribeInputFieldEvent()
		{
			inputField.onEndEdit.AddListener(ExecuteActionsForInputFields);
		}

		//Change Content
		public void SetNameTextTo(string speakerName)
		{
			nameText.text = speakerName;
		}
		public void SetSpeakerImageTo(Sprite speakerSprite)
		{
			speakerImage.sprite = speakerSprite;
		}
		public void SetDialogTextTo(string replacedText)
		{
			dialogText.text = replacedText;
		}
		public void SetQuestionPopUpTextTo(string replacedText)
		{
			questionText.text = replacedText;
		}
		public void SetButtonATextTo(string replacedText)
		{
			branchButtonAText.text = replacedText;
		}
		public void SetButtonBTextTo(string replacedText)
		{
			branchButtonBText.text = replacedText;
		}


		#endregion helper and UI functions for dialogues

		//void GetReferences()
		//{
		//	continueButton = GameObject.Find("Continue Button"); ;
		//	dialogPanel = GameObject.Find("DialogPanel");
		//	areYouSurePanel = GameObject.Find("AreYouSurePanel");
		//	dialogBranchPanel = GameObject.Find("DialogBranchPanel");
		//	nameText = GameObject.Find("Name Text").GetComponent<TextMeshProUGUI>();
		//	dialogText = GameObject.Find("Dialog Text").GetComponent<TextMeshProUGUI>();
		//	speakerImage = GameObject.Find("SpeakerImage").GetComponent<Image>();
		//	questionText = GameObject.Find("QuestionPanel").GetComponentInChildren<TextMeshProUGUI>(true);
		//	inputField = GameObject.Find("InputField").GetComponent<TMP_InputField>();
		//	dialogPanelRect = dialogPanel.GetComponent<RectTransform>();
		//	branchButtonAText = GameObject.Find("Option A Button").GetComponentInChildren<TextMeshProUGUI>(true); ;
		//	branchButtonBText = GameObject.Find("Option B Button").GetComponentInChildren<TextMeshProUGUI>(true); ;
		//}
	}
}
