using UnityEngine;
using UnityEngine.UI;

public class DisplayMoney : MonoBehaviour
{
	private Text uiText;

	private int currentMoney;

	private void Awake()
	{
		uiText = base.gameObject.GetComponent<Text>();
		UpdateText();
	}

	public void UpdateText()
	{
		currentMoney = PlayerPrefsFile.GetInt("money", 0);
		uiText.text = "$" + currentMoney.ToString();
	}
}
