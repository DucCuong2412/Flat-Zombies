using System;
using UnityEngine;
using UnityEngine.Events;

public class WeaponPoint : MonoBehaviour
{
	[Serializable]
	public struct HitsBullet
	{
		[Tooltip("Кол-во попаданий для одной пули")]
		public int nums;
	}

	public delegate void MethodHitsBullets(HitBullet hit);

	[HideInInspector]
	public float recoil;

	[Tooltip("Скорость увеличения отдачи")]
	[Range(0f, 10f)]
	public float recoilSpeed;

	[Tooltip("Максимум сила отдачи")]
	[Range(0f, 10f)]
	public float recoilMax;

	[Tooltip("Скорость уменьшения отдачи")]
	[Range(0f, 1f)]
	public float recoilDecrease;

	[Tooltip("Отклонение при стрельбе")]
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

	[HideInInspector]
	public int magazine;

	[Tooltip("Задержка вылета пули после нажатия на курок, в скндх.")]
	public float delayBullet;

	[Space(10f)]
	[Tooltip("Кол-в во пуль при одном выстреле")]
	public int bullets = 1;

	[Tooltip("Максмлн отклонение пули в метрах, для создания разброса")]
	[Range(0f, 180f)]
	public float angleScatter;

	public float radiusScatterMin;

	public float radiusScatter;

	public bool evenlySpread;

	[Tooltip("Наносимый урон от всех пуль (будет распределен между всеми пулями)")]
	public float damage = 1f;

	[Tooltip("Импульс (будет распределен между всеми пулями)")]
	public float impulse;

	[Tooltip("Параметры попадания для одной пули")]
	public HitsBullet hitsBullet = default(HitsBullet);

	[Tooltip("Снижение силы после каждого пересечения тела")]
	[Range(0f, 1f)]
	public float powerReduction = 1f;

	[Space(10f)]
	[Tooltip("Тег игровых объектов существ, с которыми искать попадание пуль (необязательно)")]
	public string tagOfEntity;

	[Tooltip("Слои с физ.телами для столкновения луча RaycastHit2D")]
	public LayerMask layerBodies;

	[Space(5f)]
	[Tooltip("Список функций, вызываемых при выстреле")]
	public UnityEvent shot = new UnityEvent();

	private float lag;

	[SerializeField]
	private float currentDelayBullet;

	private float pause;

	private bool triggerShotWeapon = true;

	private Collider2D[] hitColliders;

	public AudioClip audioShot;

	public AudioSource audioSource;

	[HideInInspector]
	public bool reload;

	[HideInInspector]
	public Vector3 startLocalPosition;

	private Vector2 globalPosition;

	private float angleRad;

	private RaycastHit2D hitLaser;

	private Vector3 localPoint;

	private HitBullet[] hits;

	private GameObject objectHit;

	private Vector2 positionBullet = Vector2.zero;

	private Vector2 slideBullet;

	private Entity entity;

	private void OnDrawGizmos()
	{
		if (hits != null)
		{
			Gizmos.color = new Color(1f, 0.4f, 0f);
			for (int i = 0; i < hits.Length; i++)
			{
				Gizmos.DrawWireSphere(hits[i].raycastHit.point, 0.2f);
			}
		}
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = new Color(1f, 0.2f, 0f);
		if (angleScatter == 0f || !base.enabled)
		{
			return;
		}
		Vector3 eulerAngles = base.gameObject.transform.rotation.eulerAngles;
		angleRad = eulerAngles.z * ((float)Math.PI / 180f) + (float)Math.PI / 2f;
		globalPosition = base.gameObject.transform.position;
		angleRad -= angleScatter * ((float)Math.PI / 180f);
		Gizmos.DrawLine(globalPosition, new Vector2(globalPosition.x + Mathf.Cos(angleRad) * 2f, globalPosition.y + Mathf.Sin(angleRad) * 2f));
		angleRad += angleScatter * ((float)Math.PI / 180f) * 2f;
		Gizmos.DrawLine(globalPosition, new Vector2(globalPosition.x + Mathf.Cos(angleRad) * 2f, globalPosition.y + Mathf.Sin(angleRad) * 2f));
		Gizmos.color = new Color(1f, 0.5f, 0f);
		float num = 0f;
		float num2 = 0f;
		while (bullets > 1 && num2 < (float)bullets && num2 < 90f)
		{
			Vector3 eulerAngles2 = base.transform.rotation.eulerAngles;
			num = eulerAngles2.z - angleScatter + angleScatter * 2f * (num2 / (float)bullets);
			if (evenlySpread)
			{
				num += angleScatter / (float)bullets;
			}
			else
			{
				Vector3 eulerAngles3 = base.gameObject.transform.rotation.eulerAngles;
				num = eulerAngles3.z - angleScatter + UnityEngine.Random.value * angleScatter * 2f;
			}
			num = num * ((float)Math.PI / 180f) + (float)Math.PI / 2f;
			float num3 = radiusScatterMin + radiusScatter * UnityEngine.Random.value;
			Gizmos.DrawLine(globalPosition, new Vector2(globalPosition.x + num3 * Mathf.Cos(num), globalPosition.y + num3 * Mathf.Sin(num)));
			num2 += 1f;
		}
	}

