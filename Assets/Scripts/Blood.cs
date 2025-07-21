using PVold;
using System;
using UnityEngine;

[RequireComponent(typeof(AnimatorSprite))]
public class Blood : BloodBase, IEffectHitBullet
{
	public static int max = 16;

	public static BloodBase[] listDrops = new BloodBase[0];

	public static Blood[] dropsScene = new Blood[0];

	public int angle;

	[Range(0f, 180f)]
	public float angleScatter;

	public float startSpeed;

	public float addSpeedInPoint;

	public float maxDistSpeed;

	public Vector2 move = default(Vector2);

	public Vector2 gravity = default(Vector2);

	public float floor;

	public Sprite[] sprites = new Sprite[0];

	public SpriteRenderer renderOnFloor;

	[HideInInspector]
	public int scaleSpeed;

	private float startFloor;

	private AnimatorSprite animator;

	private int speedAnimation;

	private SpriteRenderer render;

	private string sortingLayerName;

	private Transform transformMove;

	private Vector3 lastPositionObject = default(Vector2);

	private void Awake()
	{
		animator = base.gameObject.GetComponent<AnimatorSprite>();
		speedAnimation = animator.speed;
		render = base.gameObject.GetComponent<SpriteRenderer>();
		startFloor = floor;
	}

	private void Start()
	{
		base.gameObject.transform.Rotate(new Vector3(0f, 0f, 1f), UnityEngine.Random.Range(0, 360));
		if (renderOnFloor != null)
		{
			renderOnFloor.gameObject.SetActive(value: false);
		}
	}

	private void OnEnable()
	{
		OnGetDrops();
	}

	private void ResetAnimation()
	{
		animator.enabled = true;
		animator.SwitchRandomAnimation();
		animator.GotoAndPlay(1);
		animator.speed = UnityEngine.Random.Range(speedAnimation - 10, speedAnimation);
		render.enabled = true;
		if (renderOnFloor != null)
		{
			renderOnFloor.gameObject.SetActive(value: false);
		}
		float value = UnityEngine.Random.value;
		base.transform.localScale = new Vector3(Mathf.Floor(Mathf.Lerp(60f, 140f, value)) / 100f, Mathf.Floor(Mathf.Lerp(140f, 60f, value)) / 100f);
		base.transform.rotation = Quaternion.Euler(0f, 0f, UnityEngine.Random.Range(-180, 180));
	}

	public override void OnGetDrops()
	{
		ResetAnimation();
		SetMove(startSpeed, UnityEngine.Random.Range((float)angle - angleScatter, (float)angle + angleScatter));
	}

	public void OnEffectHitBullet(HitBullet hitBullet)
	{
		if (hitBullet.entity != null)
		{
			Vector3 position = hitBullet.entity.transform.position;
			floor = position.y;
		}
		else
		{
			floor = startFloor;
		}
		base.transform.position = hitBullet.raycastHit.point;
		base.enabled = true;
		scaleSpeed = -1;
		if (dropsScene.Length == 0 || dropsScene[0] == null)
		{
			dropsScene = hitBullet.cartridge.effectsHits.parent.GetComponentsInChildren<Blood>(includeInactive: true);
		}
		for (int i = 0; i < dropsScene.Length; i++)
		{
			if (dropsScene[i] != this && Mathf.Abs(Vector2.Distance(dropsScene[i].transform.position, hitBullet.raycastHit.point)) <= maxDistSpeed)
			{
				scaleSpeed = Mathf.Max(dropsScene[i].scaleSpeed, scaleSpeed);
			}
		}
		scaleSpeed++;
		ResetAnimation();
		SetMove(startSpeed + addSpeedInPoint * (float)scaleSpeed, UnityEngine.Random.Range((float)angle - angleScatter, (float)angle + angleScatter));
	}

	public override void SetMove(float speed, float angle)
	{
		angle *= (float)Math.PI / 180f;
		move.x = Mathf.Cos(angle) * speed;
		move.y = Mathf.Sin(angle) * speed;
		move.x = Mathf.Floor(move.x * 100f) / 100f;
		move.y = Mathf.Floor(move.y * 100f) / 100f;
	}

	public override void SetFloor(float value)
	{
		floor = value;
	}

	public override void SetSpeedBody(Vector2 speed)
	{
		move += speed;
	}

	public void SetObjectMove(Transform transform)
	{
		transformMove = transform;
		lastPositionObject = transform.position;
	}

	private void FixedUpdate()
	{
		if (transformMove != null && lastPositionObject.magnitude != transformMove.position.magnitude)
		{
			ref Vector2 reference = ref move;
			float x = reference.x;
			Vector3 position = transformMove.position;
			reference.x = x + (position.x - lastPositionObject.x);
			ref Vector2 reference2 = ref move;
			float y = reference2.y;
			Vector3 position2 = transformMove.position;
			reference2.y = y + (position2.y - lastPositionObject.y);
			transformMove = null;
		}
		move = new Vector2(move.x + gravity.x * Time.fixedDeltaTime, move.y + gravity.y * Time.fixedDeltaTime);
		Transform transform = base.gameObject.transform;
		Vector3 position3 = base.gameObject.transform.position;
		float x2 = position3.x + move.x;
		Vector3 position4 = base.gameObject.transform.position;
		float y2 = position4.y + move.y;
		Vector3 position5 = base.gameObject.transform.position;
		transform.position = new Vector3(x2, y2, position5.z);
		Vector3 position6 = base.gameObject.transform.position;
		if (position6.y < floor)
		{
			animator.Stop();
			animator.enabled = false;
			render.enabled = false;
			base.enabled = false;
			base.gameObject.transform.rotation = default(Quaternion);
			if (renderOnFloor != null && sprites.Length != 0)
			{
				renderOnFloor.gameObject.SetActive(value: true);
				renderOnFloor.sprite = sprites[animator.currentFrame - 1];
				renderOnFloor.flipX = ((double)UnityEngine.Random.value > 0.5);
				renderOnFloor.flipY = ((double)UnityEngine.Random.value > 0.5);
				render.flipX = renderOnFloor.flipX;
				render.flipY = renderOnFloor.flipY;
			}
		}
	}

	private void OnDestroy()
	{
		dropsScene = new Blood[0];
	}

	public override BloodBase[] GetListObjects()
	{
		if (max > 0 && listDrops.Length == 0)
		{
			listDrops = new BloodBase[max];
		}
		else if (max <= 0 && listDrops.Length != 0)
		{
			listDrops = new BloodBase[0];
		}
		return listDrops;
	}
}
