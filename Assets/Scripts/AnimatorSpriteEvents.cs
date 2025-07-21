using PVold;
using System;
using UnityEngine;
using UnityEngine.Events;

[AddComponentMenu("PVold/AnimatorSpriteEvents")]
public class AnimatorSpriteEvents : AnimatorSprite
{
	[Serializable]
	public struct EventFrame
	{
		[Tooltip("Кадр, на котором совершить вызов функций")]
		public Sprite frame;

		public UnityEvent show;
	}

	[Tooltip("События анимаций")]
	public EventFrame[] events;

	public override void SetFrame(int frame)
	{
		base.SetFrame(frame);
		int num = 0;
		while (true)
		{
			if (num < events.Length)
			{
				if (events[num].frame == currentAnimation.frames[frame])
				{
					break;
				}
				num++;
				continue;
			}
			return;
		}
		events[num].show.Invoke();
	}
}
