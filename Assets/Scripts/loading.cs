using UnityEngine;
using UnityEngine.SceneManagement;

public class loading : MonoBehaviour
{
	private void Start()
	{
		int @int = PlayerPrefs.GetInt("result_gdpr", 0);
		bool flag = @int != 0;
		UnityEngine.Debug.Log("Appodeal loading...");
		if (flag)
		{
			SceneManager.LoadScene("AppodealDemo");
		}
		else
		{
			SceneManager.LoadScene("GDPR");
		}
	}
}
