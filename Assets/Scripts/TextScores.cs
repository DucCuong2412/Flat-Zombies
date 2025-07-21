using UnityEngine;
using UnityEngine.UI;

public class TextScores : MonoBehaviour
{
	private Text text;

	public Color colorAdd = new Color(1f, 1f, 1f, 1f);

	[HideInInspector]
	public int currentValue;

	private Color colorDefault;

	private float timer;

	private void Awake()
	{
		text = base.gameObject.GetComponent<Text>();
		colorDefault = text.color;
	}

	private void Start()
	{
	}

	private void FixedUpdate()
	{
		if (timer != 0f)
		{
			timer -= Time.fixedDeltaTime;
			if (timer < 0f)
			{
				timer = 0f;
			}
			text.color = new Color(Mathf.Lerp(colorDefault.r, colorAdd.r, timer), Mathf.Lerp(colorDefault.g, colorAdd.g, timer), Mathf.Lerp(colorDefault.b, colorAdd.b, timer));
		}
	}

	public void SetValue(int scores)
	{
		if (currentValue != scores)
		{
			currentValue = scores;
			text.color = colorAdd;
			timer = 1f;
		}
		text.text = currentValue.ToString();
	}
}
