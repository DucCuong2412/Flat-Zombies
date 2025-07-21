using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GamePause : MonoBehaviour
{
	public Button resumeButton;

	public Button exitInMenu;

	public float timeFreeze = 2.5f;

	public string sceneMenu;

	public GameObject[] switchingActive;

	private float lastTimeScale;

	private bool lastStateListener = true;

	private float markTimeStart;

	private void Start()
	{
		resumeButton.onClick.AddListener(SwitchPause);
		exitInMenu.onClick.AddListener(LoadSceneMenu);
		markTimeStart = Time.time;
	}

	private void Update()
	{
		if (Player.onStage.isDead)
		{
			base.enabled = false;
		}
		else if (UnityEngine.Input.GetKeyUp(KeyCode.Menu) || UnityEngine.Input.GetKeyUp(KeyCode.Escape))
		{
			SwitchPause();
		}
	}

	private void OnApplicationFocus(bool hasFocus)
	{
		if (!hasFocus && base.enabled)
		{
			SwitchPause();
		}
	}

	private void OnApplicationPause(bool pauseStatus)
	{
		if (pauseStatus && base.enabled)
		{
			SwitchPause();
		}
	}

	private void OnDestroy()
	{
		Resume();
	}

	public void SwitchPause()
	{
		if (Time.time - markTimeStart <= timeFreeze)
		{
			return;
		}
		if (lastTimeScale == 0f && Time.timeScale != 0f)
		{
			lastStateListener = AudioListener.pause;
			lastTimeScale = Time.timeScale;
			Time.timeScale = 0f;
			AudioListener.pause = true;
			base.transform.SetAsLastSibling();
			UnityEngine.Debug.LogWarning("pause:300.400");
			base.enabled = false;
			for (int i = 0; i < switchingActive.Length; i++)
			{
				switchingActive[i].SetActive(!switchingActive[i].activeSelf);
			}
		}
		else if (lastTimeScale != 0f && Time.timeScale == 0f)
		{
			AudioListener.pause = lastStateListener;
			Time.timeScale = lastTimeScale;
			lastTimeScale = 0f;
			UnityEngine.Debug.LogWarning("resume:300");
			base.enabled = true;
			for (int j = 0; j < switchingActive.Length; j++)
			{
				switchingActive[j].SetActive(!switchingActive[j].activeSelf);
			}
		}
	}

	public void Resume()
	{
		if (lastTimeScale != 0f)
		{
			AudioListener.pause = lastStateListener;
			Time.timeScale = lastTimeScale;
			lastTimeScale = 0f;
			UnityEngine.Debug.LogWarning("resume:400");
		}
	}

	public void LoadSceneMenu()
	{
		Resume();
		PlayerPrefsFile.Save();
		SceneManager.LoadScene(sceneMenu);
	}

	public void Load(string sceneName)
	{
		Resume();
		PlayerPrefsFile.Save();
		SceneManager.LoadScene(sceneName);
	}

	public void Load(int sceneId)
	{
		Resume();
		PlayerPrefsFile.Save();
		SceneManager.LoadScene(sceneId);
	}

	public void ResetScene()
	{
		Resume();
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}
}
