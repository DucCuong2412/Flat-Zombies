using UnityEngine;
using UnityEngine.UI;

public class StoreWeapons : MonoBehaviour
{
	public static int moneyHash = 245;

	public static string id = string.Empty;

	public static string defaultMainWeapon = string.Empty;

	public static string defaultBackupWeapon = string.Empty;

	public static string mainWeapon = string.Empty;

	public static string backupWeapon = string.Empty;

	public static string[] keysWeapons = new string[0];

	public string idStore;

	public Text moneyDisplay;

	public string mainDefault;

	public string backupDefault;

	public WeaponOfStore[] disabledWeapons = new WeaponOfStore[0];

	public WeaponCategory[] weaponCategory = new WeaponCategory[0];

	public RectTransform container;

	public GridLayoutGroup grid;

	public int testCost;

	public Button buttonShow;

	public Button startGame;

	public SwitchUI switchUI;

	public Button buttonClose;

	[Space(8f)]
	public GameObject screenSelect;

	public Text textNameWpn;

	public Text textPropertiesWpn;

	public Image iconNewWpn;

	public Image iconMainWeapon;

	public Image iconBackupWeapon;

	public Button buttonMainWpn;

	public Button buttonBackupWpn;

	public Button buttonCloseSelect;

	[HideInInspector]
	public WeaponOfStore[] weapons = new WeaponOfStore[0];

	private WeaponOfStore selectedWeapon;

	private TouchButtonPointer buttonPointer;

	private bool isLoaded;

	private int numInvoke;

	private Text moneySignal;

	public static int money
	{
		get
		{
			return moneyHash - 245;
		}
		set
		{
			moneyHash = value + 245;
		}
	}

	public static StoreWeapons current => UnityEngine.Object.FindObjectOfType<StoreWeapons>();

	public string selectedMain
	{
		get
		{
			Load();
			return mainDefault;
		}
		set
		{
			mainDefault = value;
		}
	}

	public string selectedBackup
	{
		get
		{
			Load();
			return backupDefault;
		}
		set
		{
			backupDefault = value;
		}
	}

	public int currentMoney
	{
		get
		{
			Load();
			return money;
		}
	}

	public static void Reset()
	{
		mainWeapon = string.Empty;
		backupWeapon = string.Empty;
		for (int i = 0; i < keysWeapons.Length; i++)
		{
			PlayerPrefsFile.DeleteKey(keysWeapons[i]);
		}
		money = 0;
		PlayerPrefsFile.DeleteKey(id + "money");
	}

	public static void AddMoney(int money)
	{
		StoreWeapons.money += money;
	}

	public void Load()
	{
		if (isLoaded)
		{
			return;
		}
		isLoaded = true;
		if (id != idStore)
		{
			mainWeapon = string.Empty;
			backupWeapon = string.Empty;
			money = PlayerPrefsFile.GetInt(idStore + "money", 0);
			id = idStore;
		}
		else
		{
			if (mainWeapon != string.Empty)
			{
				mainDefault = mainWeapon;
			}
			if (backupWeapon != string.Empty)
			{
				backupDefault = backupWeapon;
			}
		}
		for (int i = 0; i < disabledWeapons.Length; i++)
		{
			disabledWeapons[i].gameObject.SetActive(value: false);
			disabledWeapons[i].enabled = false;
			UnityEngine.Object.Destroy(disabledWeapons[i].gameObject);
		}
		disabledWeapons = new WeaponOfStore[0];
		weapons = base.gameObject.GetComponentsInChildren<WeaponOfStore>();
		keysWeapons = new string[weapons.Length];
		for (int j = 0; j < weapons.Length; j++)
		{
			weapons[j].SetAsBuyed(IsBuyed(weapons[j].id));
			weapons[j].SetListenerClick(OnClickWeapon);
			keysWeapons[j] = idStore + weapons[j].id;
		}
		UpdateMoney();
	}

	public void ResetBuyed()
	{
		weapons = GetComponentsInChildren<WeaponOfStore>();
		keysWeapons = new string[weapons.Length];
		for (int i = 0; i < weapons.Length; i++)
		{
			weapons[i].SetAsBuyed(buy: false);
			keysWeapons[i] = idStore + weapons[i].id;
		}
		Reset();
		UpdateMoney();
	}

	public string GetMoney()
	{
		Load();
		return "$" + money.ToString();
	}

	public void SetMoney(int value)
	{
		money = value;
		PlayerPrefsFile.SetInt(idStore + "money", value);
	}

	private void Awake()
	{
		Load();
		for (int i = 0; i < weaponCategory.Length; i++)
		{
			buttonPointer = weaponCategory[i].tab.gameObject.AddComponent<TouchButtonPointer>();
			buttonPointer.SetListenerPointerDown(ShowCategory);
		}
		startGame.onClick.AddListener(OnRunGame);
		buttonClose.onClick.AddListener(switchUI.ShowDefault);
		buttonShow.onClick.AddListener(Show);
		screenSelect.SetActive(value: false);
		buttonMainWpn.onClick.AddListener(SelectWeaponMain);
		buttonBackupWpn.onClick.AddListener(SelectWeaponBackup);
		buttonCloseSelect.onClick.AddListener(CloseScreenSelect);
	}

	private void Start()
	{
		ShowCategory(0);
		CloseScreenSelect();
	}

	public void Show()
	{
		switchUI.Show(base.gameObject);
	}

	public void ShowCategory(TouchButtonPointer button)
	{
		for (int i = 0; i < weaponCategory.Length; i++)
		{
			if (weaponCategory[i].tab.gameObject == button.gameObject)
			{
				ShowCategory(i);
			}
			else
			{
				weaponCategory[i].tab.interactable = true;
			}
		}
	}

