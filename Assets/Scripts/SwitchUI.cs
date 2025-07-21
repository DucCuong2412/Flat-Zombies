using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("PVold/UI/SwitchUI")]
public class SwitchUI : MonoBehaviour
{
	public enum ShowUIDefault
	{
		Awake,
		Start,
		OnEnable,
		None
	}

	public static string nameDefault = string.Empty;

	[Tooltip("Объект с интерфейсом на экране по умолчанию")]
	public int idDefault;

	[Tooltip("На каком этапе показать меню по умолчанию и скрыть все остальные")]
	public ShowUIDefault showDefault;

	public bool hideNotSelected = true;

	public bool slideToFront;

	[Tooltip("Список контейнеров с элементами интерфейса")]
	public RectTransform[] list = new RectTransform[0];

	[Tooltip("Список кнопок для отображения")]
	public Button[] buttons = new Button[0];

	private void Awake()
	{
		for (int i = 0; i < buttons.Length; i++)
		{
			if (buttons[i] != null && list[i] != null)
			{
				int id = i;
				buttons[i].onClick.AddListener(delegate
				{
					Show(id);
				});
			}
		}
		int num = 0;
		while (nameDefault != string.Empty && num < list.Length)
		{
			if (list[num].gameObject.name == nameDefault)
			{
				idDefault = num;
			}
			num++;
		}
		if (showDefault == ShowUIDefault.Awake && base.enabled)
		{
			ShowDefault();
		}
	}

	private void Start()
	{
		if (showDefault == ShowUIDefault.Start)
		{
			ShowDefault();
		}
		nameDefault = string.Empty;
	}

	private void OnEnable()
	{
		if (showDefault == ShowUIDefault.OnEnable)
		{
			ShowDefault();
		}
	}

	public void Show(RectTransform parent)
	{
		int num = 0;
		while (list != null && num < list.Length)
		{
			list[num].gameObject.SetActive(list[num].gameObject == parent.gameObject == hideNotSelected);
			if (slideToFront && list[num].gameObject.activeSelf)
			{
				list[num].SetAsLastSibling();
			}
			num++;
		}
	}

	public void Show(GameObject gameObject)
	{
		int num = 0;
		while (list != null && num < list.Length)
		{
			list[num].gameObject.SetActive(list[num].gameObject == gameObject == hideNotSelected);
			if (slideToFront && list[num].gameObject.activeSelf)
			{
				list[num].SetAsLastSibling();
			}
			num++;
		}
	}

	public void Show(int id)
	{
		int num = 0;
		while (list != null && num < list.Length)
		{
			list[num].gameObject.SetActive(num == id == hideNotSelected);
			if (slideToFront && list[num].gameObject.activeSelf)
			{
				list[num].SetAsLastSibling();
			}
			num++;
		}
	}

	public void Show(string name)
	{
		int num = 0;
		while (list != null && num < list.Length)
		{
			list[num].gameObject.SetActive(list[num].gameObject.name == name == hideNotSelected);
			if (slideToFront && list[num].gameObject.activeSelf)
			{
				list[num].SetAsLastSibling();
			}
			num++;
		}
	}

	public void ShowDefault()
	{
		Show(idDefault);
	}

	public void HideAll()
	{
		Show(-1);
	}

	public void SetDefault(RectTransform continer)
	{
		int num = 0;
		while (true)
		{
			if (list != null && num < list.Length)
			{
				if (list[num] == continer)
				{
					break;
				}
				num++;
				continue;
			}
			return;
		}
		idDefault = num;
	}

	public void SetDefault(string name)
	{
		int num = 0;
		while (true)
		{
			if (list != null && num < list.Length)
			{
				if (list[num].gameObject.name == name)
				{
					break;
				}
				num++;
				continue;
			}
			return;
		}
		idDefault = num;
	}

	public void SetDefaultScene(string name)
	{
		nameDefault = name;
	}
}
