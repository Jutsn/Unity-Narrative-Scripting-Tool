using System;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using UnityEngine.UI;
using Unity.VisualScripting;

namespace Justin
{
    public class Atemuebung : MonoBehaviour
    {
        private TherapyDataSO therapyDataSO;
        public int maxClicks = 8;
        public float targetIntervalInSeconds = 3.0f;
        public float exerciseDuration = 25.0f;
        [SerializeField] TMP_Text debugText;
        [SerializeField] Animator animatorVinnie;
        [SerializeField] Animator animatorQuadrat;
        [SerializeField] bool firstClick = true;
        [SerializeField] bool exerciseStarted = false;
        [SerializeField] int breathingPhase = 0;
        [SerializeField] GameObject UIExercise1;
        [SerializeField] GameObject UIExercise2;
        [SerializeField] GameObject quadrat;
        [SerializeField] Button slowerButton;
        [SerializeField] Button fasterButton;
        [SerializeField] AudioClip beepSound;
        int clicksDetected = 0;
        float vinnieBreathingIntervall = 2.5f;

        double totalTime;
        double averageDeviation;
        double averageInterval;
        int numberOfExerciseRepeats = 0;

        private List<double> clickTimes = new List<double>();
        private List<double> timeIntervals = new List<double>();

		private void Start()
		{
			therapyDataSO = Resources.Load<TherapyDataSO>("TherapyData");
		}

		#region Exercise1

		public void StartExercise1()
		{
            ResetExercise1();
			firstClick = true;
			exerciseStarted = true;
			Debug.Log("Atemuebung gestartet");
			debugText.text = "Atemübung gestartet. Klicke " + maxClicks + " Mal in regelmässigen Abständen";
			UIExercise1.SetActive(true);
		}

		public void EndExercise1()
        {
			exerciseStarted = false;
			DialogManager.Instance.StartDialogNumber(1);
			Debug.Log("Atemuebung beendet");
			//extraUIElements.SetActive(false);
			animatorVinnie.SetTrigger("StopBreathing");
		}

        public void ResetExercise1()
        {
            clickTimes.Clear();
            totalTime = 0;
            clicksDetected = 0;
            firstClick = true;
            breathingPhase = 0;
            timeIntervals.Clear();
            averageInterval = 0;
            averageDeviation = 0;
			Debug.Log("Atemuebung zurückgesetzt");
        }

        public void DetectInput(InputAction.CallbackContext context)
        {
            if (context.performed && exerciseStarted)
            {
                clicksDetected += 1;
                Debug.Log(clicksDetected);

                StoreClicks();
            }
        }

        public void DetectButtonPress()
        {
            if (exerciseStarted && firstClick)
            {
				clicksDetected += 1;
                // Debug.Log(clicksDetected);

                breathingPhase += 1;
                animatorVinnie.SetInteger("BreathingPhase", breathingPhase);
                firstClick = false;
                StoreClicks();
            }
            else if (exerciseStarted && !firstClick)
            {
                clicksDetected += 1;
                // Debug.Log(clicksDetected);
                breathingPhase += 1;
                if (breathingPhase == 5)
                {
                    breathingPhase = 1;
                }
                animatorVinnie.SetInteger("BreathingPhase", breathingPhase);
                StoreClicks();
            }
        }

        private void StoreClicks()
        {
            clickTimes.Add(Time.time);

            if (clickTimes.Count >= 2)
            {
                double lastInterval = clickTimes[clickTimes.Count - 1] - clickTimes[clickTimes.Count - 2];
                totalTime += lastInterval;
                timeIntervals.Add(lastInterval);
                Debug.Log(clickTimes.Count);
                if (clickTimes.Count >= maxClicks)
                {
					numberOfExerciseRepeats++;
					AverageInterval();
                    AverageDeviation();
					ShowPerformanceInUI();
					StartCoroutine(SaveTherapyDataCoroutine());
                    EndExercise1();
                }
            }
        }

        private void AverageInterval()
        {
            if (timeIntervals.Count > 0)
            {
                averageInterval = timeIntervals.Average();
            }
        }

        private void AverageDeviation()
        {
            if (timeIntervals.Count > 0)
            {
                double average = timeIntervals.Average();
                double sumOfSquaresOfDifferences = timeIntervals.Select(val => (val - average) * (val - average)).Sum();
                averageDeviation = Math.Sqrt(sumOfSquaresOfDifferences / timeIntervals.Count);
            }
        }

