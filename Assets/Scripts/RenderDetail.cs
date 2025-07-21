using UnityEngine;

public class RenderDetail : MonoBehaviour
{
	public const string ALL = "allDisabled";

	public static string tagDisabled = string.Empty;

	public string tagRender = string.Empty;

	public static void Disable(string tagRender)
	{
		tagDisabled = tagRender;
	}

	public static void DisableAll()
	{
		tagDisabled = "allDisabled";
	}

	public static void EnableAll()
	{
		tagDisabled = string.Empty;
	}

	private void Awake()
	{
		if (tagDisabled != string.Empty && tagDisabled == tagRender)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
		else if (tagDisabled == "allDisabled")
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
		else
		{
			UnityEngine.Object.Destroy(this);
		}
	}
}
