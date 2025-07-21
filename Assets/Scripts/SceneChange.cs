using UnityEngine;
using UnityEngine.Events;

public class SceneChange : MonoBehaviour
{
	public static string idNewScene;

	public string id = string.Empty;

	public UnityEvent change = new UnityEvent();

	public static void SetID(string id)
	{
		idNewScene = id;
	}

	private void Start()
	{
		if (id != string.Empty && id == idNewScene)
		{
			change.Invoke();
		}
	}
}
