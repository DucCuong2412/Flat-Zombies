using UnityEngine;

namespace PVold
{
	[RequireComponent(typeof(SpriteRenderer))]
	[AddComponentMenu("PVold/AnimatorSprite")]
	public class AnimatorSprite : MonoBehaviour
	{
		[Tooltip("Имя текущей анимации")]
		public string initialAnimation;

		[Tooltip("Воспроизвести при старте")]
		public bool playStart = true;

		[Tooltip("Список анимаций")]
		public AnimationSprite[] animations;

		[Space(15f)]
		[Tooltip("Множитель скорости анимаций")]
		public float timeScale = 1f;

		[Tooltip("Скорость. Количество кадров в секунду")]
		public int speed = 30;

		[Tooltip("Воспроизвести в обратном порядке")]
		public bool reverse;

		[Tooltip("Проигрывать повторно")]
		public bool loop = true;

		[HideInInspector]
		public AnimationSprite currentAnimation;

		private float _currentFrame;

		private bool _playing;

		private SpriteRenderer _render;

		private AnimationSprite tempAnimation = default(AnimationSprite);

		private int preSetFrame;

		public int currentFrame => preSetFrame + 1;

		public bool isPlaying => _playing;

		public int totalFrames => (currentAnimation.frames != null) ? currentAnimation.frames.Length : 0;

		public SpriteRenderer render
		{
			get
			{
				if (_render == null)
				{
					_render = GetComponent<SpriteRenderer>();
				}
				return _render;
			}
		}

		private void Awake()
		{
			_render = render;
			_currentFrame = 0f;
			_playing = playStart;
			if (!string.IsNullOrEmpty(initialAnimation))
			{
				SwitchAnimation(initialAnimation);
			}
		}

		private void Update()
		{
			if (_playing)
			{
				if (reverse)
				{
					PrevFrame(useSpeed: true);
				}
				else
				{
					NextFrame(useSpeed: true);
				}
			}
		}

		public void SwitchAnimation(string name, bool saveFrame)
		{
			if (name != currentAnimation.name)
			{
				bool flag = false;
				for (int i = 0; i < animations.Length; i++)
				{
					if (animations[i].name == name)
					{
						flag = true;
						currentAnimation = animations[i];
						initialAnimation = name;
						break;
					}
				}
				if (!flag)
				{
					UnityEngine.Debug.LogWarning("AnimatorSprite: Can't find animation \"" + name + "\"\n" + base.gameObject.name);
				}
			}
			if (!saveFrame)
			{
				_currentFrame = 0f;
			}
			if (_playing)
			{
				GotoAndPlay(Mathf.RoundToInt(_currentFrame));
			}
			else
			{
				GotoAndStop(Mathf.RoundToInt(_currentFrame));
			}
		}

		public void SwitchAnimation(string name)
		{
			SwitchAnimation(name, saveFrame: false);
		}

		public void SwitchRandomAnimation()
		{
			SwitchAnimation(animations[Random.Range(0, animations.Length)].name);
		}

		public void SwitchNextAnimation()
		{
			int num = 0;
			while (true)
			{
				if (num < animations.Length)
				{
					if (currentAnimation.name == animations[num].name)
					{
						break;
					}
					num++;
					continue;
				}
				return;
			}
			if (num + 1 < animations.Length)
			{
				SwitchAnimation(animations[num + 1].name);
			}
			else
			{
				SwitchAnimation(animations[0].name);
			}
		}

		public AnimationSprite GetAnimationSprite(string name)
		{
			for (int i = 0; i < animations.Length; i++)
			{
				if (animations[i].name == name)
				{
					return animations[i];
				}
			}
			return default(AnimationSprite);
		}

		public int GetTotalFrames(string name)
		{
			tempAnimation = GetAnimationSprite(name);
			if (!string.IsNullOrEmpty(tempAnimation.name))
			{
				return tempAnimation.frames.Length;
			}
			return 0;
		}

		public void Play()
		{
			playStart = true;
			_playing = (currentAnimation.frames.Length >= 2);
		}

		public void Play(string nameAnimation)
		{
			playStart = true;
			SwitchAnimation(nameAnimation);
			_playing = (currentAnimation.frames.Length >= 2);
		}

		public void Stop()
		{
			playStart = false;
			_playing = false;
		}

