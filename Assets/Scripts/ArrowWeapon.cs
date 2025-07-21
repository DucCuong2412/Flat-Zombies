using System;
using UnityEngine;

public class ArrowWeapon : MonoBehaviour
{
	[Tooltip("Слои с физ.телами для столкновения луча RaycastHit2D")]
	public LayerMask layerBodies;

	public PhysicsMaterial2D materialBody;

	public float speed = 80f;

	public float distanceHit = 1f;

	public float timerLive = 3f;

	public float scaleGravity = 0.25f;

	public float distLivedEntity = 20f;

	public float floor;

	public Vector2 pointTip = default(Vector2);

	public Sprite spriteHit;

	public WeaponLine.MethodHitsBullets onHitsBullets;

	[HideInInspector]
	public WeaponLine weapon;

	private Collider2D startHitCollider;

	private SpriteRenderer render;

	private SpriteRenderer renderBody;

	private Rigidbody2D body;

	private Collider2D[] colliders;

	private bool hitInDead = true;

	private bool isHited;

	private Entity entityHit;

	private Vector2 currentSpeed = default(Vector2);

	private Entity[] entities = new Entity[0];

	private Vector3 position = default(Vector3);

	private Vector2 rayDirection = default(Vector2);

	private RaycastHit2D raycastHit;

	private RaycastHit2D[] raycastAll;

	private GameObject objectHit;

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = new Color(1f, 0.05f, 0f);
		Gizmos.DrawWireSphere(base.transform.TransformPoint(pointTip), 0.1f);
	}

	private void Start()
	{
		weapon = base.gameObject.GetComponent<WeaponLine>();
		weapon.distance = distanceHit + 1f;
		body = base.gameObject.GetComponent<Rigidbody2D>();
		body.simulated = false;
		colliders = base.gameObject.GetComponents<Collider2D>();
		render = base.gameObject.GetComponent<SpriteRenderer>();
		Invoke("DeleteFromStage", timerLive);
		FixedUpdate();
	}

	public void OnShot(Vector3 globalPosition, Quaternion rotation)
	{
		base.transform.position = globalPosition;
		base.transform.rotation = rotation;
		currentSpeed = base.transform.right * speed;
		startHitCollider = Physics2D.OverlapPoint(globalPosition);
		entities = UnityEngine.Object.FindObjectsOfType<Entity>();
		for (int i = 0; i < entities.Length; i++)
		{
			if (!entities[i].isDead && Mathf.Abs(Vector3.Distance(globalPosition, entities[i].transform.position)) < distLivedEntity)
			{
				hitInDead = false;
				break;
			}
		}
		entities = new Entity[0];
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
		ref Vector2 reference = ref currentSpeed;
		float y = reference.y;
		float num = scaleGravity;
		Vector2 gravity = Physics2D.gravity;
		reference.y = y + num * gravity.y * Time.fixedDeltaTime;
		position = base.transform.position;
		base.transform.position = new Vector3(position.x + currentSpeed.x * Time.fixedDeltaTime, position.y + currentSpeed.y * Time.fixedDeltaTime, 0f);
		WeaponLine.ActivateTriggers();
		ref Vector2 reference2 = ref rayDirection;
		Vector3 eulerAngles = base.gameObject.transform.rotation.eulerAngles;
		reference2.x = Mathf.Cos((float)Math.PI / 180f * eulerAngles.z);
		ref Vector2 reference3 = ref rayDirection;
		Vector3 eulerAngles2 = base.gameObject.transform.rotation.eulerAngles;
		reference3.y = Mathf.Sin((float)Math.PI / 180f * eulerAngles2.z);
		raycastAll = Physics2D.RaycastAll(base.gameObject.transform.position, rayDirection, distanceHit, layerBodies.value);
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
			weapon.Shot(30f, OnHitsBulletsArrow);
		}
	}

	public void OnHitsBulletsArrow(HitBullet hit)
	{
		if (hit.raycastHit.collider.sharedMaterial.name == materialBody.name && entityHit != null)
		{
			weapon.damage = 3f;
			base.gameObject.name = base.gameObject.name + "Hit";
			renderBody = hit.raycastHit.collider.GetComponent<SpriteRenderer>();
			if (renderBody != null)
			{
				render.sprite = spriteHit;
				render.sortingLayerName = renderBody.sortingLayerName;
				render.sortingOrder = renderBody.sortingOrder - 1;
			}
			UnityEngine.Object.Destroy(body);
			for (int i = 0; i < colliders.Length; i++)
			{
				UnityEngine.Object.Destroy(colliders[i]);
			}
			colliders = new Collider2D[0];
			base.transform.position = hit.raycastHit.point;
			base.transform.SetParent(hit.raycastHit.collider.transform);
			CancelInvoke("DeleteFromStage");
			WeaponLine.DeactivateTriggers();
			isHited = true;
			Vector3 vector = entityHit.transform.position;
			floor = vector.y - 0.25f;
		}
		else
		{
			base.enabled = false;
			body.simulated = true;
			body.AddForce(-currentSpeed);
			body.AddTorque(UnityEngine.Random.Range(-90, 90));
		}
		if (onHitsBullets != null)
		{
			onHitsBullets(hit);
		}
		UnityEngine.Object.Destroy(weapon);
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
		WeaponLine.DeactivateTriggers();
		UnityEngine.Object.Destroy(base.gameObject);
	}
}
