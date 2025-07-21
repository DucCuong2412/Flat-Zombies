using System;
using UnityEngine;
using UnityEngine.Events;

public class WeaponLine : MonoBehaviour
{
	public delegate void MethodHitsBullets(HitBullet hit);

	[Serializable]
	public struct HitsBullet
	{
		[Tooltip("Минимальное кол-во попаданий, которое будет выбрано случайно в диапозоне минимального и максимального для одной пули.")]
		public int numsMin;

		[Tooltip("Максимальное кол-во попаданий для одной пули.")]
		public int numsMax;

		[Tooltip("Минимальная дистанция между попаданиями")]
		public float distanceMin;

		[Tooltip("Максимальная дистанция между попаданиями")]
		public float distanceMax;

		public int GetRandomNumHits()
		{
			return UnityEngine.Random.Range(numsMin, numsMax + 1);
		}
	}

	public const float maxDeltaTime = 0.04f;

	public static int numShots;

	public static int numAfterShots;

	public static Entity[] entities;

	[HideInInspector]
	public float recoil;

	[Tooltip("Скорость увеличения отдачи, в градусах")]
	[Range(0f, 10f)]
	public float recoilSpeed;

	[Tooltip("Максимальный угол отдачи, в градусах")]
	[Range(0f, 10f)]
	public float recoilMax;

	[Tooltip("Скорость уменьшения отдачи, в градусы/секунду")]
	[Range(0f, 180f)]
	public float recoilDecrease;

	[Tooltip("Угол отклонения при стрельбе")]
	[Range(0f, 10f)]
	public float direction;

	[HideInInspector]
	public float currentDirection;

	[Space(10f)]
	[Tooltip("Автоматическое оружие")]
	public bool automat;

	[Tooltip("Скорострельность. Кол-во выстрелов в минуту")]
	public int shotsInMinute = 750;

	[Tooltip("Кол-во патронов в одном магазине,\n0 - бесконечно")]
	public int magazineMax = 30;

	[Tooltip("Задержка перед запуском пули после нажатия на курок, в скндх.")]
	public float delayBullet;

	[Tooltip("Высота ствола")]
	public float boltX;

	public float boltY;

	[Range(0f, 360f)]
	public float boltAngle;

	[Space(10f)]
	[Tooltip("Кол-в во пуль при одном выстреле")]
	public int bullets = 1;

	[Tooltip("Максмлн отклонение пули в градусах, для создания разброса")]
	[Range(0f, 180f)]
	public float angleScatter;

	public bool evenlySpread;

	[Tooltip("Наносимый урон от всех пуль (будет распределен между всеми пулями)")]
	public float damage = 1f;

	[Tooltip("Импульс (будет распределен между всеми пулями)")]
	public float impulse;

	[Tooltip("Дистанция луча")]
	public float distance = 10f;

	[Tooltip("Параметры попадания для одной пули. Запись попадания пули, если оно проиcходит в разные игровые объекты, которые находятся в разных родительских объектах")]
	public HitsBullet hitsBullet = default(HitsBullet);

	[Tooltip("Дополнительное снижение силы после каждого пересечения тела")]
	public PhysicsMaterialMultiply[] damageMultiply = new PhysicsMaterialMultiply[0];

	public SoundHitBullet[] soundsHit;

	public AudioClip audioShot;

	[HideInInspector]
	public AudioSource audioSource;

	[Space(10f)]
	[Tooltip("Тег игровых объектов существ, с которыми искать попадание пуль (необязательно)")]
	public string tagOfEntity;

	[Tooltip("Слои с физ.телами для столкновения луча RaycastHit2D")]
	public LayerMask layerBodies;

	[Space(5f)]
	[Tooltip("Список функций, вызываемых при выстреле")]
	public UnityEvent shot = new UnityEvent();

	private int magazCurrent = 1000000;

	private float lag;

	[SerializeField]
	private float currentDelayBullet;

	private float pause;

	private bool triggerShotWeapon = true;

	private RaycastHit2D[] hitRay;

	private Transform transfromObject;

	private float distAimOnShot;

	private Ray rayGizmos = default(Ray);

	private Vector2 globalPosition = default(Vector2);

	private float angleRad;

	private HitBullet[] hits;

	private Vector2 rayPosition = default(Vector2);

