using PVold;
using System;
using UnityEngine;

public class WeaponFireRocket : WeaponBaseArrow
{
	public static WeaponFireRocket[] onStage = new WeaponFireRocket[0];

	public float speed = 80f;

	public float timerLive = 3f;

	public float scaleGravity = 0.25f;

	[Tooltip("Радиус/Дистанция для попадания в живых существ.\nЕсли в пределах этого расстояния нет живых, то фиксируем попадние в трупы")]
	public float distLivedEntity = 20f;

	[Space(8f)]
	public Vector2 slideExplode = default(Vector2);

	public Material renderMaterialHit;

	public PhysicsMaterial2D floorWall;

	public float damage;

	public string eventArea = string.Empty;

	[Tooltip("Дистанция/Скорость для наклона на все 90 градусов")]
	public float speedMoveRotate = 0.1f;

	[Tooltip("Скорость вращения огня для достижения необходимого угла")]
	public float speedRotateFire = 2f;

	private float distRayHit = 1f;

	private SpriteRenderer renderBodyHit;

	private Vector3 deltaPosition = default(Vector3);

	private Vector3 lastPosition = default(Vector3);

	private float angleMove;

	private SpriteRenderer render;

	private AnimatorSprite animationSprite;

	private bool hitInDead = true;

	private Vector2 currentSpeed = default(Vector2);

	private float hitScale = 1f;

	private WeaponHands.MethodHitsBullets onHitBulletHands;

	private WeaponHands weapon;

	private WeaponCartridgeArrows cartridgeWeapon;

	private Vector3 position = default(Vector3);

	private Vector2 rayDirection = default(Vector2);

	private RaycastHit2D raycast;

	private IEffectHitBullet objectEffectHit;

	private Collider2D startHitCollider;

	private Entity entityHit;

	private float timeStepDamage;

	private Vector3 distPosition = default(Vector3);

	private float disableAngle;

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = new Color(1f, 0.7f, 0f);
		Gizmos.DrawLine(base.transform.position, base.transform.TransformPoint(distRayHit, 0f, 0f));
		Gizmos.color = new Color(0f, 0.7f, 1f);
		Gizmos.DrawLine(base.transform.position, base.transform.TransformPoint(Mathf.Cos(angleMove), Mathf.Sin(angleMove), 0f));
		if (cartridgeWeapon != null)
		{
			cartridgeWeapon.OnDrawGizmosSelectedWeapon(weapon);
		}
	}

	public override void OnShot(WeaponHands weapon, WeaponCartridgeArrows cartridge, WeaponHands.MethodHitsBullets onHitBullet)
	{
		this.weapon = weapon;
		cartridgeWeapon = cartridge;
		animationSprite = base.gameObject.GetComponent<AnimatorSprite>();
		onHitBulletHands = onHitBullet;
		currentSpeed = base.transform.right * speed * UnityEngine.Random.Range(75f, 100f) / 100f;
		timeStepDamage = timerLive * UnityEngine.Random.value;
		timerLive *= UnityEngine.Random.Range(0.8f, 1.25f);
		disableAngle = UnityEngine.Random.Range(540, 1440);
		hitScale = Mathf.Ceil(UnityEngine.Random.value * 100f) / 100f;
		float num = Mathf.Lerp(0.5f, 1.25f, hitScale);
		base.transform.localScale = new Vector3(num, num, 1f);
		animationSprite.speed = Mathf.CeilToInt(Mathf.Lerp(40f, 15f, hitScale));
		startHitCollider = Physics2D.OverlapPoint(base.transform.position);
		hitInDead = cartridge.HitToDead(base.transform.position, distLivedEntity);
		FixedUpdate();
	}

	private void FixedUpdate()
	{
		if (currentSpeed.x != 0f)
		{
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
			raycast = Physics2D.Raycast(base.gameObject.transform.position, rayDirection, distRayHit, weapon.cartridge.sourceBullets.layerBodies.value);
			if (raycast.collider != null && IsHitEntity(raycast.collider))
			{
				base.transform.position = raycast.point + Vector2.Scale(rayDirection, slideExplode);
				cartridgeWeapon.ShotArrow(this, distLivedEntity, OnHitsBulletsFire);
			}
		}
		else
		{
			deltaPosition = base.transform.position - lastPosition;
			lastPosition = base.transform.position;
			angleMove = deltaPosition.x / speedMoveRotate * 90f;
			angleMove = Mathf.Clamp(angleMove, -90f, 90f);
			Vector3 eulerAngles3 = base.transform.rotation.eulerAngles;
			float num2 = Mathf.DeltaAngle(eulerAngles3.z, angleMove);
			if (num2 > speedRotateFire / 2f)
			{
				base.transform.Rotate(0f, 0f, speedRotateFire);
			}
			else if (num2 < 0f - speedRotateFire / 2f)
			{
				base.transform.Rotate(0f, 0f, 0f - speedRotateFire);
			}
			timeStepDamage -= Time.fixedDeltaTime;
			if (timeStepDamage <= 0f && damage != 0f && entityHit != null)
			{
				entityHit.HealthDamage(damage);
				damage = 0f;
			}
		}
		timerLive -= Time.fixedDeltaTime;
		if (timerLive <= 0.25f)
		{
			timerLive = Mathf.Max(0f, timerLive);
			base.transform.localScale = new Vector3(Mathf.Lerp(1.5f, 1f, timerLive / 0.25f), Mathf.Lerp(0.2f, 1f, timerLive / 0.25f), 1f);
			base.transform.Rotate(0f, 0f, disableAngle * Time.fixedDeltaTime);
			base.transform.position += new Vector3(0f, 2.5f * Time.fixedDeltaTime, 0f);
			if (timerLive == 0f)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}
	}

	public virtual void OnHitsBulletsFire(HitBullet hit)
	{
		onHitBulletHands(hit);
		animationSprite.SwitchNextAnimation();
		animationSprite.loop = true;
		animationSprite.PlayRandomFrame();
		animationSprite.render.material = renderMaterialHit;
		currentSpeed = Vector2.zero;
		base.gameObject.name = base.gameObject.name + ".Hit-" + hit.id.ToString();
		entityHit = hit.entity;
		base.transform.position = hit.raycastHit.point;
		base.transform.SetParent(hit.raycastHit.collider.transform);
		base.transform.rotation = default(Quaternion);
		lastPosition = base.transform.position;
		renderBodyHit = hit.raycastHit.collider.GetComponent<SpriteRenderer>();
		if (renderBodyHit != null)
		{
			render = GetComponent<SpriteRenderer>();
			render.sortingLayerName = renderBodyHit.sortingLayerName;
			render.sortingOrder = renderBodyHit.sortingOrder + 20;
		}
		if (hit.entity != null)
		{
			hit.entity.HitTestArea(hit.raycastHit.point, 123456f, eventArea);
		}
		if (hit.collider.sharedMaterial == floorWall)
		{
			timerLive = 0f;
		}
		onStage = UnityEngine.Object.FindObjectsOfType<WeaponFireRocket>();
		for (int i = 0; i < onStage.Length; i++)
		{
			if (this != onStage[i] && base.transform.parent == onStage[i].transform.parent)
			{
				distPosition = base.transform.localPosition - onStage[i].transform.localPosition;
				if (distPosition.magnitude < 0.3f)
				{
					UnityEngine.Object.Destroy(onStage[i].gameObject);
				}
			}
		}
		onStage = new WeaponFireRocket[0];
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

	private void OnDestroy()
	{
		renderBodyHit = null;
	}
}
