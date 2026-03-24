using UnityEngine;

[CreateAssetMenu(fileName = "Unnamed Therapy Data SO", menuName = "Therapy Data SO")]
public class TherapyDataSO : ScriptableObject
{
	public bool breathingExerciseFinished;
	public int numberOfRepeatsBreathingExercise;
	public double averageInterval;
	public double averageDeviation;
}