	private void Awake()
	{
		startLocalPosition = base.transform.localPosition;
		lag = 1f / ((float)shotsInMinute / 60f);
		lag = Mathf.Round(lag * 100000f) / 100000f;
		if (bullets < 1)
		{
			bullets = 1;
		}
		damage /= bullets;
		impulse /= bullets;
		magazine = Mathf.Max(1, magazineMax);
		hitsBullet.nums = Mathf.Max(1, hitsBullet.nums);
	}

	private void Start()
	{
	}

	private void OnEnable()
	{
	}

	private void OnDisable()
	{
	}

	private void FixedUpdate()
	{
		if (recoil <= 0f)
		{
			recoil = 0f;
		}
		else
		{
			recoil -= recoilDecrease;
		}
	}

	private void Update()
	{
		if (pause < 0f)
		{
			pause += Time.deltaTime;
		}
	}

	public bool IsReady()
	{
		return base.enabled && pause >= 0f && magazine > 0;
	}

	public bool IsReady(bool trigger)
	{
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

	public void Shot(MethodHitsBullets onHitsBulletsSource)
	{
		WeaponLine.ActivateTriggers();
		hits = new HitBullet[bullets * hitsBullet.nums];
		int num = 0;
		int num2 = 0;
		currentDirection = 0f - direction + Mathf.Ceil(UnityEngine.Random.value * direction * 2f);
		pause = 0f - lag;
		recoil += recoilSpeed;
		if (recoil > recoilMax + currentDirection)
		{
			recoil = recoilMax + currentDirection;
		}
		float num3 = 0f;
		for (float num4 = 0f; num4 < (float)bullets; num4 += 1f)
		{
			objectHit = null;
			num2 = 0;
			if (evenlySpread)
			{
				Vector3 eulerAngles = base.gameObject.transform.rotation.eulerAngles;
				num3 = eulerAngles.z - angleScatter + angleScatter * 2f * (num4 / (float)bullets);
				num3 += angleScatter / (float)bullets;
			}
			else
			{
				Vector3 eulerAngles2 = base.gameObject.transform.rotation.eulerAngles;
				num3 = eulerAngles2.z - angleScatter + UnityEngine.Random.value * angleScatter * 2f;
			}
			num3 += (float)Math.PI / 2f;
			float num5 = radiusScatterMin + radiusScatter * UnityEngine.Random.value;
			positionBullet = base.gameObject.transform.position;
			slideBullet.x = Mathf.Cos((float)Math.PI / 180f * num3) * num5;
			slideBullet.y = Mathf.Sin((float)Math.PI / 180f * num3) * num5;
			positionBullet += slideBullet;
			slideBullet.x = 0f - slideBullet.x;
			slideBullet.y = 0f - slideBullet.y;
			hitColliders = Physics2D.OverlapPointAll(positionBullet, layerBodies.value);
			for (int i = 0; i < hitColliders.Length; i++)
			{
				if (num2 >= hitsBullet.nums)
				{
					break;
				}
				if (i == 0 || objectHit != hitColliders[i].gameObject)
				{
					hits[num].id = (int)num4;
					hits[num].localPoint = hitColliders[i].transform.InverseTransformPoint(positionBullet);
					hits[num].collider = hitColliders[i];
					hits[num].power = Mathf.Pow(0.5f, num2);
					hits[num].damage = damage * hits[num].power;
					hits[num].impulse = impulse * hits[num].power;
					hits[num].countHits = num2;
					entity = hitColliders[i].GetComponentInParent<Entity>();
					hits[num].entity = entity;
					objectHit = hitColliders[i].gameObject;
					entity.OnHitBullet(hits[num]);
					UnityEngine.Debug.Log("entity:" + entity.name);
					onHitsBulletsSource(hits[num]);
					num++;
					num2++;
				}
			}
		}
		WeaponLine.DeactivateTriggers();
		if (audioSource != null)
		{
			audioSource.clip = audioShot;
			audioSource.Play();
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
		hitColliders = new Collider2D[0];
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
		magazine += Mathf.Min(magazineMax, bullets);
		if (magazine > magazineMax && magazineMax > 0)
		{
			magazine = magazineMax;
		}
	}
}
