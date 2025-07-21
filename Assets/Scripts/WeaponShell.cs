using System;
using UnityEngine;

public class WeaponShell : MonoBehaviour
{
	public bool hide;

	public float ground;

	public float slideOnGround = 1f;

	public float timeSlide = 1f;

	public int minSpeedRotate;

	public int maxSpeedRotate;

	[HideInInspector]
	public int speedRotate;

	[HideInInspector]
	public Vector3 speed = default(Vector3);

	[HideInInspector]
	public SpriteRenderer render;

	private float startPosition;

	private float currentTimerSlide;

	private float distance = 1f;

	private Vector2 gravity = default(Vector2);

	private Vector3 position;

	private float deltaTime;

	public void Reset(Vector3 position, Sprite sprite, float angleMove, int impulse, int angleRotation)
	{
		render.sprite = sprite;
		base.transform.position = position;
		base.transform.rotation = Quaternion.Euler(0f, 0f, angleRotation);
		impulse = impulse * UnityEngine.Random.Range(70, 125) / 100;
		speed.x = Mathf.Cos(angleMove * ((float)Math.PI / 180f)) * (float)impulse;
		speed.y = Mathf.Sin(angleMove * ((float)Math.PI / 180f)) * (float)impulse;
		slideOnGround = Mathf.Abs(slideOnGround);
		if (speed.x < 0f)
		{
			slideOnGround = -1f * slideOnGround;
		}
		speedRotate = UnityEngine.Random.Range(minSpeedRotate, maxSpeedRotate);
		base.gameObject.SetActive(value: true);
		base.enabled = true;
		startPosition = position.y;
		gravity = Physics2D.gravity;
		currentTimerSlide = timeSlide;
	}

	private void Start()
	{
		render = GetComponent<SpriteRenderer>();
		base.gameObject.SetActive(value: false);
	}

	private void Update()
	{
		deltaTime = Time.deltaTime * 2f;
		speed.y += gravity.y * deltaTime;
		position = base.transform.position;
		position.y += speed.y * deltaTime;
		position.x += speed.x * deltaTime;
		base.transform.position = position;
		if (gravity.y != 0f && speedRotate != 0)
		{
			base.transform.Rotate(0f, 0f, (float)speedRotate * Time.deltaTime);
		}
		if (position.y < ground)
		{
			currentTimerSlide -= Time.deltaTime;
			speed.x = Mathf.Lerp(0f, slideOnGround, currentTimerSlide / timeSlide);
			if (gravity.y != 0f)
			{
				speed.y = 0f;
				gravity.y = 0f;
				base.transform.Rotate(0f, 0f, 90f);
				speedRotate = 0;
			}
		}
		if (hide)
		{
			distance = (position.y - ground) / Mathf.Abs(ground - startPosition);
			distance = Mathf.Clamp01(distance * 2f);
			render.color = new Color(1f, 1f, 1f, distance);
			if (distance == 0f)
			{
				base.gameObject.SetActive(value: false);
			}
		}
		else if (gravity.y == 0f && speed.x == 0f)
		{
			base.enabled = false;
		}
	}
}
