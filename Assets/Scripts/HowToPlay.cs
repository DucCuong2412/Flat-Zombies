using UnityEngine;
using UnityEngine.UI;

public class HowToPlay : MonoBehaviour
{
	public Image imageFrame;

	public Text textCurrentFrame;

	public Sprite[] frames = new Sprite[0];

	public Text[] description = new Text[0];

	public Button buttonNextFrame;

	private int current = -1;

	private float audioVolume;

	private void Start()
	{
		buttonNextFrame.onClick.AddListener(NextFrame);
		NextFrame();
	}

	private void OnEnable()
	{
		if (AudioListener.volume != 0f)
		{
			audioVolume = AudioListener.volume;
			AudioListener.volume = 0f;
		}
	}

	private void OnDisable()
	{
		if (AudioListener.volume != audioVolume && audioVolume != 0f)
		{
			AudioListener.volume = audioVolume;
		}
	}

	public void NextFrame()
	{
		for (int i = 0; i < description.Length; i++)
		{
			description[i].gameObject.SetActive(value: false);
		}
		current++;
		current %= frames.Length;
		imageFrame.sprite = frames[current];
		description[current].gameObject.SetActive(value: true);
		textCurrentFrame.text = (current + 1).ToString() + " / " + frames.Length.ToString();
	}
}
