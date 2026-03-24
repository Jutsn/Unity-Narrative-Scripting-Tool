using NUnit.Framework;
using Unity.Hierarchy;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BackGroundManager : MonoBehaviour
{
	public static BackGroundManager Instance;
    public Sprite[] backgroundSprites;
    private Image backgroundImage;
    private int currentBackgroundIndex;

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

		backgroundImage = transform.GetChild(0).GetChild(0).GetComponent<Image>();

		currentBackgroundIndex = 0;
		backgroundImage.sprite = backgroundSprites[currentBackgroundIndex];
	}
	public void SwitchBackGroundTo(Sprite sprite)
    {
        backgroundImage.sprite = sprite;
	}
    public void ShowNextBackground()
    {
		currentBackgroundIndex++;
		backgroundImage.sprite = backgroundSprites[currentBackgroundIndex];
	}
}
