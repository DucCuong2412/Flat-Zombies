using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameResult : MonoBehaviour
{
	[Serializable]
	public struct ParameterOfResult
	{
		public string name;

		[HideInInspector]
		public string value;

		public bool title;

		public Text text;

		[HideInInspector]
		public string unit;

		public void Reset()
		{
			name = string.Empty;
			value = string.Empty;
			unit = string.Empty;
		}
	}

	public static ParameterOfResult[] parameters = new ParameterOfResult[10];

	public GameObject container;

	public bool showFullHit;

	public ParameterOfResult[] displayParameters;

	public UnityEvent show;

	public static void SetLengthParameters(int length)
	{
		parameters = new ParameterOfResult[length];
	}

	public static void ResetAll()
	{
		parameters = new ParameterOfResult[parameters.Length];
	}

	public static void ResetParametr(string name)
	{
		int num = 0;
		while (true)
		{
			if (num < parameters.Length)
			{
				if (parameters[num].name == name)
				{
					break;
				}
				num++;
				continue;
			}
			return;
		}
		parameters[num].Reset();
	}

	public static void SetParametr(string name, string value, string unit)
	{
		int num = 0;
		while (true)
		{
			if (num < parameters.Length)
			{
				if (string.IsNullOrEmpty(parameters[num].name) || parameters[num].name == name)
				{
					break;
				}
				num++;
				continue;
			}
			return;
		}
		parameters[num].name = name;
		parameters[num].value = value;
		parameters[num].unit = unit;
	}

	public static void SetParametr(string id, string value)
	{
		SetParametr(id, value, string.Empty);
	}

	public static void SetParametr(string id, int value)
	{
		SetParametr(id, value.ToString(), string.Empty);
	}

	public static void SetParametr(string id, float value)
	{
		SetParametr(id, value.ToString(), string.Empty);
	}

	public static void SetParametr(string id, int value, string unit)
	{
		SetParametr(id, value.ToString(), unit);
	}

	public static void SetParametr(string id, float value, string unit)
	{
		SetParametr(id, value.ToString(), unit);
	}

	public static string FullTime(int seconds, bool russian)
	{
		string text = string.Empty;
		int num = seconds / 3600;
		if (num != 0)
		{
			seconds -= num * 3600;
			text += ((!russian) ? (num.ToString() + " h. ") : (num.ToString() + " ч. "));
		}
		int num2 = seconds / 60;
		if (num2 != 0)
		{
			seconds -= num2 * 60;
			text += ((!russian) ? (num2.ToString() + " min. ") : (num2.ToString() + " мин. "));
		}
		if (seconds != 0 && num == 0)
		{
			text += ((!russian) ? (seconds.ToString() + " sec.") : (seconds.ToString() + " cек."));
		}
		return text;
	}

	public static string FullTime(int seconds)
	{
		return FullTime(seconds, SimpleLocalisation.isRussia());
	}

	private void Awake()
	{
	}

	private void Start()
	{
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		bool flag = !string.IsNullOrEmpty(parameters[0].name);
		num2 = 0;
		while (flag && num2 < displayParameters.Length)
		{
			for (num3 = 0; num3 < parameters.Length; num3++)
			{
				if (displayParameters[num2].name == parameters[num3].name)
				{
					num++;
					break;
				}
			}
			num2++;
		}
		if (flag && (!showFullHit || num == displayParameters.Length))
		{
			Show();
		}
		else if (container != null)
		{
			container.SetActive(value: false);
		}
	}

	private void OnDestroy()
	{
		ResetAll();
	}

	public void Show()
	{
		if (container != null)
		{
			container.SetActive(value: true);
			container.transform.SetAsLastSibling();
		}
		for (int i = 0; i < displayParameters.Length; i++)
		{
			for (int j = 0; j < parameters.Length; j++)
			{
				if (displayParameters[i].text != null && displayParameters[i].name == parameters[j].name)
				{
					displayParameters[i].text.gameObject.SetActive(value: true);
					if (!displayParameters[i].title)
					{
						displayParameters[i].text.text = string.Empty;
					}
					displayParameters[i].text.text = displayParameters[i].text.text + parameters[j].value + parameters[j].unit;
				}
			}
		}
		base.transform.SetAsLastSibling();
		show.Invoke();
	}
}
