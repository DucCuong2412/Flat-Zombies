using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("PVold/SimpleLocalisation")]
public class SimpleLocalisation : MonoBehaviour
{
	public static bool english;

	[TextArea(2, 10)]
	public string russian;

	[TextArea(2, 10)]
	public string eng;

	private Text uiText;

	public static bool isRussia()
	{
		return (!english && Application.systemLanguage == SystemLanguage.Russian) || Application.systemLanguage == SystemLanguage.Ukrainian || Application.systemLanguage == SystemLanguage.Belarusian;
	}

	private void Awake()
	{
		uiText = base.gameObject.GetComponent<Text>();
		if (isRussia() && uiText.text.IndexOf(russian) == -1)
		{
			uiText.text = russian;
		}
		else if (!isRussia() && uiText.text != russian && uiText.text.IndexOf(russian) != -1)
		{
			uiText.text = uiText.text.Replace(russian, eng);
		}
		else if (!isRussia())
		{
			uiText.text = eng;
		}
		UnityEngine.Object.Destroy(this);
	}

	private void OnEnable()
	{
		Awake();
	}
}
