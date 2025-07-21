using System;
using UnityEngine;

[RequireComponent(typeof(WeaponLine))]
public class Rocket : MonoBehaviour
{
	[Tooltip("Слои с физ.телами для столкновения луча RaycastHit2D")]
	public LayerMask layerBodies;

	public float distanceRayHit = 2f;

	public float slideExplode = 0.5f;

	public float timerLive = 3f;

	public float distLivedEntity = 20f;

	public float scaleGravity = 0.25f;

	[Header("Тряска камеры:")]
	public float distanceSnake = 0.1f;

	public float timeSnake = 0.5f;

	public int periodSnake = 6;

	private float speedSnake;

	private bool hitInDead = true;

	private Vector2 speedMove = default(Vector2);

	public WeaponLine.MethodHitsBullets onHitsBullets;

	[HideInInspector]
	public WeaponLine weapon;

	[HideInInspector]
	public AudioSource audioSource;

	private Vector3 position = default(Vector3);

	private Collider2D startHitCollider;

	private Entity[] entities = new Entity[0];

	private Vector3 startCamera;

	private float currentTimeSnake;

	private Entity entityHit;

	private Vector2 rayDirection = default(Vector2);

	private RaycastHit2D raycastHit = default(RaycastHit2D);

	private RaycastHit2D[] raycastAll;

	private void Awake()
	{
		weapon = base.gameObject.GetComponent<WeaponLine>();
		audioSource = base.gameObject.GetComponent<AudioSource>();
		Invoke("DeleteFromStage", timerLive);
	}

	public void OnShot(Vector2 globalPosition, Quaternion rotation, Vector2 speed)
	{
		base.transform.position = globalPosition;
		base.transform.rotation = rotation;
		speedMove = speed;
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

	private void Explode(Collider2D hitCollider)
	{
		startHitCollider = null;
		weapon.Shot(20f, onHitsBullets);
		if (audioSource != null)
		{
			audioSource.Play();
		}
		if (timeSnake != 0f)
		{
			startCamera = Camera.main.transform.localPosition;
			currentTimeSnake = distanceSnake * (float)periodSnake * (1f + UnityEngine.Random.value * 0.1f);
			speedSnake = currentTimeSnake / timeSnake;
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
			entityHit = null;
			return true;
		}
		return false;
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
		if (speedMove.x == 0f)
		{
			return;
		}
		ref Vector2 reference = ref speedMove;
		float y = reference.y;
		float num = scaleGravity;
		Vector2 gravity = Physics2D.gravity;
		reference.y = y + num * gravity.y * Time.fixedDeltaTime;
		position = base.transform.position;
		base.transform.position = new Vector3(position.x + speedMove.x * Time.fixedDeltaTime, position.y + speedMove.y * Time.fixedDeltaTime, 0f);
		WeaponLine.ActivateTriggers();
		ref Vector2 reference2 = ref rayDirection;
		Vector3 eulerAngles = base.gameObject.transform.rotation.eulerAngles;
		reference2.x = Mathf.Cos((float)Math.PI / 180f * eulerAngles.z);
		ref Vector2 reference3 = ref rayDirection;
		Vector3 eulerAngles2 = base.gameObject.transform.rotation.eulerAngles;
		reference3.y = Mathf.Sin((float)Math.PI / 180f * eulerAngles2.z);
		raycastAll = Physics2D.RaycastAll(base.gameObject.transform.position, rayDirection, distanceRayHit, layerBodies.value);
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
			speedMove = Vector2.zero;
			base.transform.position = raycastHit.point - rayDirection * slideExplode;
			Explode(raycastHit.collider);
		}
	}

	public void DeleteFromStage()
	{
		WeaponLine.DeactivateTriggers();
		UnityEngine.Object.Destroy(base.gameObject);
	}
}
