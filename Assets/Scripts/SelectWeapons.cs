using UnityEngine;
using UnityEngine.UI;

public class SelectWeapons : MonoBehaviour
{
	public string mainWeapon;

	public string backupWeapon;

	public Button showScreen;

	public Button closeAndSave;

	public RectTransform[] sorting = new RectTransform[0];

	[Space(8f)]
	public GameObject screenSelect;

	public Text textNameWpn;

	public Text textPropertiesWpn;

	public Image iconSelectedWeapon;

	public Image iconMainWeapon;

	public Image iconBackupWeapon;

	public Button buttonMainWpn;

	public Button buttonBackupWpn;

	public Button buttonCloseSelect;

	private float lastTimeScale;

	private bool lastStateListener = true;

	private WeaponElementList selectedWeapon;

	private WeaponElementList[] weapons = new WeaponElementList[0];

	private void Start()
	{
		closeAndSave.onClick.AddListener(Close);
		showScreen.onClick.AddListener(Show);
		weapons = base.gameObject.GetComponentsInChildren<WeaponElementList>();
		for (int i = 0; i < weapons.Length; i++)
		{
			weapons[i].SetListenerClick(OnClickWeapon);
		}
		for (int j = 0; j < sorting.Length; j++)
		{
			sorting[j].transform.SetAsLastSibling();
		}
		buttonMainWpn.onClick.AddListener(SelectWeaponMain);
		buttonBackupWpn.onClick.AddListener(SelectWeaponBackup);
		buttonCloseSelect.onClick.AddListener(CloseScreenSelect);
		base.gameObject.SetActive(value: false);
	}

	public void Show()
	{
		UnityEngine.Debug.LogWarning("asd.Show()");
		base.gameObject.SetActive(value: true);
		base.transform.SetAsLastSibling();
		CloseScreenSelect();
		lastTimeScale = Time.timeScale;
		lastStateListener = AudioListener.pause;
		if (lastTimeScale != 0f)
		{
			Time.timeScale = 0f;
			AudioListener.pause = true;
		}
	}

	public void Close()
	{
		if (lastTimeScale != 0f)
		{
			Time.timeScale = lastTimeScale;
			AudioListener.pause = lastStateListener;
			lastTimeScale = 0f;
		}
		Player.onStage.SelectWeapon(mainWeapon, backupWeapon);
		base.gameObject.SetActive(value: false);
	}

	public void OnClickWeapon(WeaponElementList weapon)
	{
		screenSelect.SetActive(value: true);
		screenSelect.transform.SetAsLastSibling();
		selectedWeapon = weapon;
		for (int i = 0; i < weapons.Length; i++)
		{
			if (weapons[i].id == weapon.id)
			{
				weapons[i].ShowProperties(iconSelectedWeapon, textNameWpn, textPropertiesWpn);
			}
			if (weapons[i].id == mainWeapon)
			{
				iconMainWeapon.sprite = weapons[i].icon;
			}
			else if (weapons[i].id == backupWeapon)
			{
				iconBackupWeapon.sprite = weapons[i].icon;
			}
		}
	}

	public void SelectWeaponMain()
	{
		if (selectedWeapon.id == backupWeapon)
		{
			backupWeapon = mainWeapon;
		}
		mainWeapon = selectedWeapon.id;
		CloseScreenSelect();
	}

	public void SelectWeaponBackup()
	{
		if (selectedWeapon.id == mainWeapon)
		{
			mainWeapon = backupWeapon;
		}
		backupWeapon = selectedWeapon.id;
		CloseScreenSelect();
	}

	public void CloseScreenSelect()
	{
		for (int i = 0; i < weapons.Length; i++)
		{
			if (weapons[i].id != mainWeapon && weapons[i].id != backupWeapon)
			{
				weapons[i].SetAsUnselected();
			}
			else
			{
				weapons[i].SetAsSelected();
			}
		}
		screenSelect.SetActive(value: false);
		UnityEngine.Debug.Log("mainWeapon:" + mainWeapon);
		UnityEngine.Debug.Log("backupWeapon:" + backupWeapon);
	}
}
