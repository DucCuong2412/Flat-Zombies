using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsStartGame : MonoBehaviour
{
	[Serializable]
	public struct IconWeapon
	{
		public string name;

		public Sprite sprite;
	}

	public delegate void ListenerMethod();

	public const string NONE = "none";

	public static string gameMode = "none";

	public static int numLives = 0;

	public static string idFormHigscores = string.Empty;

	public static int currentCorridor = 0;

	public static int startLoadedCorridor = 0;

	public static int scores = 0;

	public static int timeLives = 0;

	public static int markStartTime = 0;

	public static int kills = 0;

	public static int deaths = 0;

	public static int allMoney = 0;

	public static int scoresBest = 0;

	public static bool pauseGame = false;

	private static ListenerMethod listenerDeathPlayer;

	public string nameGameMode = string.Empty;

	public string formHigscores = string.Empty;

	public string[] versions = new string[0];

	public int startNumLives;

	public int currentGameLevel;

	public Button startGameButton;

	public string sceneGameLevel;

	public Image imageMainWeapon;

	public Image imageBackupWeapon;

	public Text moneyDisplay;

	public Text gameLevelDisplay;

	public Text scoresDisplay;

	public Text scoresBestText;

	public Text healthPlayer;

	public Text numLivesText;

	public Text healthBarricade;

	public Text numZombies;

	public Text statusSaved;

	public Image[] showImageNumLives;

	private StoreWeapons storeWeapon;

	private bool saved;

	public static int NextCorridor()
	{
		currentCorridor++;
		BackgroundRandom.corridor++;
		StatisticsGame.UpdateMaxLevel(gameMode, currentCorridor);
		return currentCorridor;
	}

	public static int CurrentLevel(string nameLevel)
	{
		return PlayerPrefsFile.GetInt(nameLevel, -1);
	}

	public static void ApplicationFocus(bool hasFocus)
	{
		if (!(gameMode == "none"))
		{
			if (!hasFocus && !pauseGame)
			{
				numLives--;
				PlayerPrefsFile.SetInt(gameMode + "_numOfLives", numLives);
				pauseGame = true;
				PlayerPrefsFile.Save();
			}
			else if (hasFocus && pauseGame)
			{
				numLives++;
				PlayerPrefsFile.SetInt(gameMode + "_numOfLives", numLives);
				PlayerPrefsFile.Save();
				pauseGame = false;
			}
		}
	}

	public static void ResultGame(int kills, int scores, int money)
	{
		StoreWeapons.money += money;
		allMoney += money;
		SettingsStartGame.kills += kills;
		SettingsStartGame.scores += scores;
		scoresBest = Mathf.Max(scoresBest, SettingsStartGame.scores);
		StatisticsGame.UpdateData(SettingsStartGame.scores, StoreWeapons.money, SettingsStartGame.kills);
	}

	public static void OnDeathPlayer()
	{
		listenerDeathPlayer();
		listenerDeathPlayer = null;
	}

	public static void SetListenerDeathPlayer(ListenerMethod listener)
	{
		listenerDeathPlayer = listener;
	}

	public static void GameOver(bool resetWeapon)
	{
		numLives--;
		deaths++;
		if (gameMode != "none" && numLives <= 0)
		{
			PlayerPrefsFile.SetInt("attempt", 1 + PlayerPrefsFile.GetInt("attempt", 0));
			PlayerPrefsFile.SetInt(gameMode + "_scoresBest", scoresBest);
			timeLives += Mathf.FloorToInt(Time.time / Time.timeScale) - markStartTime;
			SendHighscores.ResetAddonInfo();
			SendHighscores.AddonInfo("version", PlayerPrefsFile.GetString(gameMode + "_version", "null"));
			SendHighscores.AddonInfo("idPlayer", PlayerPrefsFile.GetInt("idPlayer", -1));
			SendHighscores.AddonInfo("attempt", PlayerPrefsFile.GetInt("attempt", -1));
			SendHighscores.AddonInfo("mode", gameMode);
			SendHighscores.AddonInfo("time", GameResult.FullTime(timeLives, russian: false));
			SendHighscores.AddonInfo("kills", kills);
			SendHighscores.AddonInfo("money", allMoney);
			SendHighscores.AddonInfo("corridor", startLoadedCorridor.ToString() + "/" + currentCorridor.ToString());
			SendHighscores.AddonInfo("weapons", StoreWeapons.mainWeapon + "/" + StoreWeapons.backupWeapon);
			SendHighscores.AddonInfo("deaths", deaths);
			SendHighscores.AddonInfo("countStart", PlayerPrefsFile.GetInt(gameMode + "_countStart", -1));
			SendHighscores.GameOver(scores, idFormHigscores);
			GameResult.ResetAll();
			GameResult.SetParametr("scores", scores);
			GameResult.SetParametr("timeLives", GameResult.FullTime(timeLives));
			GameResult.SetParametr("kills", kills);
			GameResult.SetParametr(gameMode, currentCorridor);
			ResetSave(gameMode, resetWeapon, reset: true);
			currentCorridor = 0;
			scores = 0;
			BackgroundRandom.ResetSkin();
			gameMode = "none";
		}
	}

	public static void ResetSave(string gameMode, bool resetWeapon, bool reset)
	{
		if (reset)
		{
			if (resetWeapon)
			{
				StoreWeapons.Reset();
			}
			PlayerPrefsFile.DeleteKey(gameMode + "_numOfLives");
			PlayerPrefsFile.DeleteKey(gameMode + "_currentHealth");
			PlayerPrefsFile.DeleteKey(gameMode + "_totalDamageHealth");
			PlayerPrefsFile.DeleteKey(gameMode + "_barricadeHealth");
			PlayerPrefsFile.DeleteKey(gameMode + "_numZombies");
			PlayerPrefsFile.DeleteKey(gameMode + "_idGameLevel");
			PlayerPrefsFile.DeleteKey(gameMode + "_scores");
			PlayerPrefsFile.DeleteKey(gameMode + "_hash");
			PlayerPrefsFile.DeleteKey(gameMode + "_kills");
			PlayerPrefsFile.DeleteKey(gameMode + "_deaths");
			PlayerPrefsFile.DeleteKey(gameMode + "_allMoney");
			PlayerPrefsFile.DeleteKey(gameMode + "_timeLives");
			PlayerPrefsFile.DeleteKey(gameMode + "_version");
			PlayerPrefsFile.DeleteKey(gameMode + "_countStart");
			PlayerPrefsFile.DeleteKey("version");
			PlayerPrefsFile.Save();
		}
	}

	public static void AddLive()
	{
		numLives++;
		PlayerPrefsFile.SetInt(gameMode + "_numOfLives", numLives);
	}

	private void Awake()
	{
		storeWeapon = UnityEngine.Object.FindObjectOfType<StoreWeapons>();
		if (gameMode != nameGameMode)
		{
			numLives = PlayerPrefsFile.GetInt(nameGameMode + "_numOfLives", startNumLives);
			if (numLives <= 0 || numLives > startNumLives)
			{
				storeWeapon.ResetBuyed();
				ResetSave(nameGameMode, resetWeapon: true, reset: true);
				PlayerPrefsFile.Save();
			}
		}
		string text = PlayerPrefsFile.GetString("version", PlayerPrefsFile.GetString(nameGameMode + "_version", string.Empty));
		UnityEngine.Debug.Log("version:" + text);
		if (gameMode != nameGameMode)
		{
			numLives = PlayerPrefsFile.GetInt(nameGameMode + "_numOfLives", startNumLives);
			Player.currentHealth = PlayerPrefsFile.GetInt(nameGameMode + "_currentHealth", 0);
			gameMode = nameGameMode;
			idFormHigscores = formHigscores;
			currentCorridor = PlayerPrefsFile.GetInt(nameGameMode + "_idGameLevel", currentGameLevel);
			startLoadedCorridor = currentCorridor;
			scores = PlayerPrefsFile.GetInt(nameGameMode + "_scores", 0);
			kills = PlayerPrefsFile.GetInt(nameGameMode + "_kills", 0);
			deaths = PlayerPrefsFile.GetInt(nameGameMode + "_deaths", 0);
			timeLives = PlayerPrefsFile.GetInt(nameGameMode + "_timeLives", 0);
			allMoney = PlayerPrefsFile.GetInt(nameGameMode + "_allMoney", 0);
			scoresBest = PlayerPrefsFile.GetInt(nameGameMode + "_scoresBest", 0);
			ZombieBarricade.currentHealth = PlayerPrefsFile.GetFloat(nameGameMode + "_barricadeHealth", 100f);
			ZombiesInRoom.currentNum = PlayerPrefsFile.GetString(nameGameMode + "_numZombies", string.Empty);
			BackgroundRandom.corridor = currentCorridor;
			UnityEngine.Debug.Log("timeLives: " + GameResult.FullTime(timeLives));
			UnityEngine.Debug.Log("kills: " + kills);
			if (VersionIsOld(text))
			{
				storeWeapon.SetMoney(Mathf.Min(1200, storeWeapon.currentMoney));
				scores = 0;
			}
		}
		else
		{
			timeLives += Mathf.FloorToInt(Time.time / Time.timeScale) - markStartTime;
			PlayerPrefsFile.SetInt(nameGameMode + "_idGameLevel", currentCorridor);
			PlayerPrefsFile.SetInt(nameGameMode + "_numOfLives", numLives);
			PlayerPrefsFile.SetInt(nameGameMode + "_currentHealth", Player.currentHealth);
			PlayerPrefsFile.SetFloat(nameGameMode + "_barricadeHealth", ZombieBarricade.currentHealth);
			PlayerPrefsFile.SetString(nameGameMode + "_numZombies", ZombiesInRoom.currentNum);
			PlayerPrefsFile.SetInt(nameGameMode + "_kills", kills);
			PlayerPrefsFile.SetInt(nameGameMode + "_deaths", deaths);
			PlayerPrefsFile.SetInt(nameGameMode + "_timeLives", timeLives);
			PlayerPrefsFile.SetInt(nameGameMode + "_scores", scores);
			PlayerPrefsFile.SetInt(nameGameMode + "_scoresBest", scoresBest);
			PlayerPrefsFile.SetInt(nameGameMode + "_allMoney", allMoney);
			PlayerPrefsFile.SetInt("idPlayer", PlayerPrefsFile.GetInt("idPlayer", UnityEngine.Random.Range(10, 99999999)));
			if (text == string.Empty)
			{
				text = Application.version;
			}
			else if (text.IndexOf(Application.version) == -1)
			{
				text = text + ";" + Application.version;
			}
			PlayerPrefsFile.SetString(nameGameMode + "_version", text);
			DisplayStatusSave(PlayerPrefsFile.Save());
		}
		startGameButton.onClick.AddListener(LoadScene);
	}

	private void Start()
	{
	}

	private void OnEnable()
	{
		for (int i = 0; i < showImageNumLives.Length; i++)
		{
			showImageNumLives[i].gameObject.SetActive(i + 1 <= numLives);
		}
		if (numLives == 1 && showImageNumLives.Length != 0)
		{
			Image obj = showImageNumLives[0];
			Color color = showImageNumLives[0].color;
			obj.color = new Color(0.8f, 0f, 0f, color.a);
		}
		if (numLivesText != null)
		{
			numLivesText.text = numLives.ToString();
		}
		if (moneyDisplay != null)
		{
			moneyDisplay.text = storeWeapon.GetMoney();
		}
		if (gameLevelDisplay != null)
		{
			currentGameLevel = currentCorridor;
			gameLevelDisplay.text = currentGameLevel.ToString();
		}
		if (scoresDisplay != null)
		{
			scoresDisplay.text = scores.ToString();
		}
		if (scoresBestText != null)
		{
			scoresBestText.text = scoresBest.ToString();
		}
		if (healthPlayer != null && Player.currentHealth != 0)
		{
			healthPlayer.text = Player.currentHealth.ToString();
		}
		if (healthBarricade != null)
		{
			healthBarricade.text = Mathf.FloorToInt(ZombieBarricade.currentHealth).ToString();
		}
		if (numZombies != null)
		{
			numZombies.text = ZombiesInRoom.currentNum;
		}
		UnityEngine.Debug.Log("selectedMain:" + storeWeapon.selectedMain);
		UnityEngine.Debug.Log("selectedBackup:" + storeWeapon.selectedBackup);
		for (int j = 0; j < storeWeapon.weapons.Length; j++)
		{
			if (storeWeapon.weapons[j].id == storeWeapon.selectedMain)
			{
				imageMainWeapon.sprite = storeWeapon.weapons[j].icon;
				break;
			}
		}
		imageBackupWeapon.gameObject.SetActive(!string.IsNullOrEmpty(storeWeapon.selectedBackup) && storeWeapon.selectedBackup != storeWeapon.selectedMain);
		int num = 0;
		while (true)
		{
			if (num < storeWeapon.weapons.Length)
			{
				if (storeWeapon.weapons[num].id == storeWeapon.selectedBackup)
				{
					break;
				}
				num++;
				continue;
			}
			return;
		}
		imageBackupWeapon.sprite = storeWeapon.weapons[num].icon;
	}

	public void DisplayStatusSave(bool status)
	{
		saved = status;
		if (!statusSaved.gameObject.activeInHierarchy)
		{
			statusSaved.gameObject.SetActive(value: true);
			Invoke("DisplayStatusSave", 0.5f);
		}
	}

	public void DisplayStatusSave()
	{
		if (statusSaved.text == "✓")
		{
			statusSaved.gameObject.SetActive(value: false);
		}
		else if (saved)
		{
			statusSaved.text = "✓";
			statusSaved.fontSize *= 2;
			Invoke("DisplayStatusSave", 1.5f);
		}
		else
		{
			statusSaved.text = "Error 1486: Could not execute writing a file to your device";
		}
	}

	private void OnDestroy()
	{
		Player.ResetWeapon();
		markStartTime = Mathf.FloorToInt(Time.time / Time.timeScale);
		PlayerPrefsFile.Save();
	}

	public void ExitMainMenu(string scene)
	{
		numLives = 0;
		Player.currentHealth = 0;
		gameMode = "none";
		SceneManager.LoadScene(scene);
	}

	public void LoadScene()
	{
		PlayerPrefsFile.SetInt(nameGameMode + "_countStart", PlayerPrefsFile.GetInt(nameGameMode + "_countStart", 0) + 1);
		PlayerPrefsFile.Save();
		SceneManager.LoadScene(sceneGameLevel);
	}

	private bool VersionIsOld(string version)
	{
		if (string.IsNullOrEmpty(version))
		{
			return false;
		}
		bool flag = false;
		for (int i = 0; i < versions.Length; i++)
		{
			if (version.IndexOf(versions[i]) != -1)
			{
				flag = true;
				break;
			}
		}
		return !flag && versions.Length != 0;
	}

	private bool VersionIsOld(string[] versions)
	{
		for (int i = 0; i < versions.Length; i++)
		{
			if (VersionIsOld(versions[i]))
			{
				return true;
			}
		}
		return false;
	}
}
