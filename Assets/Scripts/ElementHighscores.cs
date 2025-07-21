using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class ElementHighscores : MonoBehaviour
{
	public Text positionPlayer;

	public Text namePlayer;

	public Text scores;

	public Graphic mark;

	private RectTransform rectTransform;

	public void Clear()
	{
		if (positionPlayer != null)
		{
			positionPlayer.text = string.Empty;
		}
		if (namePlayer != null)
		{
			namePlayer.text = string.Empty;
		}
		if (scores != null)
		{
			scores.text = string.Empty;
		}
		Selected(value: false);
	}

	public bool IsClear()
	{
		return string.IsNullOrEmpty(positionPlayer.text);
	}

	public void Selected(bool value)
	{
		if (mark != null)
		{
			mark.enabled = value;
		}
	}

	public void SetFields(string position, string name, string scores, bool selected = false)
	{
		if (positionPlayer != null)
		{
			positionPlayer.text = position;
		}
		if (namePlayer != null)
		{
			namePlayer.text = name;
		}
		if (this.scores != null)
		{
			this.scores.text = scores;
		}
		Selected(selected);
	}

	public void SetFields(int position, string name, string scores, bool selected = false)
	{
		SetFields(position.ToString() + ".", name, scores, selected);
	}

	public RectTransform GetTransform()
	{
		if (rectTransform == null)
		{
			rectTransform = GetComponent<RectTransform>();
		}
		return rectTransform;
	}
}
