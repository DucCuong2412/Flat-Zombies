using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WeaponElementList : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
{
	public delegate void ListenerMethod(WeaponElementList weapon);

	public string id;

	public string nameFull = string.Empty;

	public float damage;

	public float magazineSize;

	public float speedShot;

	public float timeReload;

	public Sprite icon;

	public Color colorSelected = Color.blue;

	private Color defaultColorBack;

	private Image background;

	private ListenerMethod listenerClick;

	private void Awake()
	{
		background = GetComponent<Image>();
		defaultColorBack = background.color;
	}

	private void Start()
	{
	}

	public void SetListenerClick(ListenerMethod listener)
	{
		listenerClick = listener;
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		listenerClick(this);
	}

	public void SetAsSelected()
	{
		background.color = colorSelected;
	}

	public void SetAsUnselected()
	{
		background.color = defaultColorBack;
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
}
