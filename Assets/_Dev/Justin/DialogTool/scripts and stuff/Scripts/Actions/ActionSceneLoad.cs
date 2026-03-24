
namespace Justin
{
	public class ActionSceneLoad : Action
	{
		public int specificSceneToLoad;
		public bool justLoadNextScene;


		public void LoadScene()
		{
			if (!justLoadNextScene)
			{
				SceneLoader.Instance.LoadSpecificLevel(specificSceneToLoad);
			}
			else
			{
				SceneLoader.Instance.LoadNextLevel();
			}
			
		}
	}
}