	private GameObject objectHit;

	private Collider2D[] collidersOverlap;

	private RaycastHit2D raycastHit;

	private Entity entity;

	private Vector2 rayDirection = default(Vector2);

	private GameObject prefab;

	private float timePlaySound;

	public int magazine
	{
		get
		{
			magazCurrent = Mathf.Min(magazineMax, magazCurrent);
			if (magazineMax == 0)
			{
				magazCurrent = 1;
			}
			return magazCurrent;
		}
		set
		{
			magazineMax = Mathf.Max(0, magazineMax);
			magazCurrent = Mathf.Min(value, magazineMax);
			if (magazineMax == 0)
			{
				magazCurrent = 1;
			}
		}
	}

	public static void ActivateTriggers()
	{
		if (numShots == numAfterShots)
		{
			numShots++;
			entities = UnityEngine.Object.FindObjectsOfType<Entity>();
			for (int i = 0; i < entities.Length; i++)
			{
				entities[i].SetActiveTriggers(value: true);
			}
		}
	}

	public static void DeactivateTriggers()
	{
		if (numAfterShots != numShots)
		{
			numAfterShots++;
			for (int i = 0; i < entities.Length; i++)
			{
				entities[i].SetActiveTriggers(value: false);
			}
		}
	}

	private void OnDrawGizmos()
	{
		if (!base.enabled)
		{
			return;
		}
		Vector3 vector = rayGizmos.direction;
		if (vector.x != 0f)
		{
			Gizmos.color = new Color(1f, 0.4f, 0f);
			Gizmos.DrawLine(rayGizmos.origin, rayGizmos.origin + rayGizmos.direction * 20f);
			Gizmos.color = new Color(1f, 0.9f, 0f);
			Gizmos.DrawWireSphere(rayGizmos.origin + rayGizmos.direction * distAimOnShot, 0.16f);
		}
		if (hits == null)
		{
			return;
		}
		Gizmos.color = new Color(1f, 0.4f, 0f);
		for (int i = 0; i < hits.Length; i++)
		{
			if (hits[i].weapon != null)
			{
				Gizmos.DrawWireSphere(hits[i].raycastHit.point, 0.2f);
				Gizmos.DrawLine(hits[i].raycastHit.point, hits[i].raycastHit.point + hits[i].raycastHit.normal);
			}
		}
	}

	private void OnDrawGizmosSelected()
	{
		if (!base.enabled)
		{
			return;
		}
		Gizmos.color = new Color(1f, 0.2f, 0f);
		transfromObject = base.gameObject.transform;
		globalPosition = transfromObject.TransformPoint(boltX, boltY, 0f);
		float num = boltAngle;
		Vector3 eulerAngles = transfromObject.rotation.eulerAngles;
		float num2 = num + eulerAngles.z;
		angleRad = num2 * ((float)Math.PI / 180f);
		Vector2 b = new Vector2(Mathf.Cos(angleRad) * distance, Mathf.Sin(angleRad) * distance);
		if (distance <= 0f)
		{
			Gizmos.DrawLine(globalPosition, transfromObject.TransformPoint(boltX + 3f, boltY, 0f));
		}
		else
		{
			Gizmos.DrawLine(globalPosition, globalPosition + b);
			Gizmos.DrawWireSphere(globalPosition + b, 0.2f);
			Gizmos.color = new Color(1f, 0.5f, 0f);
		}
		b.x *= 0.5f;
		b.y *= 0.5f;
		Gizmos.color = new Color(0f, 1f, 0.1f);
		Gizmos.DrawWireSphere(globalPosition + b, 0.2f);
		b.x += hitsBullet.distanceMin * Mathf.Cos(angleRad);
		b.y += hitsBullet.distanceMin * Mathf.Sin(angleRad);
		Gizmos.DrawWireSphere(globalPosition + b, 0.15f);
		Gizmos.DrawLine(globalPosition + b, new Vector3(globalPosition.x + b.x + hitsBullet.distanceMax * Mathf.Cos(angleRad), globalPosition.y + b.y + hitsBullet.distanceMax * Mathf.Sin(angleRad)));
		if (angleScatter != 0f)
		{
			Gizmos.color = new Color(1f, 0.5f, 0f);
			angleRad -= angleScatter * ((float)Math.PI / 180f);
			float num3 = 30f;
			if (distance != 0f)
			{
				num3 = distance;
			}
			Gizmos.DrawLine(globalPosition, new Vector2(globalPosition.x + Mathf.Cos(angleRad) * num3, globalPosition.y + Mathf.Sin(angleRad) * num3));
			angleRad += angleScatter * ((float)Math.PI / 180f) * 2f;
			Gizmos.DrawLine(globalPosition, new Vector2(globalPosition.x + Mathf.Cos(angleRad) * num3, globalPosition.y + Mathf.Sin(angleRad) * num3));
			float num4 = 0f;
			float num5 = 0f;
			while (bullets > 1 && num5 < (float)bullets && num5 < 90f)
			{
				num4 = num2 - angleScatter + angleScatter * 2f * (num5 / (float)bullets);
				num4 = ((!evenlySpread) ? (num2 - angleScatter + UnityEngine.Random.value * angleScatter * 2f) : (num4 + angleScatter / (float)bullets));
				num4 *= (float)Math.PI / 180f;
				Gizmos.DrawLine(globalPosition, new Vector2(globalPosition.x + Mathf.Cos(num4) * num3, globalPosition.y + Mathf.Sin(num4) * num3));
				num5 += 1f;
			}
		}
		float num6 = shotsInMinute;
		num6 = num6 / 60f * recoilSpeed;
		num6 = Mathf.Floor(num6 * 100f) / 100f;
		recoilDecrease = Mathf.Clamp(recoilDecrease, 0f, num6);
	}

