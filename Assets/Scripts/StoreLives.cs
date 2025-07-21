using UnityEngine;
using UnityEngine.UI;

public class StoreLives : MonoBehaviour
{
	public int cost;

	public Button buttonShow;

	public Button buttonClose;

	public StoreWeapons storeWeapons;

	public SwitchUI switchUI;

	public Text textMoney;

	public Color colorBuyingMark = new Color(0f, 1f, 0f, 1f);

	public Button[] buttons;

	private Text textCost;

	private void OnDrawGizmosSelected()
	{
		if (storeWeapons == null)
		{
			storeWeapons = UnityEngine.Object.FindObjectOfType<StoreWeapons>();
		}
	}

	private void Awake()
	{
		buttonClose.onClick.AddListener(switchUI.ShowDefault);
		buttonShow.onClick.AddListener(Show);
		if (storeWeapons == null)
		{
			UnityEngine.Debug.LogError("StoreLives: storeWeapons == NULL");
		}
	}

	private void Start()
	{
		for (int i = 0; i < buttons.Length; i++)
		{
			buttons[i].interactable = (i + 2 > SettingsStartGame.numLives);
			Button buttn = buttons[i];
			buttons[i].onClick.AddListener(delegate
			{
				Buy(buttn);
			});
		}
	}

	public void Show()
	{
		switchUI.Show(base.gameObject);
		textMoney.text = storeWeapons.GetMoney();
	}

	public void Buy(Button button)
	{
		textCost = button.gameObject.GetComponentInChildren<Text>();
		if (storeWeapons.Buy(cost))
		{
			SettingsStartGame.AddLive();
			button.enabled = false;
			button.targetGraphic.color = colorBuyingMark;
			button.enabled = false;
			textCost.enabled = false;
			textMoney.text = storeWeapons.GetMoney();
		}
		else
		{
			for (int i = 0; i < buttons.Length; i++)
			{
				buttons[i].interactable = false;
			}
		}
	}
}
