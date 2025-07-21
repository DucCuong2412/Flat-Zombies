using UnityEngine;
using UnityEngine.UI;

public class SliderValueText : MonoBehaviour
{
	private Slider slider;

	public Text text;

	public int num = 1000;

	private void OnDrawGizmosSelected()
	{
		if (text == null)
		{
			text = GetComponentInChildren<Text>();
		}
	}

	private void Awake()
	{
		slider = GetComponent<Slider>();
		slider.onValueChanged.AddListener(ShowSliderValue);
		ShowSliderValue(slider.value);
	}

	public void ShowSliderValue(float value)
	{
		value = Mathf.Floor(value * (float)num) / (float)num;
		text.text = value.ToString();
		slider.value = value;
	}
}
