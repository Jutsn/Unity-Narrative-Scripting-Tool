using Justin;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace Justin
{
	public class GameManager : MonoBehaviour
	{
		public static GameManager Instance;
		private void Awake()
		{
			if (Instance == null)
			{
				Instance = this;
				SceneManager.activeSceneChanged += OnActiveSceneChanged;
			}
			else if (Instance != null)
			{
				Destroy(gameObject);
			}
			DontDestroyOnLoad(gameObject);

			
		}

		void OnActiveSceneChanged(Scene previous, Scene next)
		{
			if (next.buildIndex == 0)
			{
				//initialize main menu
			}
			if (next.buildIndex == 1)
			{
				//Initialize scene 1
				StartCoroutine(StartDialog());

			}
			if (next.buildIndex == 2)
			{
				//Initialize scene 1
				StartCoroutine(StartDialog());

			}
			if (next.buildIndex == 3)
			{
				//Initialize scene 1
				StartCoroutine(StartDialog());

			}

		}
		IEnumerator StartDialog()
		{
			yield return new WaitForSeconds(1);
			DialogManager.Instance.StartDialogNumber(0);

		}
	}
}