        private void ShowPerformanceInUI()
        {
            Debug.Log("Average Interval: " + averageInterval.ToString("F2") + " Sekunden.");
            Debug.Log("Average Deviation: " + averageDeviation.ToString("F2") + " Sekunden.");
            debugText.text = "Average Interval: " + averageInterval.ToString("F2") + " Sekunden." + "\n" + "Average Deviation: " + averageDeviation.ToString("F2") + " Sekunden.";      
        }

        IEnumerator SaveTherapyDataCoroutine()
        {
            Debug.Log("Daten gesendet");
            therapyDataSO.breathingExerciseFinished = true; 
            therapyDataSO.numberOfRepeatsBreathingExercise = numberOfExerciseRepeats; 
            therapyDataSO.averageInterval = averageInterval;
			therapyDataSO.averageDeviation = averageDeviation;
            yield return null;

			APICommunication.Instance.PostData(); 
		}

        private void DebugClicks()
        {
            foreach (var click in clickTimes)
            {
                Debug.Log("Click Time: " + click.ToString("F2"));
            }
            foreach (var interval in timeIntervals)
            {
                Debug.Log("Interval: " + interval.ToString("F2"));
            }
        }
		#endregion Exercise 1

		#region Exercise 2
		public void StartExercise2()
		{
			animatorVinnie.speed = 0.8f;
			animatorQuadrat.speed = 0.8f;
			animatorVinnie.SetBool("exercise2", true);
			exerciseStarted = true;
			UIExercise1.SetActive(false);
			UIExercise2.SetActive(true);
            quadrat.SetActive(true);
			//animator.speed = (float)averageInterval / targetIntervalInSeconds;
			StartCoroutine(VinniAnimationCoroutine());
			StartCoroutine(WaitForContinueQuestionCoroutine());
		}

        public void EndExercise2()
        {
            StopAllCoroutines();
            breathingPhase = 0;
            animatorVinnie.SetTrigger("StopBreathing");
		}

		IEnumerator VinniAnimationCoroutine()
		{
            breathingPhase = 0;

			while (true)
            {
				breathingPhase += 1;
				animatorVinnie.SetInteger("BreathingPhase", breathingPhase);
				if (breathingPhase == 5)
				{
					breathingPhase = 1;
				}
				
				yield return new WaitForSeconds(vinnieBreathingIntervall);
			}
		}
		IEnumerator WaitForContinueQuestionCoroutine()
		{
            yield return new WaitForSeconds(exerciseDuration);
            EndExercise2();
			UIExercise2.SetActive(false);
			DialogManager.Instance.StartDialogNumber(2);
		}
		
		public void BreathFaster()
        {
            switch (vinnieBreathingIntervall)
            {
                case 3.0f:
					animatorVinnie.speed = 0.8f;
					animatorQuadrat.speed = 0.8f;
					vinnieBreathingIntervall = 2.5f;
                    slowerButton.interactable = true;
					
					break;
                case 2.5f:
					animatorVinnie.speed = 1f;
					animatorQuadrat.speed = 1f;
					vinnieBreathingIntervall = 2.0f;
					fasterButton.interactable = false;
					break;

                default:
                    break;
			}
            
		}
		public void BreathSlower()
		{
            switch (vinnieBreathingIntervall)
            {
                case 2.0f:
					animatorVinnie.speed = 0.8f;
					animatorQuadrat.speed = 0.8f;
					vinnieBreathingIntervall = 2.5f;
					fasterButton.interactable = true;
					break;

				case 2.5f:
					animatorVinnie.speed = 0.666f;
					animatorQuadrat.speed = 0.666f;
					vinnieBreathingIntervall = 3.0f;
					slowerButton.interactable = false;
					break;

				default:
                    break;
            }
            
		}

		#endregion Exercise 2

		#region Exercise 3
		public void StartExercise3()
		{
			exerciseStarted = true;
			UIExercise1.SetActive(false);
			UIExercise2.SetActive(false);
			
			//animator.speed = (float)averageInterval / targetIntervalInSeconds;
			StartCoroutine(BeepHandlerCoroutine());
			StartCoroutine(WaitForFinishQuestionCoroutine());
		}

		public void EndExercise3()
		{
			StopAllCoroutines();
			SFXManager.Instance.StopAllSFX();
		}

		IEnumerator BeepHandlerCoroutine()
		{
			while (true)
			{
                SFXManager.Instance.PlaySFX(beepSound);
				yield return new WaitForSeconds(vinnieBreathingIntervall);
			}
		}

		IEnumerator WaitForFinishQuestionCoroutine()
		{
			yield return new WaitForSeconds(exerciseDuration);
            EndExercise3();
			DialogManager.Instance.StartDialogNumber(3);
		}
		#endregion Exercise 3
	}
}


