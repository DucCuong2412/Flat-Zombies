using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameMenu : MonoBehaviour
{
	public static bool restartMainMenu;

	public UnityEvent restartScene;

	public Button quitGameButton;

	private void Start()
	{
		if ((bool)quitGameButton)
		{
			quitGameButton.onClick.AddListener(Quit);
		}
		if (restartMainMenu)
		{
			restartScene.Invoke();
		}
		restartMainMenu = true;
	}

	private void OnDestroy()
	{
		PlayerPrefsFile.Save();
	}

	public void Quit()
	{
		Application.Quit();
	}

	public void LoadScene(string sceneName)
	{
		SceneManager.LoadScene(sceneName);
	}

	public void LoadScene(int sceneId)
	{
		SceneManager.LoadScene(sceneId);
	}

	public void SceneReset()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}

	public void OpenURL(string url)
	{
		Application.OpenURL(url);
	}
}