		public void GotoAndStop(int aFrame)
		{
			aFrame--;
			aFrame = ((aFrame > 0) ? ((aFrame <= totalFrames - 1) ? aFrame : (totalFrames - 1)) : 0);
			SetFrame(aFrame);
			_currentFrame = aFrame;
			Stop();
		}

		public void GotoAndPlay(int aFrame)
		{
			aFrame--;
			aFrame = ((aFrame > 0) ? ((aFrame <= totalFrames - 1) ? aFrame : (totalFrames - 1)) : 0);
			SetFrame(aFrame);
			_currentFrame = aFrame;
			Play();
		}

		public void GotoAndPlay(Sprite sprite)
		{
			bool flag = false;
			for (int i = 0; i < animations.Length; i++)
			{
				for (int j = 0; j < animations[i].frames.Length; j++)
				{
					if (animations[i].frames[j] == sprite)
					{
						flag = true;
						currentAnimation = animations[i];
						_currentFrame = j;
						initialAnimation = animations[i].name;
						SetFrame(j);
						Play();
						break;
					}
				}
			}
			if (!flag)
			{
				UnityEngine.Debug.LogWarning("Анимация не найдена, со спрайтом: " + sprite.name);
			}
		}

		public void PlayRandomFrame()
		{
			PlayRandomFrame(totalFrames);
		}

		public void PlayRandomFrame(int totalFrames)
		{
			GotoAndPlay(Mathf.CeilToInt(UnityEngine.Random.value * (float)totalFrames));
		}

		public void PlayRandomFrame(int start, int totalFrames)
		{
			GotoAndPlay(start + Mathf.CeilToInt(UnityEngine.Random.value * (float)totalFrames));
		}

		public void StopRandomFrame()
		{
			StopRandomFrame(totalFrames);
		}

		public void StopRandomFrame(int totalFrames)
		{
			GotoAndStop(Mathf.CeilToInt(UnityEngine.Random.value * (float)totalFrames));
		}

		public void StopRandomFrame(int start, int totalFrames)
		{
			GotoAndStop(start + Mathf.CeilToInt(UnityEngine.Random.value * (float)totalFrames));
		}

		public void NextFrame(bool useSpeed)
		{
			if (_currentFrame < (float)totalFrames)
			{
				SetFrame(Mathf.FloorToInt(_currentFrame));
				_currentFrame += (float)speed * Time.deltaTime * timeScale;
				_currentFrame = Mathf.Min(_currentFrame, totalFrames);
			}
			else if (loop)
			{
				_currentFrame = 0f;
				SetFrame(0);
			}
			else
			{
				Stop();
				SetFrame(totalFrames - 1);
			}
		}

		public void NextFrame()
		{
			if (_currentFrame < (float)totalFrames)
			{
				SetFrame(Mathf.FloorToInt(_currentFrame));
				_currentFrame += 1f;
				_currentFrame = Mathf.Min(_currentFrame, totalFrames);
			}
			else if (loop)
			{
				_currentFrame = 0f;
				SetFrame(0);
			}
			else
			{
				Stop();
				SetFrame(totalFrames - 1);
			}
		}

		public void PrevFrame(bool useSpeed)
		{
			if (_currentFrame > 0f)
			{
				SetFrame(Mathf.FloorToInt(_currentFrame));
				_currentFrame -= (float)speed * Time.deltaTime * timeScale;
				_currentFrame = Mathf.Max(_currentFrame, 0f);
			}
			else if (loop)
			{
				_currentFrame = totalFrames - 1;
				SetFrame(totalFrames);
			}
			else
			{
				Stop();
				SetFrame(totalFrames - 1);
			}
		}

		public void PrevFrame()
		{
			if (_currentFrame > 0f)
			{
				SetFrame(Mathf.FloorToInt(_currentFrame));
				_currentFrame -= 1f;
				_currentFrame = Mathf.Max(_currentFrame, 0f);
			}
			else if (loop)
			{
				_currentFrame = totalFrames - 1;
				SetFrame(totalFrames - 1);
			}
			else
			{
				Stop();
				SetFrame(0);
			}
		}

		public virtual void SetFrame(int frame)
		{
			if (currentAnimation.frames != null && preSetFrame != frame)
			{
				_render.sprite = currentAnimation.frames[frame];
				preSetFrame = frame;
			}
		}
	}
}
