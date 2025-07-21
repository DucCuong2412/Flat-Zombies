using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WeaponOfStore : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
{
	public delegate void ListenerMethod(WeaponOfStore weapon);

	public static int money = -1;

	[Tooltip("Строковой идентификатор, для сохранения через PlayerPrefs")]
	public string id;

	public int cost;

	public string nameFull = string.Empty;

	public float damage;

	public float magazineSize;

	public float speedShot;

	public float timeReload;

	public Sprite icon;

	public Text price;

	public Image iconImage;

	public Color colorIconBuy = Color.white;

	public Color colorSelected = Color.red;

	public Image background;

	public Color colorNotBought = Color.red;

	public Color colorBought = Color.green;

	public Color colorBlock = Color.grey;

	[HideInInspector]
	public bool isBuyed;

	private ListenerMethod listenerClick;

	private void Awake()
	{
		if (price != null)
		{
			price.text = "$" + cost.ToString();
		}
	}

	public void SetListenerClick(ListenerMethod listener)
	{
		listenerClick = listener;
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		listenerClick(this);
	}

	public void ShowProperties(Image imageScreen, Text textName, Text properties)
	{
		imageScreen.sprite = icon;
		textName.text = nameFull;
		properties.text = damage.ToString() + "\n";
		properties.text = properties.text + GetValueNA(magazineSize) + "\n";
		properties.text = properties.text + GetValueNA(speedShot) + "\n";
		properties.text += GetValueNA(timeReload);
	}

	private string GetValueNA(float value)
	{
		if (value != 0f)
		{
			return value.ToString();
		}
		return "- -";
	}

	public void Buy(StoreWeapons store)
	{
		if (cost == 0)
		{
			SetAsBuyed(buy: true);
		}
		else if (store.Buy(id, cost))
		{
			SetAsBuyed(buy: true);
		}
		else if (!isBuyed)
		{
			background.color = colorBlock;
		}
	}

	public void SetAsSelected()
	{
		background.color = colorSelected;
	}

	public void SetAsUnselected()
	{
		if (isBuyed)
		{
			background.color = colorBought;
		}
		else
		{
			background.color = colorNotBought;
		}
	}

	public void SetAsBuyed(bool buy)
	{
		isBuyed = (buy || cost == 0);
		if (isBuyed)
		{
			iconImage.color = Color.white;
			background.color = colorBought;
		}
		else
		{
			iconImage.color = colorIconBuy;
			background.color = colorNotBought;
		}
		if (price != null)
		{
			price.gameObject.SetActive(!isBuyed);
		}
	}
}
