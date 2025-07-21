using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("PVold/UI/AnimationImageUI")]
[RequireComponent(typeof(Image))]
public class AnimationImageUI : MonoBehaviour
{
	[Tooltip("Воспроизвести при старте")]
	public bool playStart = true;

	[Tooltip("Скорость. Количество кадров в секунду")]
	public float speed = 30f;

	[Tooltip("Воспроизвести в обратном порядке")]
	public bool reverse;

	[Tooltip("Учитывать множитель времени Time.timeScale")]
	public bool useTimeScale;

	[Tooltip("Проигрывать повторно")]
	public bool loop = true;

	public Sprite[] frames;

	[HideInInspector]
	public Image image;

	[HideInInspector]
	public bool isPlaying;

	private float curFrame;

	private int preSetFrame;

	public int currentFrame => preSetFrame;

	public int totalFrames => (frames != null) ? frames.Length : 0;

	private void Start()
	{
		image = base.gameObject.GetComponent<Image>();
		curFrame = 0f;
		isPlaying = playStart;
		if (useTimeScale && Time.timeScale != 0f)
		{
			speed /= Time.timeScale;
		}
	}

	private void Update()
	{
		if (!isPlaying)
		{
			return;
		}
		if (reverse)
		{
			PrevFrame(aUseSpeed: true);
			if (loop && curFrame <= 0f)
			{
				curFrame = totalFrames;
			}
		}
		else
		{
			NextFrame(aUseSpeed: true);
			if (loop && curFrame >= (float)totalFrames)
			{
				curFrame = 0f;
			}
		}
	}

	public void Play()
	{
		isPlaying = true;
	}

	public void Stop()
	{
		isPlaying = false;
	}

	public void GotoAndStop(int aFrame)
	{
		aFrame--;
		aFrame = ((aFrame > 0) ? ((aFrame <= totalFrames - 1) ? aFrame : (totalFrames - 1)) : 0);
		SetFrame(aFrame);
		curFrame = aFrame;
		Stop();
	}

	public void GotoAndPlay(int aFrame)
	{
		aFrame--;
		aFrame = ((aFrame > 0) ? ((aFrame <= totalFrames - 1) ? aFrame : (totalFrames - 1)) : 0);
		SetFrame(aFrame);
		curFrame = aFrame;
		Play();
	}

	public void PlayRandomFrame()
	{
		GotoAndPlay(Mathf.CeilToInt(UnityEngine.Random.value * (float)totalFrames));
	}

	public void StopRandomFrame()
	{
		GotoAndStop(Mathf.CeilToInt(UnityEngine.Random.value * (float)totalFrames));
	}

	public void NextFrame(bool aUseSpeed = false)
	{
		if (curFrame < (float)totalFrames)
		{
			SetFrame(Mathf.FloorToInt(curFrame));
			if (aUseSpeed)
			{
				curFrame += speed * Time.deltaTime;
			}
			else
			{
				curFrame += 1f;
			}
		}
		else
		{
			Stop();
			SetFrame(totalFrames - 1);
		}
	}

	public void PrevFrame(bool aUseSpeed = false)
	{
		if (curFrame > 0f)
		{
			if (aUseSpeed)
			{
				curFrame -= speed * Time.deltaTime;
			}
			else
			{
				curFrame -= 1f;
			}
			if (curFrame < 0f)
			{
				curFrame = 0f;
			}
			SetFrame(Mathf.FloorToInt(curFrame));
		}
		else
		{
			Stop();
			SetFrame(0);
		}
	}

	protected void SetFrame(int frame)
	{
		if (frames != null && preSetFrame != frame)
		{
			image.sprite = frames[frame];
			preSetFrame = frame;
		}
	}
}
