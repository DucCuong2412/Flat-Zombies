using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Highscores : MonoBehaviour
{
	public const int VERSION = 1;

	public static int _scores = 0;

	public static string currentPlayerName = string.Empty;

	public static bool russian = true;

	[Tooltip("Идентификатор компонента для отображения списка")]
	public string ID = string.Empty;

	[Tooltip("Адрес скрипта для получения списка")]
	public string URL = "http://";

	[Tooltip("Идентификатор-название игры для базы данных")]
	public string game = "game";

	[Tooltip("За сколько дней извлечь список")]
	public int days;

	[Tooltip("Кол-во игроков на страницу")]
	public int amount = 10;

	[Space(8f)]
	[Tooltip("Текст для отображения информации о загрузке/отправке/ошибке")]
	public Text status;

	[Space(8f)]
	[Tooltip("Элемент списка игроков")]
	public ElementHighscores elementOfList;

	[Tooltip("Объект для отображения текущего игрока, если он не был найден в списке")]
	public ElementHighscores elementCurrentPlayer;

	[Tooltip("Контейнер с элементами списка, у которого будет изменен размер, будет умножен на кол-во")]
	public RectTransform containerList;

	private ElementHighscores[] list = new ElementHighscores[0];

	[Space(6f)]
	[Tooltip("Кнопка загрузить список")]
	public Button buttonLoad;

	[Tooltip("Кнопка показать список")]
	public Button buttonGroup;

	[Tooltip("Кнопка показать список")]
	public Button buttonShow;

	[Tooltip("Показать информацию во время загрузки, список или текст ")]
	public bool showOnLoad;

	[Space(6f)]
	[Tooltip("Когда загрузка удачно завершена")]
	public UnityEvent completedLoad = new UnityEvent();

	[Tooltip("Ошибка подключения/загрузки данных")]
	public UnityEvent errorLoad = new UnityEvent();

	private PlayerElement[] showPlayers = new PlayerElement[0];

	private TouchButtonPointer buttonPointer;

	private Highscores[] highscores = new Highscores[0];

	private GameObject newElementList;

	private string showTextServer;

	private WWWForm form;

	private WWW connect;

	private PlayerHighscores player;

	public static int scores
	{
		get
		{
			return _scores / 846;
		}
		set
		{
			_scores = value * 846;
		}
	}

	private void Awake()
	{
		russian = (russian && (Application.systemLanguage == SystemLanguage.Russian || Application.systemLanguage == SystemLanguage.Ukrainian || Application.systemLanguage == SystemLanguage.Belarusian));
		amount = Mathf.Max(1, amount);
		if (status != null)
		{
			status.text = string.Empty;
		}
		if (buttonLoad != null)
		{
			buttonLoad.onClick.AddListener(LoadList);
		}
		if (buttonShow != null)
		{
			buttonPointer = buttonShow.gameObject.GetComponent<TouchButtonPointer>();
			if (buttonPointer == null)
			{
				buttonPointer = buttonShow.gameObject.AddComponent<TouchButtonPointer>();
			}
			buttonPointer.pointerDown.baseEvent.AddListener(ShowAsInsert);
		}
		if (buttonGroup != null)
		{
			buttonPointer = buttonGroup.gameObject.GetComponent<TouchButtonPointer>();
			if (buttonPointer == null)
			{
				buttonPointer = buttonGroup.gameObject.AddComponent<TouchButtonPointer>();
			}
			buttonPointer.pointerDown.baseEvent.AddListener(ShowAsGroup);
		}
		buttonPointer = null;
	}

	private void Start()
	{
		ResetList();
	}

	private void OnEnable()
	{
	}

	public void LoadHighscores()
	{
		highscores = base.gameObject.GetComponents<Highscores>();
		for (int i = 0; i < highscores.Length; i++)
		{
			highscores[i].LoadList();
		}
	}

	public void LoadHighscores(string id)
	{
		highscores = base.gameObject.GetComponents<Highscores>();
		for (int i = 0; i < highscores.Length; i++)
		{
			if (highscores[i].ID == id)
			{
				highscores[i].LoadList();
			}
		}
	}

	public void ShowHighscores(string id)
	{
		highscores = base.gameObject.GetComponents<Highscores>();
		for (int i = 0; i < highscores.Length; i++)
		{
			if (highscores[i].ID == id)
			{
				highscores[i].Show();
			}
		}
	}

	public void ShowHighscores()
	{
		highscores = base.gameObject.GetComponents<Highscores>();
		for (int i = 0; i < highscores.Length; i++)
		{
			highscores[i].Show();
		}
	}

	public void ShowAsInsert()
	{
		if (buttonShow != buttonGroup && !buttonGroup.interactable)
		{
			Show();
		}
	}

	public void ShowAsGroup()
	{
		if (buttonShow != buttonGroup && !buttonShow.interactable)
		{
			Show();
		}
	}

	public void Show()
	{
		ResetList();
		if (showPlayers.Length != 0)
		{
			if (status != null)
			{
				status.text = string.Empty;
			}
			list = elementOfList.transform.parent.GetComponentsInChildren<ElementHighscores>(includeInactive: true);
			for (int i = list.Length; i < amount; i++)
			{
				newElementList = UnityEngine.Object.Instantiate(elementOfList.gameObject, elementOfList.transform.parent);
				newElementList.name = elementOfList.gameObject.name + "-" + i.ToString();
				Vector3 localPosition = elementOfList.GetTransform().localPosition;
				localPosition.y += (float)(-i) * elementOfList.GetTransform().rect.height;
				newElementList.transform.localPosition = localPosition;
			}
			list = elementOfList.transform.parent.GetComponentsInChildren<ElementHighscores>(includeInactive: true);
			if (elementCurrentPlayer != null && !string.IsNullOrEmpty(currentPlayerName) && scores > 0)
			{
				elementCurrentPlayer.SetFields(null, currentPlayerName, scores.ToString(), selected: true);
			}
			for (int j = 0; j < list.Length && j < showPlayers.Length; j++)
			{
				list[j].gameObject.SetActive(value: true);
				list[j].SetFields(showPlayers[j].position, showPlayers[j].name, showPlayers[j].scores.ToString());
				if (elementCurrentPlayer != null && showPlayers[j].name == currentPlayerName && showPlayers[j].scores == scores)
				{
					list[j].Selected(value: true);
					elementCurrentPlayer.Clear();
				}
			}
			if (containerList != null)
			{
				containerList.localPosition = Vector3.zero;
				containerList.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, showPlayers.Length * Mathf.FloorToInt(elementOfList.GetTransform().rect.height));
				containerList.gameObject.GetComponentInParent<ScrollRect>().verticalNormalizedPosition = 1f;
			}
		}
		else if (status != null)
		{
			status.text = showTextServer;
		}
		if (buttonShow != buttonGroup)
		{
			buttonGroup.interactable = false;
			buttonShow.interactable = false;
		}
		highscores = base.gameObject.GetComponents<Highscores>();
		for (int k = 0; k < highscores.Length; k++)
		{
			highscores[k].showOnLoad = (highscores[k] == this);
		}
	}

	public void LoadList()
	{
		ResetList();
		base.gameObject.SetActive(value: true);
		showTextServer = ((!russian) ? "Loading..." : "Загрузка...");
		showPlayers = new PlayerElement[0];
		if (showOnLoad)
		{
			Show();
		}
		form = new WWWForm();
		form.AddField("game", game);
		form.AddField("version", Application.version);
		form.AddField("days", days);
		form.AddField("version_comp", 1);
		form.AddField("amount", amount);
		form.AddField("russian", russian.ToString());
		StartCoroutine(GetText(form));
	}

	private IEnumerator GetText(WWWForm form)
	{
		UnityEngine.Debug.Log(ID + ".GetText()");
		connect = new WWW(URL, form);
		yield return connect;
		showTextServer = connect.text;
		player = new PlayerHighscores();
		if (!string.IsNullOrEmpty(connect.error) || showTextServer.IndexOf("players:") == -1)
		{
			UnityEngine.Debug.Log(ID + "/connect.error:" + connect.error + "\n" + showTextServer);
			errorLoad.Invoke();
			showTextServer = ((!russian) ? "Error. Server is not available\nCheck your internet connection" : "Ошибка. Сервер не доступен\nПроверьте подключение к интернету");
		}
		else
		{
			showTextServer = showTextServer.Replace("players:", string.Empty);
			if (showTextServer.IndexOf("\"names\":") != -1)
			{
				JsonUtility.FromJsonOverwrite(showTextServer, player);
				showPlayers = player.names;
				completedLoad.Invoke();
			}
			else
			{
				errorLoad.Invoke();
			}
		}
		showTextServer = ID + "\n" + showTextServer;
		if (showOnLoad)
		{
			Show();
		}
	}

	public void ResetList()
	{
		highscores = base.gameObject.GetComponents<Highscores>();
		for (int i = 0; i < highscores.Length; i++)
		{
			if (highscores[i].buttonGroup != null)
			{
				highscores[i].buttonGroup.interactable = true;
			}
			if (highscores[i].buttonShow != null)
			{
				highscores[i].buttonShow.interactable = true;
			}
		}
		if (elementCurrentPlayer != null)
		{
			elementCurrentPlayer.Clear();
		}
		if (elementOfList != null)
		{
			elementOfList.Clear();
			list = elementOfList.transform.parent.GetComponentsInChildren<ElementHighscores>();
			for (int j = 0; j < list.Length; j++)
			{
				list[j].Clear();
				list[j].gameObject.SetActive(value: false);
			}
		}
		if (containerList != null)
		{
			containerList.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 10f);
		}
	}
}