	public void ShowCategory(int selected)
	{
		weaponCategory[selected].tab.interactable = false;
		for (int i = 0; i < weapons.Length; i++)
		{
			weapons[i].gameObject.SetActive(value: false);
			for (int j = 0; j < weaponCategory[selected].list.Length; j++)
			{
				if (weapons[i] == weaponCategory[selected].list[j])
				{
					weapons[i].gameObject.SetActive(value: true);
					break;
				}
			}
		}
		float num = 0f;
		for (int k = 0; k < weaponCategory[selected].list.Length; k++)
		{
			if (weaponCategory[selected].list[k] != null)
			{
				weaponCategory[selected].list[k].transform.SetAsLastSibling();
				num += 1f;
			}
		}
		if (container != null)
		{
			float num2 = Mathf.Ceil(num / (float)grid.constraintCount);
			RectTransform rectTransform = container;
			float num3 = num2;
			Vector2 cellSize = grid.cellSize;
			float y = cellSize.y;
			Vector2 spacing = grid.spacing;
			rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, num3 * (float)Mathf.FloorToInt(y + spacing.y) + (float)grid.padding.top);
			container.gameObject.GetComponentInParent<ScrollRect>().verticalNormalizedPosition = 1f;
			container.localPosition = Vector3.zero;
		}
	}

	private void OnDestroy()
	{
	}

	public void OnRunGame()
	{
		mainWeapon = mainDefault;
		backupWeapon = backupDefault;
		id = idStore;
		PlayerPrefsFile.Save();
	}

	public void OnClickWeapon(WeaponOfStore weapon)
	{
		screenSelect.SetActive(value: true);
		screenSelect.transform.SetAsLastSibling();
		selectedWeapon = weapon;
		for (int i = 0; i < weapons.Length; i++)
		{
			if (weapons[i].id == weapon.id)
			{
				weapons[i].ShowProperties(iconNewWpn, textNameWpn, textPropertiesWpn);
			}
			if (weapons[i].id == selectedMain)
			{
				iconMainWeapon.sprite = weapons[i].icon;
			}
			else if (weapons[i].id == selectedBackup)
			{
				iconBackupWeapon.sprite = weapons[i].icon;
			}
		}
	}

	public void SelectWeaponMain()
	{
		selectedWeapon.Buy(this);
		if (selectedWeapon.isBuyed)
		{
			if (selectedWeapon.id == selectedBackup)
			{
				selectedBackup = selectedMain;
			}
			selectedMain = selectedWeapon.id;
		}
		CloseScreenSelect();
	}

	public void SelectWeaponBackup()
	{
		selectedWeapon.Buy(this);
		if (selectedWeapon.isBuyed)
		{
			if (selectedWeapon.id == selectedMain)
			{
				selectedMain = selectedBackup;
			}
			selectedBackup = selectedWeapon.id;
		}
		CloseScreenSelect();
	}

	public void CloseScreenSelect()
	{
		for (int i = 0; i < weapons.Length; i++)
		{
			if (weapons[i].id != selectedMain && weapons[i].id != selectedBackup)
			{
				weapons[i].SetAsUnselected();
			}
			else
			{
				weapons[i].SetAsSelected();
			}
		}
		screenSelect.SetActive(value: false);
	}

	public bool Buy(string id, int cost)
	{
		if (IsBuyed(id))
		{
			return false;
		}
		if (UpdateMoney(cost))
		{
			PlayerPrefsFile.SetInt(idStore + id, 1);
			RunSignalMoney(moneyDisplay, (-1 * cost).ToString());
			return true;
		}
		RunSignalMoney(moneyDisplay);
		return false;
	}

	public bool Buy(int cost)
	{
		return Buy(cost, moneyDisplay);
	}

	public bool Buy(int cost, Text money)
	{
		if (UpdateMoney(cost))
		{
			RunSignalMoney(money, (-1 * cost).ToString());
			return true;
		}
		RunSignalMoney(money);
		return false;
	}

	public bool Buy(string name, int cost, int num = 1)
	{
		if (IsBuyed(name))
		{
			return false;
		}
		if (UpdateMoney(cost))
		{
			PlayerPrefsFile.SetInt(idStore + name, num);
			RunSignalMoney(moneyDisplay, (-1 * cost).ToString());
			return true;
		}
		RunSignalMoney(moneyDisplay);
		return false;
	}

	public bool IsBuyed(string name)
	{
		name = idStore + name;
		return PlayerPrefsFile.GetInt(name, 0) != 0;
	}

	private bool UpdateMoney()
	{
		PlayerPrefsFile.SetInt(idStore + "money", money);
		moneyDisplay.text = "$" + money.ToString();
		return false;
	}

	private bool UpdateMoney(int cost)
	{
		if (money >= cost)
		{
			money -= cost;
			PlayerPrefsFile.SetInt(idStore + "money", money);
			moneyDisplay.text = "$" + money.ToString();
			return true;
		}
		return false;
	}

	public void RunSignalMoney(Text money, string textSignal)
	{
		if (moneySignal != null)
		{
			moneySignal.text = GetMoney();
		}
		moneySignal = money;
		moneySignal.text = textSignal;
		CancelInvoke("SignalMoney");
		InvokeRepeating("SignalMoney", 0.15f, 0.15f);
	}

	public void RunSignalMoney(Text money)
	{
		RunSignalMoney(money, money.text);
	}

	public void SignalMoney()
	{
		moneySignal.enabled = !moneySignal.enabled;
		numInvoke++;
		if (numInvoke >= 6 && moneySignal.enabled)
		{
			numInvoke = 0;
			moneySignal.text = GetMoney();
			CancelInvoke("SignalMoney");
		}
	}
}
