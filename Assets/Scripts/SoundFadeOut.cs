using UnityEngine;

public class SoundFadeOut : MonoBehaviour
{
	public static string[] isRun = new string[5];

	public string onceScene = string.Empty;

	public AudioSource sound;

	[Range(0f, 1f)]
	public float startTime = 0.5f;

	[Range(0f, 1f)]
	public float endTime = 1f;

	private void Awake()
	{
		sound.loop = false;
		if (!base.isActiveAndEnabled || !(onceScene != string.Empty))
		{
			return;
		}
		int num = 0;
		while (true)
		{
			if (num < isRun.Length)
			{
				if (isRun[num] == onceScene)
				{
					break;
				}
				num++;
				continue;
			}
			return;
		}
		base.enabled = false;
		onceScene = string.Empty;
	}

	private void Start()
	{
		startTime *= sound.clip.length;
		endTime *= sound.clip.length;
	}

	private void Update()
	{
		sound.volume = 1f - Mathf.Clamp01((sound.time - startTime) / (endTime - startTime));
		sound.volume = Mathf.Floor(sound.volume * 1000f) / 1000f;
		if (sound.volume <= 0.05f || sound.time >= endTime)
		{
			base.enabled = false;
		}
	}

	private void OnDisable()
	{
		sound.volume = 0f;
		sound.Stop();
		sound.enabled = false;
	}

	private void OnDestroy()
	{
		if (!(onceScene != string.Empty))
		{
			return;
		}
		int num = 0;
		while (true)
		{
			if (num < isRun.Length)
			{
				if (isRun[num] == null)
				{
					break;
				}
				num++;
				continue;
			}
			return;
		}
		isRun[num] = onceScene;
	}
}