	private void Awake()
	{
		audioSource = base.gameObject.GetComponentInParent<AudioSource>();
		transfromObject = base.gameObject.transform;
		lag = 1f / ((float)shotsInMinute / 60f);
		lag = Mathf.Round(lag * 100000f) / 100000f;
		recoilMax = Mathf.Max(recoilMax, recoilSpeed);
		bullets = Mathf.Max(1, bullets);
		damage /= bullets;
		impulse /= bullets;
		hitsBullet.numsMin = Mathf.Max(1, hitsBullet.numsMin);
		hitsBullet.numsMax = Mathf.Max(1, hitsBullet.numsMax);
	}

	private void Start()
	{
	}

	private void OnDisable()
	{
		DeactivateTriggers();
	}

	private void OnDestroy()
	{
		DeactivateTriggers();
	}

	public bool IsReady()
	{
		if (pause < 0f)
		{
			pause += Time.deltaTime;
		}
		return pause >= 0f && magazine > 0;
	}

	public float GetRecoil(float deltaTime)
	{
		deltaTime = Mathf.Min(0.04f, deltaTime);
		recoil = Mathf.Max(recoil - recoilDecrease * deltaTime, 0f);
		return recoil;
	}

	public bool IsReady(bool trigger)
	{
		if (pause < 0f)
		{
			pause += Time.deltaTime;
		}
		if (trigger == triggerShotWeapon)
		{
			triggerShotWeapon = true;
		}
		else
		{
			trigger = false;
		}
		trigger = (trigger && base.enabled && pause >= 0f && magazine > 0);
		if (currentDelayBullet > 0f)
		{
			currentDelayBullet -= Time.deltaTime;
			trigger = false;
			if (currentDelayBullet <= 0f)
			{
				currentDelayBullet = 0f;
				trigger = true;
			}
		}
		else if (trigger && delayBullet != 0f)
		{
			currentDelayBullet = Mathf.Abs(delayBullet);
			shot.Invoke();
			trigger = false;
		}
		return trigger && triggerShotWeapon;
	}

