using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ZombiesCrowd : MonoBehaviour
{
	public Transform container;

	public float timeCrowd = 120f;

	public float wallRoom = 10f;

	public string sceneComplete;

	public Image imageTitle;

	public Text titleStart;

	public Text titleComplete;

	public Text numZombies;

	public GameObject[] switchingActive;

	private ManagerZombies[] managerZombies;

	private float[] startMinSpeed = new float[0];

	private Vector3 startPosition = default(Vector3);

	private float scaleTimer = 1f;

	private float currentTimerCrowd;

	private float markDistTimer;

	private Vector3 vect = default(Vector3);

	private float scale;

	private float timeScreen = 0.25f;

	private void OnDrawGizmos()
	{
		if (base.enabled)
		{
			Gizmos.color = new Color(0f, 1f, 0.5f);
			Vector3 position = base.transform.position;
			Gizmos.DrawLine(position, new Vector3(position.x + wallRoom, position.y + 3f, position.y));
			Gizmos.DrawLine(new Vector3(position.x + wallRoom, position.y - 2f, position.y), new Vector3(position.x + wallRoom, position.y + 5f, position.y));
		}
	}

	private void Start()
	{
		base.transform.SetParent(container);
		startPosition = base.transform.position;
		managerZombies = base.gameObject.GetComponents<ManagerZombies>();
		startMinSpeed = new float[managerZombies.Length];
		for (int i = 0; i < managerZombies.Length; i++)
		{
			startMinSpeed[i] = managerZombies[i].scaleSpeedMove;
		}
		InvokeRepeating("CheckAlive", 10f, 4f);
		StartScreenEmergence();
		SettingsStartGame.SetListenerDeathPlayer(OnDeathPlayer);
	}

	private void Update()
	{
		if (Time.timeScale != 0f)
		{
			currentTimerCrowd += Time.deltaTime / Time.timeScale;
			scaleTimer = Mathf.Clamp01((timeCrowd - currentTimerCrowd) / timeCrowd);
			float a = scaleTimer;
			Vector3 position = base.transform.position;
			scaleTimer = Mathf.Min(a, 1f - Mathf.Clamp01((position.x - startPosition.x) / wallRoom));
		}
		if (currentTimerCrowd - markDistTimer > 0.2f)
		{
			markDistTimer = currentTimerCrowd;
			for (int i = 0; i < managerZombies.Length; i++)
			{
				managerZombies[i].coefficient = scaleTimer;
				managerZombies[i].scaleSpeedMove = Mathf.Lerp(managerZombies[i].scaleSpeedMoveMax, startMinSpeed[i], scaleTimer);
			}
		}
	}

	public void CheckAlive()
	{
		if (numZombies != null)
		{
			numZombies.text = ManagerZombies.aliveSceneTotal.ToString();
		}
		if (ManagerZombies.aliveSceneTotal == 0)
		{
			titleComplete.gameObject.SetActive(value: true);
			titleStart.gameObject.SetActive(value: false);
			imageTitle.gameObject.SetActive(value: true);
			Image image = imageTitle;
			Color color = imageTitle.color;
			float r = color.r;
			Color color2 = imageTitle.color;
			float g = color2.g;
			Color color3 = imageTitle.color;
			image.color = new Color(r, g, color3.b, 0.7f);
			ZombiesInRoom.currentNum = string.Empty;
			SetActiveUI(value: false);
			CancelInvoke("CheckAlive");
			InvokeRepeating("DisposeTitle", 0f, 0.075f);
			Invoke("StopDisposeTitle", 0.3f);
			Invoke("LoadScene", 2.6f);
		}
	}

	public void OnDeathPlayer()
	{
		Invoke("LoadScene", 4f);
	}

	public void LoadScene()
	{
		if (imageTitle != null)
		{
			Image image = imageTitle;
			Color color = imageTitle.color;
			float r = color.r;
			Color color2 = imageTitle.color;
			float g = color2.g;
			Color color3 = imageTitle.color;
			image.color = new Color(r, g, color3.b, 0.9f);
		}
		SettingsStartGame.ResultGame(ManagerZombies.kills, Mathf.FloorToInt(Player.onStage.scores), Mathf.CeilToInt(Player.onStage.moneyHits));
		SettingsStartGame.GameOver(resetWeapon: false);
		SceneManager.LoadScene(sceneComplete);
	}

	public void DisposeTitle()
	{
		titleComplete.rectTransform.Rotate(0f, 0f, UnityEngine.Random.Range(1, 8) * 10);
		vect = titleComplete.rectTransform.localPosition;
		titleComplete.rectTransform.localPosition = new Vector3(UnityEngine.Random.Range(-120, 120), vect.y, vect.z);
		scale = UnityEngine.Random.value;
		titleComplete.rectTransform.localScale = new Vector3(Mathf.Lerp(0.5f, 1.5f, scale), Mathf.Lerp(4f, 0.6f, scale), 1f);
		titleStart.rectTransform.localPosition = titleComplete.rectTransform.localPosition;
		titleStart.rectTransform.rotation = titleComplete.rectTransform.rotation;
		titleStart.rectTransform.localScale = titleComplete.rectTransform.localScale;
	}

	public void StopDisposeTitle()
	{
		CancelInvoke("DisposeTitle");
		vect = titleComplete.rectTransform.localPosition;
		titleComplete.rectTransform.localPosition = new Vector3(0f, vect.y, vect.z);
		titleComplete.rectTransform.rotation = Quaternion.Euler(0f, 0f, 0f);
		titleComplete.rectTransform.localScale = new Vector3(1f, 1f, 1f);
		titleStart.rectTransform.localPosition = titleComplete.rectTransform.localPosition;
		titleStart.rectTransform.rotation = titleComplete.rectTransform.rotation;
		titleStart.rectTransform.localScale = titleComplete.rectTransform.localScale;
	}

	private void StartScreenEmergence()
	{
		imageTitle.enabled = true;
		imageTitle.raycastTarget = true;
		imageTitle.gameObject.SetActive(value: true);
		InvokeRepeating("ScreenEmergence", 1.5f, 0.05f);
		InvokeRepeating("DisposeTitle", 0f, 0.05f);
		Invoke("StopDisposeTitle", 0.25f);
		titleStart.gameObject.SetActive(value: true);
		titleComplete.gameObject.SetActive(value: false);
		SetActiveUI(value: false);
	}

	public void ScreenEmergence()
	{
		Image image = imageTitle;
		Color color = imageTitle.color;
		float r = color.r;
		Color color2 = imageTitle.color;
		float g = color2.g;
		Color color3 = imageTitle.color;
		float b = color3.b;
		Color color4 = imageTitle.color;
		image.color = new Color(r, g, b, color4.a - 0.05f / timeScreen);
		Color color5 = imageTitle.color;
		if (color5.a <= 0f)
		{
			imageTitle.gameObject.SetActive(value: false);
			CancelInvoke("ScreenEmergence");
			SetActiveUI(value: true);
		}
	}

	private void SetActiveUI(bool value)
	{
		for (int i = 0; i < switchingActive.Length; i++)
		{
			if (switchingActive[i] != null)
			{
				switchingActive[i].SetActive(value);
			}
		}
	}
}
