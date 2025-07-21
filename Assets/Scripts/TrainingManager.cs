using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TrainingManager : MonoBehaviour
{
	public static TrainingManagerSettings settings = new TrainingManagerSettings(-1f, -1);

	public static int markTimeGame = 0;

	public ManagerZombies manager;

	public string sceneGameOver = string.Empty;

	[Space(6f)]
	[Tooltip("Кол-во зомби на сцене")]
	public Slider limitOnStage;

	[Tooltip("Временной промежуток для добавления")]
	public Slider periodAdd;

	public string sceneStart = string.Empty;

	public Button startButton;

	public string sceneMainMenu = string.Empty;

	public Button mainMenuButton;

	public Image[] icons = new Image[0];

	private Vector2[] startPositionIcons = new Vector2[0];

	private void Start()
	{
		if (!string.IsNullOrEmpty(sceneGameOver))
		{
			SettingsStartGame.SetListenerDeathPlayer(OnDeathPlayer);
		}
		if (manager != null && settings.limitOnStage >= 0)
		{
			manager.periodAdd = settings.periodAdd;
			manager.SetLimitOnStage(Mathf.FloorToInt(settings.limitOnStage));
			return;
		}
		if (PlayerPrefsFile.HasKey("training.limitOnStage"))
		{
			settings.periodAdd = PlayerPrefsFile.GetFloat("training.periodAdd", settings.periodAdd);
			settings.limitOnStage = PlayerPrefsFile.GetInt("training.limitOnStage", settings.limitOnStage);
		}
		if (limitOnStage != null && periodAdd != null)
		{
			limitOnStage.value = settings.limitOnStage;
			limitOnStage.onValueChanged.AddListener(ChangeLimitStage);
			periodAdd.value = settings.periodAdd;
			periodAdd.onValueChanged.AddListener(ChangedPeriodAdd);
			startButton.onClick.AddListener(LoadSceneStart);
			mainMenuButton.onClick.AddListener(LoadSceneMenu);
			Invoke("UpdateValueSlider", 0.1f);
		}
	}

	public void UpdateValueSlider()
	{
		limitOnStage.onValueChanged.Invoke(limitOnStage.value);
		periodAdd.onValueChanged.Invoke(periodAdd.value);
	}

	public void ChangeLimitStage(float valueSlider)
	{
		for (int i = 0; i < icons.Length; i++)
		{
			icons[i].gameObject.SetActive((float)i < valueSlider);
		}
	}

	public void ChangedPeriodAdd(float valueSlider)
	{
		if (startPositionIcons.Length != icons.Length)
		{
			startPositionIcons = new Vector2[icons.Length];
			for (int i = 0; i < startPositionIcons.Length; i++)
			{
				startPositionIcons[i] = icons[i].rectTransform.anchoredPosition;
			}
		}
		valueSlider += 0.1f * periodAdd.maxValue;
		for (int j = 0; j < icons.Length; j++)
		{
			icons[j].rectTransform.anchoredPosition = Vector2.Lerp(icons[0].rectTransform.anchoredPosition, startPositionIcons[j], valueSlider / periodAdd.maxValue);
		}
	}

	public void LoadSceneStart()
	{
		SceneManager.LoadScene(sceneStart);
	}

	public void LoadSceneMenu()
	{
		SceneManager.LoadScene(sceneMainMenu);
	}

	public void OnDeathPlayer()
	{
		Invoke("LoadSceneGameOver", 4f);
	}

	public void LoadSceneGameOver()
	{
		SceneManager.LoadScene(sceneGameOver);
	}

	private void OnDestroy()
	{
		if (manager != null)
		{
			GameResult.SetParametr("scoresTraining", Mathf.FloorToInt(Player.onStage.scores));
			GameResult.SetParametr("timeTraining", GameResult.FullTime(Mathf.FloorToInt(Time.time / Time.timeScale) - markTimeGame));
			GameResult.SetParametr("kills", ManagerZombies.kills.ToString());
		}
		else if (limitOnStage != null)
		{
			settings.periodAdd = periodAdd.value;
			settings.limitOnStage = Mathf.FloorToInt(limitOnStage.value);
			markTimeGame = Mathf.FloorToInt(Time.time / Time.timeScale);
			PlayerPrefsFile.SetFloat("training.periodAdd", settings.periodAdd);
			PlayerPrefsFile.SetInt("training.limitOnStage", settings.limitOnStage);
			PlayerPrefsFile.Save();
		}
	}
}
