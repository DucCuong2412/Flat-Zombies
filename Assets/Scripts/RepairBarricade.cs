using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RepairBarricade : MonoBehaviour
{
	public static string currentGameMode = string.Empty;

	public string gameMode = string.Empty;

	public Text healthBarricade;

	public Image healthIndicator;

	public Color colorMin = Color.red;

	public Color colorMax = Color.green;

	public Text money;

	public int cost;

	public int sizeRecoveryHelath;

	public Button addHelath;

	public StoreWeapons storeWeapon;

	private void Awake()
	{
		addHelath.onClick.AddListener(RepairAddHealth);
	}

	private void OnEnable()
	{
		UpdateIndicator();
		money.text = storeWeapon.GetMoney();
	}

	public void RepairAddHealth()
	{
		if (ZombieBarricade.currentHealth < 100f && storeWeapon.Buy(cost, money))
		{
			ZombieBarricade.currentHealth = Mathf.Clamp(ZombieBarricade.currentHealth + (float)sizeRecoveryHelath, 0f, 100f);
			PlayerPrefsFile.SetFloat(gameMode + "_barricadeHealth", ZombieBarricade.currentHealth);
			UpdateIndicator();
		}
		else if (ZombieBarricade.currentHealth == 100f)
		{
			StartCoroutine(SignalHealthIndicator(3));
		}
	}

	private void UpdateIndicator()
	{
		float value = Mathf.Floor(ZombieBarricade.currentHealth / 100f * 100f) / 100f;
		value = Mathf.Clamp01(value);
		float H = 0f;
		float H2 = 0f;
		float S = 0f;
		float V = 0f;
		Color.RGBToHSV(colorMin, out H, out S, out V);
		Color.RGBToHSV(colorMax, out H2, out S, out V);
		healthIndicator.GetComponent<RectTransform>().localScale = new Vector3(value, 1f, 1f);
		healthIndicator.color = Color.HSVToRGB(Mathf.Lerp(H, H2, value), S, V);
		healthBarricade.text = Mathf.FloorToInt(ZombieBarricade.currentHealth).ToString();
	}

	private IEnumerator SignalHealthIndicator(int numSignal)
	{
		if (numSignal > 0)
		{
			healthIndicator.gameObject.SetActive(!healthIndicator.gameObject.activeInHierarchy);
			healthBarricade.gameObject.SetActive(!healthBarricade.gameObject.activeInHierarchy);
			yield return new WaitForSeconds(0.15f);
			StartCoroutine(SignalHealthIndicator(numSignal - 1));
		}
		else
		{
			healthIndicator.gameObject.SetActive(value: true);
			healthBarricade.gameObject.SetActive(value: true);
			StopCoroutine("SignalHealthIndicator");
		}
	}
}
