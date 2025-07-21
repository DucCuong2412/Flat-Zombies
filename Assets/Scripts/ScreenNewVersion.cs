using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ScreenNewVersion : MonoBehaviour
{
	public const string BEGIN = "version:";

	public const string SPR = ";";

	public static bool isShow;

	[Tooltip("Адрес скрипта для получения списка ")]
	public string URL = "http: //";

	[Tooltip("Идентификатор-название игры")]
	public string game = "game";

	[Space(8f)]
	[Tooltip("Текст для отображения новой версии")]
	public Text text;

	public string googlePlayURL = "http://";

	public GameObject window;

	public Button buttonOpenURL;

	public Button buttonOpenGoogle;

	public Button buttonClose;

	public UnityEvent show = new UnityEvent();

	public UnityEvent closed = new UnityEvent();

	private float startTimeScale;

	private WWWForm form;

	private string apkURL = string.Empty;

	private string apkVersion = string.Empty;

	private WWW connect;

	private void Start()
	{
		window.SetActive(value: false);
		startTimeScale = Time.timeScale;
		if (!isShow)
		{
			form = new WWWForm();
			form.AddField("game", game);
			form.AddField("version", Application.version);
			form.AddField("russian", (Application.systemLanguage == SystemLanguage.Russian || Application.systemLanguage == SystemLanguage.Ukrainian || Application.systemLanguage == SystemLanguage.Belarusian).ToString());
			StartCoroutine(GetText(form));
			buttonOpenURL.onClick.AddListener(OpenURL);
			buttonOpenGoogle.onClick.AddListener(OpenGooglePlay);
			buttonClose.onClick.AddListener(Close);
		}
	}

	private void OnDestroy()
	{
		UnityEngine.Object.Destroy(window);
	}

	private IEnumerator GetText(WWWForm form)
	{
		connect = new WWW(URL, form);
		yield return connect;
		if (string.IsNullOrEmpty(connect.error) && !string.IsNullOrEmpty(connect.text) && connect.text.StartsWith("version:"))
		{
			apkVersion = connect.text.Remove(connect.text.IndexOf(";"));
			apkVersion = apkVersion.Replace("version:", string.Empty);
			apkURL = connect.text.Remove(0, connect.text.IndexOf(";") + 1);
		}
		else if (!string.IsNullOrEmpty(connect.text))
		{
			UnityEngine.Debug.Log("ScreenNewVersion.error:" + connect.error + "\n" + connect.text);
		}
		if (apkVersion != apkURL && !string.IsNullOrEmpty(apkVersion))
		{
			Time.timeScale = 0f;
			AudioListener.pause = true;
			window.SetActive(value: true);
			window.transform.SetAsLastSibling();
			text.text = apkVersion;
			show.Invoke();
			isShow = true;
		}
		else
		{
			Close();
		}
	}

	public void Close()
	{
		if (window.activeSelf && Time.timeScale == 0f)
		{
			closed.Invoke();
			AudioListener.pause = false;
			Time.timeScale = startTimeScale;
		}
		UnityEngine.Object.Destroy(this);
	}

	public void OpenURL()
	{
		Application.OpenURL(apkURL);
	}

	public void OpenGooglePlay()
	{
		Application.OpenURL(googlePlayURL);
	}
}
