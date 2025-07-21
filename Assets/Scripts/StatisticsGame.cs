using UnityEngine;

public class StatisticsGame : MonoBehaviour
{
	public static int lastTime;

	public static void UpdateData(int scores, int money, int kills)
	{
		PlayerPrefsFile.SetInt("statisticMaxScores", Mathf.Max(scores, PlayerPrefsFile.GetInt("statisticMaxScores", 0)));
		PlayerPrefsFile.SetInt("statisticTotalMoney", money + PlayerPrefsFile.GetInt("statisticTotalMoney", 0));
		PlayerPrefsFile.SetInt("statisticTotalKills", kills + PlayerPrefsFile.GetInt("statisticTotalKills", 0));
		if (lastTime == 0)
		{
			lastTime = PlayerPrefsFile.GetInt("statisticTotalTime", 0);
		}
		PlayerPrefsFile.SetInt("statisticTotalTime", lastTime + Mathf.FloorToInt(Time.time / Time.timeScale));
	}

	public static void UpdateMaxLevel(string nameLevel, int currentValue)
	{
		PlayerPrefsFile.SetInt("statisticMax." + nameLevel, Mathf.Max(currentValue, PlayerPrefsFile.GetInt("statisticMax." + nameLevel, 0)));
	}

	public static int GetMaxScores()
	{
		return PlayerPrefsFile.GetInt("statisticMaxScores", 0);
	}

	public static int GetTotalMoney()
	{
		return PlayerPrefsFile.GetInt("statisticTotalMoney", 0);
	}

	public static int GetTotalKills()
	{
		return PlayerPrefsFile.GetInt("statisticTotalKills", 0);
	}

	public static int GetTotalTime()
	{
		return PlayerPrefsFile.GetInt("statisticTotalTime", 0);
	}

	public static int GetMaxRoom()
	{
		return PlayerPrefsFile.GetInt("statisticMaxRoom", 0);
	}

	public static string GetIdFavoriteWeapon(string gameMode)
	{
		return PlayerPrefsFile.GetString(gameMode + ".favoriteWeapon", string.Empty);
	}

	public static void ResetFavoriteWeapon(string gameMode)
	{
		PlayerPrefsFile.DeleteKey(gameMode + ".favoriteWeapon");
		PlayerPrefsFile.DeleteKey(gameMode + ".favoriteWeaponNum");
	}

	private void Start()
	{
	}
}
