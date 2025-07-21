using System;
using UnityEngine;

public class Meat : BloodBase, IEffectHitBullet
{
	private const float TIME_SLIDE = 0.5f;

	public static int max = 10;

	public static BloodBase[] listDrops = new BloodBase[0];

	public Sprite[] skin = new Sprite[0];

	public int angle;

	[Range(0f, 180f)]
	public float angleScatter;

	public float startSpeed;

	public int speedRotateMax;

	public Vector3 move = default(Vector3);

	public Vector2 gravity = default(Vector2);

	public float floor;

	public SpriteRenderer renderOnFloor;

	private SpriteRenderer render;

	private float speedRotate;

	private float timeSlideOnFloor = 0.5f;

	private int startLayer;

	private int startOrder;

	private void Start()
	{
	}

	public override void OnGetDrops()
	{
		if (render == null)
		{
			render = base.gameObject.GetComponent<SpriteRenderer>();
			startLayer = render.sortingLayerID;
			startOrder = render.sortingOrder;
		}
		render.sprite = skin[UnityEngine.Random.Range(0, skin.Length)];
		base.transform.Rotate(0f, 0f, UnityEngine.Random.Range(0, 360));
		render.enabled = true;
		render.sortingLayerID = startLayer;
		render.sortingOrder = startOrder;
		renderOnFloor.gameObject.SetActive(value: false);
		renderOnFloor.flipX = ((double)UnityEngine.Random.value > 0.5);
		renderOnFloor.flipY = ((double)UnityEngine.Random.value > 0.5);
		render.flipX = ((double)UnityEngine.Random.value > 0.5);
		render.flipY = ((double)UnityEngine.Random.value > 0.5);
		speedRotate = UnityEngine.Random.Range(speedRotateMax / 2, speedRotateMax);
		timeSlideOnFloor = 0.5f;
		SetMove(startSpeed, UnityEngine.Random.Range((float)angle - angleScatter, (float)angle + angleScatter));
	}

	public void OnEffectHitBullet(HitBullet hitBullet)
	{
		base.transform.position = hitBullet.raycastHit.point;
		Vector3 position = hitBullet.raycastHit.collider.GetComponentInParent<Entity>().transform.position;
		floor = position.y;
		base.enabled = true;
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
		floor = value + (0.6f - UnityEngine.Random.value);
		floor = Mathf.Floor(floor * 1000f) / 1000f;
	}

	public override void SetSpeedBody(Vector2 speed)
	{
		move.x += speed.x;
		move.y += speed.y;
	}

	private void FixedUpdate()
	{
		Vector3 position = base.gameObject.transform.position;
		if (position.y > floor)
		{
			move.x += gravity.x * Time.fixedDeltaTime;
			move.y += gravity.y * Time.fixedDeltaTime;
			base.transform.position += move;
			base.transform.Rotate(0f, 0f, speedRotate * Time.fixedDeltaTime);
			return;
		}
		if (!renderOnFloor.gameObject.activeSelf)
		{
			renderOnFloor.gameObject.SetActive(value: true);
			renderOnFloor.transform.SetParent(base.gameObject.transform.parent);
			renderOnFloor.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
			renderOnFloor.transform.position = base.gameObject.transform.position;
			renderOnFloor.transform.localScale = base.gameObject.transform.localScale;
			float num = Mathf.Floor(1000f * (render.sprite.rect.height / render.sprite.pixelsPerUnit)) / 1000f;
			renderOnFloor.transform.Translate(move.x * 2f, (0f - num) / 2f, 0f);
			render.sortingLayerID = renderOnFloor.sortingLayerID;
			render.sortingOrder = renderOnFloor.sortingOrder;
			move.x *= 0.25f;
		}
		base.transform.position += new Vector3(move.x * (timeSlideOnFloor / 0.5f), 0f, 0f);
		timeSlideOnFloor -= Time.fixedDeltaTime;
		if (timeSlideOnFloor <= 0f)
		{
			base.enabled = false;
		}
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