	public void Shot(float distToAim, MethodHitsBullets onHitsBulletsSource)
	{
		ActivateTriggers();
		if (audioSource != null)
		{
			audioSource.clip = audioShot;
			audioSource.Play();
		}
		distAimOnShot = distToAim;
		hits = new HitBullet[bullets * hitsBullet.numsMax];
		int num = 0;
		int num2 = 0;
		currentDirection = 0f - direction + Mathf.Ceil(UnityEngine.Random.value * direction * 2f);
		pause = 0f - lag;
		recoil = Mathf.Min(recoil + recoilSpeed, recoilMax + currentDirection);
		if (hitsBullet.distanceMax <= hitsBullet.distanceMin)
		{
			hitsBullet.distanceMax = distance;
		}
		float num3 = 0f;
		float num4 = 0f;
		float num5 = boltAngle;
		Vector3 eulerAngles = transfromObject.rotation.eulerAngles;
		float num6 = num5 + eulerAngles.z;
		rayPosition = transfromObject.TransformPoint(boltX, boltY, 0f);
		collidersOverlap = Physics2D.OverlapPointAll(rayPosition, layerBodies.value);
		for (int i = 0; i < bullets; i++)
		{
			num3 = 0f;
			objectHit = null;
			num2 = 0;
			if (evenlySpread)
			{
				num4 = num6 - angleScatter + angleScatter * 2f * (float)(i / bullets);
				num4 += angleScatter / (float)bullets;
			}
			else
			{
				num4 = num6 - angleScatter + UnityEngine.Random.value * angleScatter * 2f;
			}
			rayDirection = new Vector2(Mathf.Cos((float)Math.PI / 180f * num4), Mathf.Sin((float)Math.PI / 180f * num4));
			ref Vector2 reference = ref rayDirection;
			float x = rayDirection.x;
			Vector3 lossyScale = transfromObject.lossyScale;
			reference.x = x * lossyScale.x;
			ref Vector2 reference2 = ref rayDirection;
			float y = rayDirection.y;
			Vector3 lossyScale2 = transfromObject.lossyScale;
			reference2.y = y * lossyScale2.x;
			rayGizmos = new Ray(rayPosition, rayDirection);
			hitRay = Physics2D.RaycastAll(rayPosition, rayDirection, distance, layerBodies.value);
			int randomNumHits = hitsBullet.GetRandomNumHits();
			int num7 = 0;
			for (int j = 0; j < collidersOverlap.Length; j++)
			{
				for (num7 = 0; num7 < hitRay.Length - 1; num7++)
				{
					if (hitRay[num7].collider == collidersOverlap[j])
					{
						hitRay[num7] = hitRay[num7 + 1];
					}
				}
			}
			int num8 = 0;
			int num9 = 0;
			for (num7 = 0; num7 < hitRay.Length - num8; num7++)
			{
				if (!(hitRay[num7].collider.sharedMaterial != null))
				{
					continue;
				}
				entity = hitRay[num7].collider.GetComponentInParent<Entity>();
				if (entity != null && Mathf.Floor(entity.timeDeath * 100f) != Mathf.Floor(Time.time * 100f) && entity.IsLowPriorityBullet(hitRay, hitRay[num7].distance < distAimOnShot))
				{
					raycastHit = hitRay[num7];
					for (num9 = num7; num9 < hitRay.Length - 1; num9++)
					{
						hitRay[num9] = hitRay[num9 + 1];
					}
					hitRay[hitRay.Length - 1] = raycastHit;
					num7--;
					num8++;
				}
			}
			if (hitRay.Length >= 2 && UnityEngine.Random.value <= 0.5f)
			{
				entity = hitRay[0].collider.GetComponentInParent<Entity>();
				if (entity != null && entity == hitRay[1].collider.GetComponentInParent<Entity>() && hitRay[0].collider.offset == hitRay[1].collider.offset && hitRay[0].collider.gameObject != hitRay[1].collider.gameObject && hitRay[0].collider.sharedMaterial == hitRay[1].collider.sharedMaterial)
				{
					raycastHit = hitRay[0];
					hitRay[0] = hitRay[1];
					hitRay[1] = raycastHit;
				}
			}
			for (num7 = 1; num7 < hitRay.Length; num7++)
			{
				float num10 = hitRay[num7].distance - hitRay[0].distance;
				if (num10 <= hitsBullet.distanceMin || hitsBullet.distanceMax < num10)
				{
					hitRay[num7] = default(RaycastHit2D);
				}
			}
			for (num7 = 0; num7 < hitRay.Length; num7++)
			{
				if (num2 >= randomNumHits)
				{
					break;
				}
				if (hitRay[num7].collider != null && hitRay[num7].collider.sharedMaterial != null && (objectHit == null || (objectHit != hitRay[num7].collider.gameObject && objectHit.transform.parent != hitRay[num7].collider.transform.parent && objectHit.transform.parent != hitRay[num7].collider.transform && objectHit.transform != hitRay[num7].collider.transform.parent)))
				{
					hits[num].id = i;
					hits[num].raycastHit = hitRay[num7];
					hits[num].collider = hitRay[num7].collider;
					hits[num].localPoint = hitRay[num7].collider.transform.InverseTransformPoint(hitRay[num7].point);
					hits[num].power = PhysicsMaterialMultiply.GetMultiplyPower(hitRay[num7].collider.sharedMaterial, damageMultiply, num2, 1f);
					hits[num].damage = damage * hits[num].power;
					hits[num].impulse = impulse * hits[num].power;
					hits[num].countHits = num2;
					hits[num].idShot = Mathf.FloorToInt(Time.time) + magazine * 1000;
					entity = hitRay[num7].collider.GetComponentInParent<Entity>();
					if (entity != null && (string.IsNullOrEmpty(tagOfEntity) || entity.CompareTag(tagOfEntity)))
					{
						hits[num].entity = entity;
						objectHit = hitRay[num7].collider.gameObject;
						num3 = hitRay[num7].distance;
						PlaySoundHit(hits[num]);
						entity.OnHitBullet(hits[num]);
					}
					onHitsBulletsSource(hits[num]);
					num++;
					num2++;
					if (PhysicsMaterialMultiply.StopBullet(hitRay[num7].collider.sharedMaterial, damageMultiply))
					{
						break;
					}
				}
			}
		}
		collidersOverlap = null;
		objectHit = null;
		entity = null;
		DeactivateTriggers();
		if (hits[0].weapon != null && hits[0].raycastHit.collider != null)
		{
			hits[0].raycastHit.collider.enabled = true;
		}
		if (magazineMax > 0)
		{
			magazine--;
		}
		if (delayBullet == 0f)
		{
			shot.Invoke();
		}
		triggerShotWeapon = automat;
	}

