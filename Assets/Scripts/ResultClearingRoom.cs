using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class ResultClearingRoom : MonoBehaviour, IInteractionArea, IEventSystemHandler
{
	public string sceneGameOver = string.Empty;

	public string testManager;

	public InteractionArea[] roomAreas = new InteractionArea[0];

	private GameObject selectedManager;

	private ManagerZombies[] managers = new ManagerZombies[1];

	private PlayerMove player;

	private int idCurrent;

	private bool playerIsDead;

	private InteractionArea area;

	private float startRadius;

	private float markTime;

	private float pause = 4f;

	private void Awake()
	{
		player = UnityEngine.Object.FindObjectOfType<PlayerMove>();
		managers[0] = UnityEngine.Object.FindObjectOfType<ManagerZombies>();
		managers = managers[0].transform.parent.gameObject.GetComponentsInChildren<ManagerZombies>(includeInactive: false);
		idCurrent = UnityEngine.Random.Range(0, managers.Length);
		idCurrent = PlayerPrefsFile.GetInt("selectedManagerRoom", idCurrent);
		idCurrent = Mathf.FloorToInt(Mathf.Repeat(idCurrent, managers.Length));
		selectedManager = managers[idCurrent].gameObject;
		for (int i = 0; i < managers.Length; i++)
		{
			if (managers[i].gameObject != selectedManager)
			{
				UnityEngine.Object.Destroy(managers[i].gameObject);
				managers[i] = null;
			}
		}
		testManager = selectedManager.name;
		SettingsStartGame.SetListenerDeathPlayer(OnDeathPlayer);
	}

	private void Start()
	{
		area = base.gameObject.GetComponent<InteractionArea>();
		startRadius = area.radius;
	}

	private void Update()
	{
		if (!(area == null))
		{
			if (ManagerZombies.aliveSceneTotal == 0 && !area.enabled)
			{
				area.enabled = true;
				SetActiveAreas(value: true);
				player.SwitchMoveMode(MoveModePlayer.FREE);
			}
			else if (ManagerZombies.aliveSceneTotal == 0 && area.radius != startRadius)
			{
				area.radius = Mathf.Lerp(0f, startRadius, Mathf.Clamp01((Time.time - markTime) / pause));
			}
			else if (ManagerZombies.aliveSceneTotal != 0 && area.enabled)
			{
				area.enabled = false;
				area.radius = 0f;
				SetActiveAreas(value: false);
				player.SwitchMoveMode(MoveModePlayer.AIM);
			}
			else if (ManagerZombies.aliveSceneTotal != 0)
			{
				markTime = Mathf.Floor(Time.time) + 2f;
			}
		}
	}

	public void SetActiveAreas(bool value)
	{
		for (int i = 0; i < roomAreas.Length; i++)
		{
			roomAreas[i].gameObject.SetActive(value);
		}
	}

	private void OnApplicationFocus(bool hasFocus)
	{
		SettingsStartGame.ApplicationFocus(hasFocus);
	}

	private void OnApplicationPause(bool pauseStatus)
	{
		SettingsStartGame.ApplicationFocus(!pauseStatus);
	}

	public void OnActiveArea()
	{
	}

	public void OnInactiveArea()
	{
	}

	public void OnActivationArea()
	{
	}

	public void OnDeathPlayer()
	{
		Invoke("LoadSceneGameOver", 4f);
		playerIsDead = true;
	}

	public void LoadSceneGameOver()
	{
		PlayerPrefsFile.DeleteKey("selectedManagerRoom");
		Door.ResetListBlocked();
		SettingsStartGame.ResultGame(ManagerZombies.kills, 0, Mathf.CeilToInt(Player.onStage.moneyHits));
		SettingsStartGame.GameOver(resetWeapon: false);
		SceneManager.LoadScene(sceneGameOver);
	}

	private void OnDestroy()
	{
		if (ManagerZombies.aliveSceneTotal == 0 && !playerIsDead)
		{
			PlayerPrefsFile.SetInt("selectedManagerRoom", idCurrent + 1);
			SettingsStartGame.ResultGame(ManagerZombies.kills, 1, Mathf.CeilToInt(Player.onStage.moneyHits));
			base.enabled = false;
		}
	}
}
