using UnityEngine;
using UnityEngine.UI;

public class SendResultLater : MonoBehaviour
{
	[Tooltip("Кнопка для отправки запроса и удаление результата из памяти")]
	public Button buttonSend;

	[Tooltip("Кнопка под текстом ошибки, для сохранения результата в память")]
	public Button buttonSave;

	[Tooltip("Кнопка для повторной отправки после запуска игры")]
	public Button button;

	public SwitchUI switchScreen;

	public GameObject formSend;

	private int scores;

	private void Awake()
	{
		buttonSend.onClick.AddListener(DeleteResult);
		buttonSave.onClick.AddListener(SaveResult);
		button.onClick.AddListener(ShowResult);
		if (PlayerPrefsFile.HasKey("resultLater.idForm"))
		{
			button.gameObject.SetActive(value: true);
		}
		else
		{
			button.gameObject.SetActive(value: false);
		}
	}

	public void SaveResult()
	{
		button.gameObject.SetActive(value: true);
		PlayerPrefsFile.SetString("resultLater.idForm", SendHighscores.idForm);
		PlayerPrefsFile.SetString("resultLater.addonStringInfo", SendHighscores.addonStringInfo);
		PlayerPrefsFile.SetInt("resultLater.scores", Highscores.scores);
		if (Highscores.scores != scores)
		{
			scores = Highscores.scores;
			PlayerPrefsFile.Save();
		}
	}

	public void DeleteResult()
	{
		button.gameObject.SetActive(value: false);
		PlayerPrefsFile.DeleteKey("resultLater.idForm");
		PlayerPrefsFile.DeleteKey("resultLater.addonStringInfo");
		PlayerPrefsFile.DeleteKey("resultLater.scores");
	}

	public void ShowResult()
	{
		SendHighscores.ResetAddonInfo();
		SendHighscores.AddonInfo(PlayerPrefsFile.GetString("resultLater.addonStringInfo", string.Empty));
		SendHighscores.GameOver(PlayerPrefsFile.GetInt("resultLater.scores", 0), PlayerPrefsFile.GetString("resultLater.idForm", string.Empty));
		switchScreen.Show(formSend);
	}
}
