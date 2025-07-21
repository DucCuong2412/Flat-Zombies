using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("PVold/DisplayFPS")]
[RequireComponent(typeof(Text))]
public class DisplayFPS : MonoBehaviour
{
	public string prefix = "FPS: ";

	[Range(0f, 2f)]
	public float intervalUpdate = 1f;

	private float updateValue;

	private int frameCount;

	private Text text;

	private float currentSpeed;

	private void Start()
	{
		text = base.gameObject.GetComponent<Text>();
		text.text = "- -";
	}

	private void Update()
	{
		if (updateValue >= intervalUpdate)
		{
			currentSpeed = Mathf.Floor((float)frameCount / intervalUpdate);
			text.text = prefix + currentSpeed.ToString();
			updateValue = 0f;
			frameCount = 0;
		}
		updateValue += Time.deltaTime;
		frameCount++;
	}
}
