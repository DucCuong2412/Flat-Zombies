using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[AddComponentMenu("PVold/UI/LoadingGameObject")]
public class LoadingGameObject : MonoBehaviour
{
	public float timerStart = 0.25f;

	[Tooltip("Полоса загрузки")]
	public RectTransform progressBar;

	public GameObject objectComplete;

	public Text textCurrentObject;

	public int step = 1;

	public string openScene = "scene";

	public Object[] objects = new Object[0];

	private IScreenLoadingGame loadingObject;

	private float startSize;

	private float timerRunScene = 0.1f;

	private int currentID;

	private float percent;

	private GameObject children;

	private void Start()
	{
		Vector2 sizeDelta = progressBar.sizeDelta;
		startSize = sizeDelta.x;
		progressBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0f);
		objectComplete.SetActive(value: false);
		for (int i = 0; i < objects.Length; i++)
		{
			loadingObject = (objects[i] as IScreenLoadingGame);
			loadingObject.OnStartLoad(this);
		}
	}

	private void Update()
	{
		if (currentID < objects.Length)
		{
			loadingObject = (objects[currentID] as IScreenLoadingGame);
			if (loadingObject.OnStepLoad(this))
			{
				currentID++;
			}
			percent = currentID;
			percent /= objects.Length;
			progressBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Mathf.Floor(percent * startSize));
			return;
		}
		if (!objectComplete.activeSelf)
		{
			objectComplete.SetActive(value: true);
			textCurrentObject.gameObject.SetActive(value: false);
			progressBar.gameObject.SetActive(value: false);
		}
		timerRunScene -= Time.deltaTime;
		if (timerRunScene <= 0f)
		{
			base.enabled = false;
			UnityEngine.Debug.ClearDeveloperConsole();
			SceneManager.LoadScene(openScene);
		}
	}

	private void Complete()
	{
		for (int i = 0; i < progressBar.parent.childCount; i++)
		{
			children = progressBar.parent.GetChild(i).gameObject;
			children.SetActive(!children.activeSelf);
		}
		children = null;
	}
}
