using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameModeHelp : MonoBehaviour
{
	public static string selectedMode = string.Empty;

	public Button buttonShow;

	public Image image;

	[Space(6f)]
	public Text title;

	public Text text;

	[Space(6f)]
	public Sprite screenshot;

	public string sceneName = string.Empty;

	public Button buttonPlay;

	private float audioVolume;

	private TouchButtonPointer buttonPointer;

	private GameModeHelp[] screenHelp = new GameModeHelp[0];

	private void Awake()
	{
		buttonPointer = buttonShow.gameObject.AddComponent<TouchButtonPointer>();
		buttonPointer.SetListenerPointerDown(Show);
		buttonPlay.onClick.AddListener(LoadScene);
		screenHelp = base.gameObject.GetComponents<GameModeHelp>();
	}

	private void Start()
	{
		if ((selectedMode == string.Empty && !buttonShow.interactable) || selectedMode == sceneName)
		{
			Show();
		}
	}

	private void OnEnable()
	{
		if (AudioListener.volume != 0f)
		{
			audioVolume = AudioListener.volume;
			AudioListener.volume = 0f;
		}
	}

	private void OnDisable()
	{
		if (audioVolume != 0f && AudioListener.volume != audioVolume)
		{
			AudioListener.volume = audioVolume;
		}
	}

	public void Show()
	{
		for (int i = 0; i < screenHelp.Length; i++)
		{
			screenHelp[i].Hide();
		}
		SetActiveComponents(value: true);
		image.sprite = screenshot;
		selectedMode = sceneName;
	}

	public void Show(TouchButtonPointer button)
	{
		Show();
	}

	public void LoadScene()
	{
		if (selectedMode == sceneName)
		{
			SceneManager.LoadScene(sceneName);
		}
	}

	public void Hide()
	{
		SetActiveComponents(value: false);
	}

	private void SetActiveComponents(bool value)
	{
		title.gameObject.SetActive(value);
		text.gameObject.SetActive(value);
		buttonShow.interactable = !value;
	}
}
