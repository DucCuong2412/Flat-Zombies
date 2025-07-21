using UnityEngine;
using UnityEngine.UI;

public class ListLastBestPlayers : MonoBehaviour
{
	public static bool restart;

	public static int currentGame;

	public Image imageIcon;

	public HighscoresIconGameMode[] games = new HighscoresIconGameMode[0];

	private Highscores highscores;

	private void Start()
	{
		if (!restart)
		{
			restart = true;
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		highscores = base.gameObject.GetComponent<Highscores>();
		highscores.completedLoad.AddListener(OnCompletedLoad);
		highscores.errorLoad.AddListener(OnErrorLoad);
		highscores.game = games[currentGame].game;
		highscores.LoadList();
		imageIcon.sprite = games[currentGame].icon;
		currentGame++;
		currentGame %= games.Length;
	}

	private void OnEnable()
	{
		if (highscores != null)
		{
			highscores.LoadList();
		}
	}

	public void OnCompletedLoad()
	{
		UnityEngine.Object.Destroy(highscores);
		UnityEngine.Object.Destroy(this);
	}

	public void OnErrorLoad()
	{
		UnityEngine.Object.Destroy(base.gameObject);
	}
}
