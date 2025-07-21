using System;
using UnityEngine;

public class WeaponArrow : WeaponBaseArrow
{
	public PhysicsMaterial2D materialBody;

	public float speed = 80f;

	public float timerLive = 3f;

	public float scaleGravity = 0.25f;

	[Tooltip("Дистанция для попадания в живых существ.\nЕсли в пределах этого расстояния нет живых, то фиксируем попадние в трупы")]
	public float distLivedEntity = 20f;

	public float floor;

	public Vector2 pointTip = default(Vector2);

	public Sprite spriteHit;

	public WeaponHands.MethodHitsBullets onHitBulletHands;

	private float distRayHit;

	private WeaponHands weapon;

	private WeaponCartridgeArrows cartridgeWeapon;

	private Collider2D startHitCollider;

	private SpriteRenderer render;

	private SpriteRenderer renderBody;

	private Rigidbody2D body;

	private Collider2D[] colliders;

	private bool hitInDead = true;

	private bool isHited;

	private Entity entityHit;

	private Vector2 currentSpeed = default(Vector2);

	private Vector3 position = default(Vector3);

	private Vector2 rayDirection = default(Vector2);

	private RaycastHit2D raycastHit;

	private RaycastHit2D[] raycastAll;

	private GameObject objectHit;

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = new Color(1f, 0.7f, 0f);
		Gizmos.DrawLine(base.transform.position, base.transform.TransformPoint(distRayHit, 0f, 0f));
		Gizmos.color = new Color(1f, 0.05f, 0f);
		Gizmos.DrawWireSphere(base.transform.TransformPoint(pointTip), 0.1f);
		if (cartridgeWeapon != null)
		{
			cartridgeWeapon.OnDrawGizmosSelectedWeapon(weapon);
		}
	}

	public override void OnShot(WeaponHands weapon, WeaponCartridgeArrows cartridge, WeaponHands.MethodHitsBullets onHitBullet)
	{
		cartridgeWeapon = cartridge;
		body = base.gameObject.GetComponent<Rigidbody2D>();
		body.simulated = false;
		colliders = base.gameObject.GetComponents<Collider2D>();
		render = base.gameObject.GetComponent<SpriteRenderer>();
		Invoke("DeleteFromStage", timerLive);
		this.weapon = weapon;
		onHitBulletHands = onHitBullet;
		currentSpeed = base.transform.right * speed;
		startHitCollider = Physics2D.OverlapPoint(base.transform.position);
		hitInDead = cartridge.HitToDead(base.transform.position, distLivedEntity);
		FixedUpdate();
	}

	private void FixedUpdate()
	{
		if (isHited)
		{
			position = base.transform.TransformPoint(pointTip);
			if (position.y < floor)
			{
				base.transform.Translate(floor - position.y, 0f, 0f);
			}
			return;
		}
		cartridgeWeapon.ActivateTriggers();
		ref Vector2 reference = ref currentSpeed;
		float y = reference.y;
		float num = scaleGravity;
		Vector2 gravity = Physics2D.gravity;
		reference.y = y + num * gravity.y * Time.fixedDeltaTime;
		position = base.transform.position;
		base.transform.position = new Vector3(position.x + currentSpeed.x * Time.fixedDeltaTime, position.y + currentSpeed.y * Time.fixedDeltaTime, 0f);
		distRayHit = speed * Time.fixedDeltaTime;
		ref Vector2 reference2 = ref rayDirection;
		Vector3 eulerAngles = base.gameObject.transform.rotation.eulerAngles;
		reference2.x = Mathf.Cos((float)Math.PI / 180f * eulerAngles.z);
		ref Vector2 reference3 = ref rayDirection;
		Vector3 eulerAngles2 = base.gameObject.transform.rotation.eulerAngles;
		reference3.y = Mathf.Sin((float)Math.PI / 180f * eulerAngles2.z);
		raycastAll = Physics2D.RaycastAll(base.gameObject.transform.position, rayDirection, distRayHit, weapon.cartridge.sourceBullets.layerBodies.value);
		for (int i = 0; i < raycastAll.Length; i++)
		{
			if (raycastAll[i].collider != null && IsHitEntity(raycastAll[i].collider))
			{
				raycastHit = raycastAll[i];
				break;
			}
			raycastHit = default(RaycastHit2D);
		}
		if (raycastHit.collider != null)
		{
			base.transform.position = raycastHit.point - rayDirection;
			cartridgeWeapon.ShotArrow(this, distLivedEntity, OnHitsBulletsArrow);
		}
	}

	public virtual void OnHitsBulletsArrow(HitBullet hit)
	{
		if (hit.raycastHit.collider.sharedMaterial.name == materialBody.name && entityHit != null)
		{
			UnityEngine.Object.Destroy(body);
			for (int i = 0; i < colliders.Length; i++)
			{
				UnityEngine.Object.Destroy(colliders[i]);
			}
			colliders = new Collider2D[0];
			CancelInvoke("DeleteFromStage");
			isHited = true;
			Vector3 vector = entityHit.transform.position;
			floor = vector.y - 0.25f;
			OnHitEntity(hit);
		}
		else if (entityHit != null)
		{
			OnHitArmor();
		}
		else
		{
			OnHitGround();
		}
		onHitBulletHands(hit);
	}

	public virtual void OnHitEntity(HitBullet hit)
	{
		base.gameObject.name = base.gameObject.name + ".Hit";
		base.transform.position = hit.raycastHit.point;
		base.transform.SetParent(hit.raycastHit.collider.transform);
		renderBody = hit.raycastHit.collider.GetComponent<SpriteRenderer>();
		if (renderBody != null)
		{
			render.sprite = spriteHit;
			render.sortingLayerName = renderBody.sortingLayerName;
			render.sortingOrder = renderBody.sortingOrder - 1;
		}
	}

	public virtual void OnHitGround()
	{
		OnHitArmor();
	}

	public virtual void OnHitArmor()
	{
		base.enabled = false;
		body.simulated = true;
		body.AddForce(-currentSpeed);
		body.AddTorque(UnityEngine.Random.Range(-90, 90));
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

	public void DeleteFromStage()
	{
		CancelInvoke("DeleteFromStage");
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private void OnDestroy()
	{
	}
}
