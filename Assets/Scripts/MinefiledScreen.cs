using UnityEngine;
using UnityEngine.UI;

public class MinefiledScreen : MonoBehaviour
{
	public static string currentGameMode = string.Empty;

	public string gameMode = string.Empty;

	public GameObject containerButtons;

	public StoreWeapons storeWeapon;

	public GameObject windowStore;

	public Text moneyStore;

	public Button closeStore;

	public MineOnScreen[] mines;

	public MineOfStore[] minesStore;

	private void Start()
	{
		CloseStore();
		UpdateTextMine();
		closeStore.onClick.AddListener(CloseStore);
		for (int i = 0; i < mines.Length && i < minesStore.Length; i++)
		{
			string id = mines[i].id;
			mines[i].buttonSelect.onClick.AddListener(delegate
			{
				SelectMine(id);
			});
			id = minesStore[i].id;
			minesStore[i].buttonBuy.onClick.AddListener(delegate
			{
				BuyMine(id);
			});
		}
	}

	public void UpdateTextMine()
	{
		for (int i = 0; i < mines.Length; i++)
		{
			mines[i].numAmount = PlayerPrefsFile.GetInt(gameMode + ".mine." + mines[i].id, 0);
			mines[i].textAmount.text = mines[i].numAmount.ToString();
		}
		for (int j = 0; j < minesStore.Length; j++)
		{
			minesStore[j].numAmount = PlayerPrefsFile.GetInt(gameMode + ".mine." + minesStore[j].id, 0);
			minesStore[j].textAmount.text = minesStore[j].numAmount.ToString();
		}
	}

	public void BuyMine(string id)
	{
		int num = 0;
		for (int i = 0; i < minesStore.Length; i++)
		{
			if (minesStore[i].id == id)
			{
				num = i;
				break;
			}
		}
		if (storeWeapon.Buy(minesStore[num].cost, moneyStore))
		{
			minesStore[num].numAmount++;
			PlayerPrefsFile.SetInt(gameMode + ".mine." + minesStore[num].id, minesStore[num].numAmount);
			UpdateTextMine();
		}
	}

	public void SelectMine(string id)
	{
		int num = 0;
		while (true)
		{
			if (num < mines.Length)
			{
				if (mines[num].id == id)
				{
					break;
				}
				num++;
				continue;
			}
			return;
		}
		if (mines[num].numAmount == 0)
		{
			ShowStore();
		}
	}

	public void ShowStore()
	{
		windowStore.SetActive(value: true);
		moneyStore.text = storeWeapon.GetMoney();
	}

	public void CloseStore()
	{
		windowStore.SetActive(value: false);
	}

	private int GetMineByID(int instanceID)
	{
		for (int i = 0; i < mines.Length; i++)
		{
			if (mines[i].buttonSelect.GetInstanceID() == instanceID)
			{
				return i;
			}
		}
		UnityEngine.Debug.LogWarning("GetMineByID(): мина не найдена, " + instanceID);
		return 0;
	}
}
