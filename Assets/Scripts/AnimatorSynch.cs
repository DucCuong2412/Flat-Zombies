using PVold;
using UnityEngine;

public class AnimatorSynch : MonoBehaviour
{
	public AnimatorSprite animator;

	[Tooltip("Список анимаций")]
	public AnimationSprite[] animations;

	[HideInInspector]
	public SpriteRenderer render;

	[HideInInspector]
	public AnimationSprite currentAnimation = default(AnimationSprite);

	private void Start()
	{
		render = GetComponent<SpriteRenderer>();
	}

	private void LateUpdate()
	{
		if (currentAnimation.name != animator.currentAnimation.name)
		{
			for (int i = 0; i < animations.Length; i++)
			{
				if (animations[i].name == animator.currentAnimation.name)
				{
					currentAnimation = animations[i];
					break;
				}
			}
		}
		render.sprite = currentAnimation.frames[animator.currentFrame - 1];
	}
}
