using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameResultVariable : MonoBehaviour
{
	public string nameVariable;

	public bool title;

	public Text text;

	public bool reset;

	public UnityEvent show;

	private void Start()
	{
		int num = 0;
		while (true)
		{
			if (num < GameResult.parameters.Length)
			{
				if (nameVariable == GameResult.parameters[num].name)
				{
					break;
				}
				num++;
				continue;
			}
			return;
		}
		if (!title)
		{
			text.text = string.Empty;
		}
		text.text = text.text + GameResult.parameters[num].value + GameResult.parameters[num].unit;
		show.Invoke();
		if (reset)
		{
			GameResult.parameters[num].Reset();
		}
	}
}
