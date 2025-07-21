using PVold;
using System;
using UnityEngine;

public class WeaponRocket : WeaponBaseArrow
{
	public static AnimatorSprite objectHitStage = null;

	public static AnimatorSprite[] animationExplode = new AnimatorSprite[0];

	public float speed = 80f;

	public float timerLive = 3f;

	public float scaleGravity = 0.25f;

	[Tooltip("Дистанция для попадания в живых существ.\nЕсли в пределах этого расстояния нет живых, то фиксируем попадние в трупы")]
	public float distLivedEntity = 20f;

	[Space(8f)]
	[Header("Тряска камеры:")]
	public float distanceSnake = 0.1f;

	public float timeSnake = 0.5f;

	public int periodSnake = 6;

	public AudioClip soundHit;

	public Vector2 slideExplode = default(Vector2);

	public AnimatorSprite objectExplode;

	private float speedSnake;

	private Vector3 startCamera = default(Vector3);

	private float currentTimeSnake;

	private bool hitInDead = true;

	private Vector2 currentSpeed = default(Vector2);

	private WeaponHands.MethodHitsBullets onHitBulletHands;

	private WeaponHands weapon;

	private WeaponCartridgeArrows cartridgeWeapon;

	private Vector3 position = default(Vector3);

	private Vector2 rayDirection = default(Vector2);

	private float distRayHit = 1.5f;

	private RaycastHit2D raycastHit;

	private RaycastHit2D[] raycastAll;

	private Collider2D startHitCollider;

	private Entity entityHit;

	public override void OnShot(WeaponHands weapon, WeaponCartridgeArrows cartridge, WeaponHands.MethodHitsBullets onHitBullet)
	{
		this.weapon = weapon;
		cartridgeWeapon = cartridge;
		onHitBulletHands = onHitBullet;
		base.transform.position = weapon.GetPositionBolt();
		base.transform.rotation = Quaternion.Euler(0f, 0f, weapon.GetAngleBolt());
		currentSpeed = base.transform.right * speed;
		startHitCollider = Physics2D.OverlapPoint(base.transform.position);
		hitInDead = cartridge.HitToDead(base.transform.position, distLivedEntity);
		FixedUpdate();
	}

	private void FixedUpdate()
	{
		if (currentTimeSnake > 0f)
		{
			Camera.main.transform.localPosition = new Vector3(startCamera.x + Mathf.PingPong(currentTimeSnake, distanceSnake), startCamera.y + Mathf.PingPong(currentTimeSnake, distanceSnake * 1.3f), startCamera.z);
			currentTimeSnake -= speedSnake * Time.fixedDeltaTime;
		}
		else if (currentTimeSnake < 0f)
		{
			currentTimeSnake = 0f;
			Camera.main.transform.localPosition = startCamera;
		}
		if (currentSpeed.x != 0f)
		{
			cartridgeWeapon.ActivateTriggers();
			ref Vector2 reference = ref currentSpeed;
			float y = reference.y;
			float num = scaleGravity;
			Vector2 gravity = Physics2D.gravity;
			reference.y = y + num * gravity.y * Time.fixedDeltaTime;
			position = base.transform.position;
			base.transform.position = new Vector3(position.x + currentSpeed.x * Time.fixedDeltaTime, position.y + currentSpeed.y * Time.fixedDeltaTime, 0f);
			ref Vector2 reference2 = ref rayDirection;
			Vector3 eulerAngles = base.gameObject.transform.rotation.eulerAngles;
			reference2.x = Mathf.Cos((float)Math.PI / 180f * eulerAngles.z);
			ref Vector2 reference3 = ref rayDirection;
			Vector3 eulerAngles2 = base.gameObject.transform.rotation.eulerAngles;
			reference3.y = Mathf.Sin((float)Math.PI / 180f * eulerAngles2.z);
			distRayHit = Mathf.Ceil(speed * Time.fixedDeltaTime / 0.5f) * 0.5f;
			raycastAll = Physics2D.RaycastAll(base.gameObject.transform.position, rayDirection, distRayHit, weapon.cartridge.sourceBullets.layerBodies.value);
			for (int i = 0; i < raycastAll.Length; i++)
			{
				if (raycastAll[i].collider != null && IsHitEntity(raycastAll[i].collider))
				{
					currentSpeed = Vector2.zero;
					base.transform.position = raycastAll[i].point - Vector2.Scale(rayDirection, slideExplode);
					cartridgeWeapon.ShotArrow(this, distLivedEntity, onHitBulletHands);
					EffectHit();
					startHitCollider = null;
					break;
				}
			}
		}
		timerLive -= Time.fixedDeltaTime;
		if (timerLive <= 0f)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private void EffectHit()
	{
		GetComponent<SpriteRenderer>().enabled = false;
		startCamera = Camera.main.transform.localPosition;
		currentTimeSnake = distanceSnake * (float)periodSnake * (1f + UnityEngine.Random.value * 0.1f);
		speedSnake = currentTimeSnake / timeSnake;
		weapon.audioSource.PlayOneShot(soundHit);
		if (objectHitStage == null)
		{
			objectHitStage = UnityEngine.Object.Instantiate(objectExplode);
			animationExplode = objectHitStage.GetComponentsInChildren<AnimatorSprite>(includeInactive: true);
		}
		objectHitStage.gameObject.SetActive(value: true);
		objectHitStage.transform.position = base.transform.position;
		for (int i = 0; i < animationExplode.Length; i++)
		{
			animationExplode[i].gameObject.SetActive(value: true);
			animationExplode[i].enabled = true;
			animationExplode[i].GotoAndPlay(1);
			animationExplode[i].transform.Rotate(0f, 0f, UnityEngine.Random.Range(0, 360));
		}
	}

	private bool IsHitEntity(Collider2D trigger)
	{
		if (startHitCollider != null && startHitCollider.gameObject == trigger.gameObject)
		{
			return false;
		}
		entityHit = trigger.GetComponentInParent<Entity>();
		if (entityHit == null)
		{
			return true;
		}
		if (hitInDead)
		{
			return true;
		}
		if (!hitInDead && !entityHit.isDead)
		{
			return true;
		}
		return false;
	}
}
