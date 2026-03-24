using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using TMPro;

namespace Justin
{
	public class APICommunication : MonoBehaviour
	{
		public bool sendData;
		public bool getData;
		public static APICommunication Instance;
		public static System.Action postToAPI;
		public TMP_InputField outputArea;
		public DialogVariablesSO variablesSO;
		private TherapyDataSO therapyDataSO;
		private void Awake()
		{
			if (Instance == null)
			{
				Instance = this;
				DontDestroyOnLoad(gameObject);
			}
			else
			{
				Destroy(gameObject);
			}
			
		}
		private void Start()
		{
			//GameObject.Find("PostButton").GetComponent<Button>().onClick.AddListener(() => PostData());
			therapyDataSO = Resources.Load<TherapyDataSO>("TherapyData");
		}
		public void GetData()
		{
			if (getData)
			{
				StartCoroutine(GetData_Coroutine());
			}
		}


		public void PostData()
		{
			if (sendData)
			{
				StartCoroutine(PostData_Coroutine());
			}
		}

		IEnumerator GetData_Coroutine()
		{
			outputArea.text = "Loading...";
			string url = "https://6925d62382b59600d725658f.mockapi.io/api/v1/user";
			using (UnityWebRequest request = UnityWebRequest.Get(url))
			{
				yield return request.SendWebRequest();
				if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
				{
					outputArea.text = request.error;
				}
				else
				{
					outputArea.text = request.downloadHandler.text;
				}
			}
		}

		IEnumerator PostData_Coroutine()
		{
			outputArea.text = "Loading...";
			string url = "https://6925d62382b59600d725658f.mockapi.io/api/v1/user";
			WWWForm form = new WWWForm();
			form.AddField("Name:", variablesSO.dic["_playerName"]);
			form.AddField("Atemübung beendet", therapyDataSO.breathingExerciseFinished.ToString());
			form.AddField("Anzahl der Wiederholungen der Atemübung", therapyDataSO.numberOfRepeatsBreathingExercise.ToString());
			form.AddField("averageInterval", therapyDataSO.averageInterval.ToString("F2"));
			form.AddField("averageDeviation", therapyDataSO.averageDeviation.ToString("F2"));
			using (UnityWebRequest request = UnityWebRequest.Post(url, form))
			{
				yield return request.SendWebRequest();
				if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
				{
					outputArea.text = request.error;
				}
				else
				{
					outputArea.text = request.downloadHandler.text;
				}
			}
		}



	}
}
