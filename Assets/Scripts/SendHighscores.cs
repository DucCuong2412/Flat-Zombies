using System.Collections;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SendHighscores : MonoBehaviour
{
	public static string idForm = string.Empty;

	public static string showHigscoresScene = string.Empty;

	public static bool addonStringReset = false;

	public static string addonStringInfo = string.Empty;

	[Tooltip("Идентификатор компонента используемый для отправки результата")]
	public string id = string.Empty;

	[Tooltip("Адрес скрипта для отправки")]
	public string URL = "http://";

	[Header("POST-запрос")]
	[Tooltip("Идентификатор/название игры для базы данных")]
	public string game = "game";

	[Tooltip("Поле ввода с именем игрока")]
	public InputField playerName;

	[Tooltip("Кнопка для отправки")]
	public Button buttonSend;

	private Button buttonBackError;

	[Tooltip("Показать текущее кол-во очков")]
	public Text scoresDisplay;

	[Space(6f)]
	public GameObject screenFrom;

	public GameObject screenSending;

	public GameObject screenError;

	[Tooltip("Текст для отображения информации о загрузке/отправке")]
	public Text errorDisplay;

	[Space(6f)]
	[Tooltip("Список для загрузки после успешной отправки")]
	public Highscores highscores;

	[Tooltip("Идентификатор списка для показа после загрузки")]
	public string showHighscores;

	[Space(10f)]
	[Tooltip("Функции, вызываемые при ошибке")]
	public UnityEvent errorLoad = new UnityEvent();

	[Tooltip("Функции, вызываемые при успешной отправке")]
	public UnityEvent completedLoad = new UnityEvent();

	private int timeGame;

	private int idPlayer;

	private WWWForm form;

	private WWW connect;

	private string serverText;

	public static void AddonInfo(string name, string value)
	{
		if (addonStringReset)
		{
			ResetAddonInfo();
			addonStringReset = false;
		}
		value = value.Trim();
		name = name.Trim();
		if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(value))
		{
			name = name.Replace(",", ".");
			value = value.Replace(",", ".");
			string text = addonStringInfo;
			addonStringInfo = text + name + ": " + value + ",";
		}
	}

	public static void AddonInfo(string name, int value)
	{
		AddonInfo(name, value.ToString());
	}

	public static void AddonInfo(string name, float value)
	{
		AddonInfo(name, value.ToString());
	}

	public static void AddonInfo(string value)
	{
		addonStringInfo += value;
	}

	public static void ResetAddonInfo()
	{
		addonStringInfo = string.Empty;
	}

	public static void GameOver(int scores)
	{
		addonStringReset = true;
		idForm = string.Empty;
		Highscores.scores = scores;
	}

	public static void GameOver(int scores, string idForm)
	{
		addonStringReset = true;
		SendHighscores.idForm = idForm;
		Highscores.scores = scores;
	}

	private void Start()
	{
		timeGame = Mathf.FloorToInt(Time.time / Time.timeScale) + PlayerPrefs.GetInt("SendHighscores_TimeGame", 0);
		PlayerPrefs.SetInt("SendHighscores_TimeGame", timeGame);
		buttonSend.onClick.AddListener(SendResult);
		Highscores.currentPlayerName = PlayerPrefs.GetString("currentPlayerName", Highscores.currentPlayerName);
		playerName.text = Highscores.currentPlayerName;
		EnterName(playerName.text);
		playerName.onValueChanged.AddListener(EnterName);
		playerName.onValueChanged.Invoke(playerName.text);
		scoresDisplay.text = Highscores.scores.ToString();
		if (errorDisplay != null)
		{
			errorDisplay.text = string.Empty;
		}
		buttonBackError = screenError.GetComponentInChildren<Button>();
		buttonBackError.onClick.AddListener(ShowScreenForm);
		ShowScreenForm();
	}

	private void OnEnable()
	{
		if (!(id != idForm))
		{
			scoresDisplay.text = Highscores.scores.ToString();
			EnterName(playerName.text);
			ShowScreenForm();
		}
	}

	private void OnDestroy()
	{
		if (Highscores.currentPlayerName != null)
		{
			PlayerPrefs.SetString("currentPlayerName", Highscores.currentPlayerName);
		}
		if (timeGame == 0)
		{
			timeGame = Mathf.FloorToInt(Time.time / Time.timeScale) + PlayerPrefs.GetInt("SendHighscores_TimeGame", 0);
			PlayerPrefs.SetInt("SendHighscores_TimeGame", timeGame);
		}
		PlayerPrefs.Save();
	}

	public void EnterName(string value)
	{
		if (!(id != idForm))
		{
			buttonSend.interactable = (value.Length >= 3 && Highscores.scores > 0);
		}
	}

	public void SendResult()
	{
		if (!(id != idForm))
		{
			UnityEngine.Debug.Log("SendResult:" + id);
			screenFrom.SetActive(value: false);
			screenError.SetActive(value: false);
			screenSending.SetActive(value: true);
			Highscores.currentPlayerName = playerName.text;
			idPlayer = PlayerPrefs.GetInt("idPlayer", UnityEngine.Random.Range(2, 99999999));
			PlayerPrefs.SetInt("idPlayer", idPlayer);
			form = new WWWForm();
			form.AddField("game", game);
			form.AddField("version", Application.version);
			form.AddField("player", idPlayer);
			form.AddField("name", playerName.text);
			form.AddField("scores", Highscores.scores);
			form.AddField("time_game", timeGame);
			form.AddField("hash", GetHashMD5(game + Application.version + (timeGame + Highscores.scores + 9324).ToString()));
			form.AddField("russian", Highscores.russian.ToString());
			form.AddField("data", addonStringInfo);
			form.AddField("hash_data", GetHashMD5(game + Application.version + addonStringInfo));
			StartCoroutine(GetText(form));
		}
	}

	public void ShowScreenForm()
	{
		screenFrom.SetActive(value: true);
		screenError.SetActive(value: false);
		screenSending.SetActive(value: false);
	}

	public void ShowScreenError(string textError)
	{
		screenError.SetActive(value: true);
		screenFrom.SetActive(value: false);
		screenSending.SetActive(value: false);
		errorDisplay.text = textError;
	}

	private IEnumerator GetText(WWWForm form)
	{
		connect = new WWW(URL, form);
		yield return connect;
		serverText = connect.text;
		if (connect.error != null || serverText.IndexOf("players:") == -1)
		{
			UnityEngine.Debug.Log(connect.error);
			if (errorDisplay != null)
			{
				string textError = (!Highscores.russian) ? "Error. Server is not available" : "Ошибка. Сервер не доступен";
				ShowScreenError(textError);
				errorLoad.Invoke();
			}
			yield break;
		}
		serverText = serverText.Replace("players:", null);
		if (serverText.IndexOf("added") != -1)
		{
			buttonSend.interactable = false;
			buttonSend.gameObject.SetActive(value: false);
			playerName.readOnly = true;
			highscores.LoadHighscores();
			if (!string.IsNullOrEmpty(showHighscores))
			{
				highscores.ShowHighscores(showHighscores);
			}
			completedLoad.Invoke();
		}
		else if (errorDisplay != null)
		{
			ShowScreenError(serverText);
			errorLoad.Invoke();
		}
	}

	public string GetHashMD5(string text)
	{
		MD5CryptoServiceProvider mD5CryptoServiceProvider = new MD5CryptoServiceProvider();
		byte[] array = mD5CryptoServiceProvider.ComputeHash(Encoding.Default.GetBytes(text));
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < array.Length; i++)
		{
			stringBuilder.Append(array[i].ToString("x2"));
		}
		return stringBuilder.ToString();
	}
}
