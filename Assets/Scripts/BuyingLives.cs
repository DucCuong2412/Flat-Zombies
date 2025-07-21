using UnityEngine;
using UnityEngine.UI;

public class BuyingLives : MonoBehaviour
{
	public static int numButtons;

	public StoreWeapons storeWeapons;

	public int cost;

	private int numBuyedLives;

	private Button button;

	private Text textCost;

	public Color colorBuyingMark = new Color(0f, 1f, 0f, 1f);

	private void Start()
	{
		textCost = base.gameObject.GetComponentInChildren<Text>();
		button = base.gameObject.GetComponent<Button>();
		button.onClick.AddListener(Buy);
		if (numButtons > SettingsStartGame.numLives)
		{
			button.interactable = false;
		}
		numButtons++;
	}

	public void Buy()
	{
		if (StoreWeapons.money >= cost)
		{
			storeWeapons.Buy(cost);
			SettingsStartGame.numLives++;
			button.enabled = false;
			button.targetGraphic.color = colorBuyingMark;
			textCost.enabled = false;
		}
		else
		{
			button.interactable = false;
		}
	}
}
