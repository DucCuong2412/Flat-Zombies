using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ZombiesInRoom : MonoBehaviour
{
	public static readonly char[] separator = new char[1]
	{
		','
	};

	public static int[] leftTotal = new int[0];

	public Transform container;

	public int room;

	public int roomMax = 15;

	public int zombiesMax = 50;

	[Space(6f)]
	public float wallRoom = 10f;

	private Vector3 startPosition;

	public float timeCrowd = 120f;

	private float currentTimerCrowd;

	public string sceneReplay = string.Empty;

	public string sceneGameOver = string.Empty;

	public float timerComplete = 8f;

	public float timerDisplayText = 5f;

	public Image imageFade;

	public Text titleOfLevel;

	public Text numZombies;

	public Text titleOfComplete;

	public bool saveZombiesNum;

	public GameObject[] switchingActive;

	private ManagerZombies[] managerZombies;

	private float timer = -1f;

	private float speedHide = 0.3f;

	private Vector3 vec;

	public int totalNumZombies;

	private float colorAlpha;

	private Vector3 vect = default(Vector3);

	private float scale;

	public static string currentNum
	{
		get
		{
			if (leftTotal.Length == 0)
			{
				return string.Empty;
			}
			string text = string.Empty;
			for (int i = 0; i < leftTotal.Length; i++)
			{
				text = text + leftTotal[i].ToString() + separator[0].ToString();
			}
			return text.TrimEnd(separator);
		}
		set
		{
			if (string.IsNullOrEmpty(value))
			{
				leftTotal = new int[0];
				return;
			}
			string[] array = value.Split(separator);
			leftTotal = new int[array.Length];
			for (int i = 0; i < array.Length && !string.IsNullOrEmpty(array[i]); i++)
			{
				leftTotal[i] = int.Parse(array[i]);
			}
		}
	}

	private void OnDrawGizmos()
	{
		if (base.enabled)
		{
			Gizmos.color = new Color(0f, 1f, 0.5f);
			vec = base.transform.position;
			Gizmos.DrawLine(vec, new Vector3(vec.x + wallRoom, vec.y + 3f, vec.y));
			Gizmos.DrawLine(new Vector3(vec.x + wallRoom, vec.y - 2f, vec.y), new Vector3(vec.x + wallRoom, vec.y + 5f, vec.y));
		}
	}

	private void Awake()
	{
		if (SettingsStartGame.gameMode != "none")
		{
			room = SettingsStartGame.currentCorridor;
		}
		managerZombies = base.gameObject.GetComponents<ManagerZombies>();
		float num = room;
		float num2 = roomMax;
		float num3 = Mathf.Floor(num / num2 * 1000f) / 1000f;
		float num4 = 0f;
		zombiesMax += Mathf.Max(0, room - roomMax);
		int num5 = Mathf.Clamp(room - roomMax, 0, 30);
		UnityEngine.Debug.Log("ZombiesInRoom.limitOnStage:" + num5.ToString());
		for (int i = 0; i < num5; i++)
		{
			managerZombies[i % managerZombies.Length].limitOnStage++;
		}
		for (int j = 0; j < managerZombies.Length; j++)
		{
			num4 += (float)managerZombies[j].total;
		}
		for (int k = 0; k < managerZombies.Length; k++)
		{
			managerZombies[k].periodAdd = Mathf.Lerp(managerZombies[k].periodAdd, 0f, num3);
			managerZombies[k].periodAdd = Mathf.Floor(managerZombies[k].periodAdd * 100f) / 100f;
			managerZombies[k].timeStart = Mathf.Lerp(managerZombies[k].timeStart, 2f, num3);
			managerZombies[k].timeStart = Mathf.Floor(managerZombies[k].timeStart * 100f) / 100f;
			managerZombies[k].scaleSpeedMove = Mathf.Lerp(managerZombies[k].scaleSpeedMove, managerZombies[k].scaleSpeedMoveMax, num3);
			managerZombies[k].scaleSpeedMove = Mathf.Round(managerZombies[k].scaleSpeedMove * 100f) / 100f;
			managerZombies[k].addHealth = Mathf.Clamp(room - roomMax, 0, 10);
			managerZombies[k].total += Mathf.RoundToInt((float)managerZombies[k].total / num4 * (float)zombiesMax * num3);
			if (saveZombiesNum && leftTotal.Length == managerZombies.Length)
			{
				managerZombies[k].total = Mathf.CeilToInt(Mathf.Lerp(leftTotal[k], managerZombies[k].total, num3 * 0.8f));
			}
			totalNumZombies += managerZombies[k].total;
		}
	}

	private void Start()
	{
		if (container != null)
		{
			base.transform.SetParent(container);
		}
		imageFade.enabled = true;
		imageFade.raycastTarget = true;
		imageFade.gameObject.SetActive(value: true);
		titleOfComplete.raycastTarget = false;
		startPosition = base.transform.position;
		InvokeRepeating("UpdateTimer", 10f, 5f);
		InvokeRepeating("DisposeTitle", 0f, 0.05f);
		Invoke("StopDisposeTitle", 0.25f);
		InvokeRepeating("HideImage", 1.5f, 0.1f);
		titleOfLevel.gameObject.SetActive(value: true);
		titleOfLevel.text = (room + 1).ToString() + titleOfLevel.text;
		titleOfComplete.enabled = false;
		SettingsStartGame.SetListenerDeathPlayer(OnDeathPlayer);
	}

	private void OnApplicationFocus(bool hasFocus)
	{
		SettingsStartGame.ApplicationFocus(hasFocus);
	}

	private void OnApplicationPause(bool pauseStatus)
	{
		SettingsStartGame.ApplicationFocus(!pauseStatus);
	}

	private void FixedUpdate()
	{
		if (timer >= 0f)
		{
			if (timer > timerDisplayText && switchingActive.Length != 0 && switchingActive[0] != null)
			{
				for (int i = 0; i < switchingActive.Length; i++)
				{
					if (switchingActive[i] != null)
					{
						switchingActive[i].SetActive(!switchingActive[i].activeSelf);
						switchingActive[i] = null;
					}
				}
			}
			if (imageFade != null && timer > timerDisplayText && colorAlpha < 1f)
			{
				colorAlpha = Mathf.Floor(1000f * ((timer - timerDisplayText) / ((timerComplete - timerDisplayText) * 0.5f))) / 1000f;
				colorAlpha = Mathf.Abs(colorAlpha);
				colorAlpha = Mathf.Max(0.7f, colorAlpha);
				colorAlpha = Mathf.Min(colorAlpha, 1f);
				Image image = imageFade;
				Color color = imageFade.color;
				float r = color.r;
				Color color2 = imageFade.color;
				float g = color2.g;
				Color color3 = imageFade.color;
				image.color = new Color(r, g, color3.b, colorAlpha);
				if (!imageFade.gameObject.activeInHierarchy)
				{
					imageFade.gameObject.SetActive(value: true);
				}
			}
			timer += Time.fixedDeltaTime / Time.timeScale;
			if (timer > timerComplete)
			{
				timer = 0f;
				LoadSceneGameOver();
			}
		}
		currentTimerCrowd += Time.fixedDeltaTime;
		for (int j = 0; j < managerZombies.Length; j++)
		{
			if (wallRoom != 0f)
			{
				ManagerZombies obj = managerZombies[j];
				Vector3 position = base.transform.position;
				obj.coefficient = Mathf.Clamp01(1f - (position.x - startPosition.x) / wallRoom);
			}
			if (timeCrowd != 0f)
			{
				currentTimerCrowd = Mathf.Min(currentTimerCrowd, timeCrowd);
				managerZombies[j].coefficient = Mathf.Min(managerZombies[j].coefficient, (timeCrowd - currentTimerCrowd) / timeCrowd);
			}
		}
	}

	public void OnDeathPlayer()
	{
		Invoke("LoadSceneGameOver", 4f);
	}

	public void LoadSceneGameOver()
	{
		if (Player.onStage.isDead)
		{
			if (saveZombiesNum)
			{
				leftTotal = new int[managerZombies.Length];
				for (int i = 0; i < managerZombies.Length; i++)
				{
					leftTotal[i] = managerZombies[i].total + managerZombies[i].currentNumAlive;
				}
			}
			SettingsStartGame.ResultGame(ManagerZombies.kills, 0, Mathf.CeilToInt(Player.onStage.moneyHits));
			SettingsStartGame.GameOver(resetWeapon: true);
		}
		else
		{
			SettingsStartGame.NextCorridor();
			SettingsStartGame.ResultGame(ManagerZombies.kills, Mathf.FloorToInt(Player.onStage.scores), Mathf.CeilToInt(Player.onStage.moneyHits));
		}
		if (SettingsStartGame.numLives <= 0)
		{
			SceneManager.LoadScene(sceneGameOver);
		}
		else
		{
			SceneManager.LoadScene(sceneReplay);
		}
	}

	public void UpdateTimer()
	{
		int b = totalNumZombies - ManagerZombies.kills;
		b = Mathf.Max(0, b);
		if (numZombies != null)
		{
			numZombies.text = b.ToString();
		}
		if (b != 0)
		{
			return;
		}
		timer = 0f;
		for (int i = 0; i < managerZombies.Length; i++)
		{
			if (managerZombies[i].total != 0)
			{
				timer = -1f;
				break;
			}
		}
		if (timer == 0f)
		{
			CancelInvoke("UpdateTimer");
			InvokeRepeating("DisposeTitle", timerDisplayText, 0.15f);
			Invoke("StopDisposeTitle", timerDisplayText + 0.6f);
			titleOfComplete.enabled = true;
			titleOfLevel.gameObject.SetActive(value: false);
			imageFade.raycastTarget = false;
			Image image = imageFade;
			Color color = imageFade.color;
			float r = color.r;
			Color color2 = imageFade.color;
			float g = color2.g;
			Color color3 = imageFade.color;
			image.color = new Color(r, g, color3.b, 0.7f);
			currentNum = string.Empty;
		}
	}

	public void DisposeTitle()
	{
		titleOfComplete.rectTransform.Rotate(0f, 0f, UnityEngine.Random.Range(1, 8) * 10);
		vect = titleOfComplete.rectTransform.localPosition;
		titleOfComplete.rectTransform.localPosition = new Vector3(UnityEngine.Random.Range(-60, 60), vect.y, vect.z);
		scale = UnityEngine.Random.Range(50f, 250f) / 100f;
		titleOfComplete.rectTransform.localScale = new Vector3(scale, 2f / scale, 1f);
		titleOfLevel.rectTransform.localPosition = titleOfComplete.rectTransform.localPosition;
		titleOfLevel.rectTransform.rotation = titleOfComplete.rectTransform.rotation;
		titleOfLevel.rectTransform.localScale = titleOfComplete.rectTransform.localScale;
	}

	public void StopDisposeTitle()
	{
		CancelInvoke("DisposeTitle");
		vect = titleOfComplete.rectTransform.localPosition;
		titleOfComplete.rectTransform.localPosition = new Vector3(0f, vect.y, vect.z);
		titleOfComplete.rectTransform.rotation = Quaternion.Euler(0f, 0f, 0f);
		titleOfComplete.rectTransform.localScale = new Vector3(1f, 1f, 1f);
		titleOfLevel.rectTransform.localPosition = titleOfComplete.rectTransform.localPosition;
		titleOfLevel.rectTransform.rotation = titleOfComplete.rectTransform.rotation;
		titleOfLevel.rectTransform.localScale = titleOfComplete.rectTransform.localScale;
	}

	public void HideImage()
	{
		Image image = imageFade;
		Color color = imageFade.color;
		float r = color.r;
		Color color2 = imageFade.color;
		float g = color2.g;
		Color color3 = imageFade.color;
		float b = color3.b;
		Color color4 = imageFade.color;
		image.color = new Color(r, g, b, color4.a - speedHide);
		Color color5 = imageFade.color;
		if (color5.a <= 0f)
		{
			imageFade.gameObject.SetActive(value: false);
			CancelInvoke("HideImage");
		}
	}
}