	public void StopHitRayBullet()
	{
		hitRay = new RaycastHit2D[0];
	}

	public void SetHitRayBullet(RaycastHit2D[] hits)
	{
		hitRay = hits;
	}

	public int Reload(int stock)
	{
		if (stock >= magazineMax)
		{
			magazine += magazineMax;
			stock -= magazineMax;
		}
		else
		{
			magazine += stock;
			stock = 0;
		}
		return stock;
	}

	public void AddBullets(int bullets)
	{
		bullets = Mathf.Min(bullets, magazineMax);
		magazine += bullets;
	}

	public RaycastHit2D CreateLineBullet(Vector2 rayPosition, float angle)
	{
		RaycastHit2D result = default(RaycastHit2D);
		rayDirection = new Vector2(Mathf.Cos((float)Math.PI / 180f * angle), Mathf.Sin((float)Math.PI / 180f * angle));
		if (string.IsNullOrEmpty(tagOfEntity))
		{
			return Physics2D.Raycast(rayPosition, rayDirection, distance, layerBodies.value);
		}
		hitRay = Physics2D.RaycastAll(rayPosition, rayDirection, distance, layerBodies.value);
		for (int i = 0; i < hitRay.Length; i++)
		{
			if (hitRay[i].collider.gameObject.CompareTag(tagOfEntity))
			{
				result = hitRay[i];
				break;
			}
		}
		return result;
	}

	public void PlaySoundHit(HitBullet hitRay)
	{
		if (timePlaySound == Mathf.Floor(Time.time * 1000f) || soundsHit.Length == 0 || !(audioSource != null))
		{
			return;
		}
		audioSource.Stop();
		timePlaySound = Mathf.Floor(Time.time * 1000f);
		if (soundsHit.Length == 1 && soundsHit[0].material.name == hitRay.raycastHit.collider.sharedMaterial.name)
		{
			audioSource.PlayOneShot(soundsHit[0].sound);
		}
		else
		{
			if (soundsHit.Length <= 1)
			{
				return;
			}
			int num = 0;
			while (true)
			{
				if (num < soundsHit.Length)
				{
					if (soundsHit[num].material.name == hitRay.raycastHit.collider.sharedMaterial.name)
					{
						break;
					}
					num++;
					continue;
				}
				return;
			}
			audioSource.PlayOneShot(soundsHit[num].sound);
		}
	}
}
