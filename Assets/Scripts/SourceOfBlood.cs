using System;
using UnityEngine;

public class SourceOfBlood : ComponentArea
{
	public static int current = 0;

	public static BloodBase[] listDrops = new BloodBase[0];

	public string nameSource;

	public BloodBase drops;

	public Vector2 localPosition;

	[Tooltip("Локальный угол полета капель крови")]
	[Range(0f, 360f)]
	public int angle;

	private float angleGlobal;

	[Tooltip("Направление крови из центра физического тела")]
	public bool fromCenter;

	[Tooltip("Существо, координаты которой будут использованы как координаты земли")]
	public Transform entity;

	[Tooltip("Отклонение от указаного направления")]
	[Range(0f, 180f)]
	public int deviationAngle = 10;

	private float randomAngle;

	public int elementsInSeconds = 10;

	public int totalElements = 15;

	public int totalMax;

	public float speedMin;

	public float speedMax = 0.1f;

	public bool spreadSpeed;

	[Tooltip("Масштаб частиц")]
	public float scale = 1f;

	[Tooltip("Пульсация. Время, в течении которого будут идти брызги, затем в течении этого же времени будет длиться приостановка")]
	public float timePulse;

	private float currentTimePulse;

	[HideInInspector]
	public bool pause = true;

	private BloodBase blood;

	private float timer;

	private float speed = 1f;

	private float stepSpeed;

	private Vector3 movePartBody = default(Vector2);

	private Rigidbody2D body;

	private Vector2 globalPosition = default(Vector2);

	public static BloodBase GetDropsList(BloodBase dropsSource)
	{
		listDrops = dropsSource.GetListObjects();
		if (listDrops.Length != 0)
		{
			current++;
			current = Mathf.FloorToInt(Mathf.Repeat(current, listDrops.Length));
			if (listDrops[current] == null)
			{
				listDrops[current] = UnityEngine.Object.Instantiate(dropsSource.gameObject).GetComponent<BloodBase>();
			}
			listDrops[current].gameObject.SetActive(value: true);
			listDrops[current].enabled = true;
			listDrops[current].OnGetDrops();
			return listDrops[current];
		}
		return null;
	}

	public override void OnDrawGizmosSelected()
	{
		base.OnDrawGizmosSelected();
		if (base.enabled)
		{
			globalPosition = base.gameObject.transform.TransformPoint(localPosition);
			Gizmos.color = new Color(1f, 0.05f, 0f);
			if (body == null)
			{
				body = base.gameObject.GetComponent<Rigidbody2D>();
			}
			if (body == null)
			{
				body = base.gameObject.GetComponentInParent<Rigidbody2D>();
			}
			if (fromCenter && body != null && !body.isKinematic)
			{
				float y = globalPosition.y;
				Vector2 worldCenterOfMass = body.worldCenterOfMass;
				float y2 = y - worldCenterOfMass.y;
				float x = globalPosition.x;
				Vector2 worldCenterOfMass2 = body.worldCenterOfMass;
				angleGlobal = Mathf.Atan2(y2, x - worldCenterOfMass2.x);
			}
			else
			{
				angleGlobal = (float)angle * ((float)Math.PI / 180f);
			}
			float num = angleGlobal;
			Vector3 eulerAngles = base.gameObject.transform.rotation.eulerAngles;
			angleGlobal = num + eulerAngles.z * ((float)Math.PI / 180f);
			Gizmos.DrawLine(globalPosition, new Vector2(globalPosition.x + Mathf.Cos(angleGlobal) * speedMax * 2f, globalPosition.y + Mathf.Sin(angleGlobal) * speedMax * 2f));
			angleGlobal -= (float)deviationAngle * ((float)Math.PI / 180f);
			Gizmos.DrawLine(globalPosition, new Vector2(globalPosition.x + Mathf.Cos(angleGlobal) * speedMax * 2f, globalPosition.y + Mathf.Sin(angleGlobal) * speedMax * 2f));
			angleGlobal += (float)(deviationAngle * 2) * ((float)Math.PI / 180f);
			Gizmos.DrawLine(globalPosition, new Vector2(globalPosition.x + Mathf.Cos(angleGlobal) * speedMax * 2f, globalPosition.y + Mathf.Sin(angleGlobal) * speedMax * 2f));
		}
	}

	public override bool ShowIconGizmos(AreaDamage area)
	{
		OnDrawGizmosSelected();
		return true;
	}

	public override void InitArea(AreaDamage area)
	{
		if (drops.GetListObjects().Length == 0)
		{
			currentTimePulse = 0f - timePulse;
			timer = -1f;
			pause = true;
			UnityEngine.Object.Destroy(this);
		}
		body = base.gameObject.GetComponent<Rigidbody2D>();
		fromCenter = (fromCenter && body != null && !body.isKinematic);
		currentTimePulse = timePulse;
	}

	public override void Activation(AreaDamage area)
	{
		Enable();
	}

	public void Enable()
	{
		movePartBody = base.transform.position;
		pause = false;
		totalElements = UnityEngine.Random.Range(totalElements, Mathf.Max(totalElements, totalMax));
		stepSpeed = (speedMax - speedMin) / (float)totalElements;
		stepSpeed = Mathf.Floor(stepSpeed * 1000f) / 1000f;
	}

	public void Enable(string name)
	{
		if (drops.GetListObjects().Length == 0)
		{
			return;
		}
		SourceOfBlood[] components = base.gameObject.GetComponents<SourceOfBlood>();
		for (int i = 0; i < components.Length; i++)
		{
			if (components[i].nameSource == name)
			{
				components[i].Enable();
			}
		}
		components = null;
	}

	public void Disable(string name)
	{
		if (drops.GetListObjects().Length == 0)
		{
			return;
		}
		SourceOfBlood[] components = base.gameObject.GetComponents<SourceOfBlood>();
		for (int i = 0; i < components.Length; i++)
		{
			if (components[i].nameSource == name)
			{
				components[i].pause = true;
			}
		}
		components = null;
	}

	private void FixedUpdate()
	{
		if (pause)
		{
			return;
		}
		if (currentTimePulse > 0f)
		{
			currentTimePulse -= Time.fixedDeltaTime;
			if (currentTimePulse <= 0f)
			{
				currentTimePulse = 0f - timePulse;
			}
		}
		else if (currentTimePulse < 0f)
		{
			currentTimePulse += Time.fixedDeltaTime;
			if (currentTimePulse >= 0f)
			{
				currentTimePulse = timePulse;
			}
		}
		if (currentTimePulse >= 0f)
		{
			timer += Time.fixedDeltaTime;
		}
		movePartBody = base.gameObject.transform.position - movePartBody;
		if (timer > 1f / (float)elementsInSeconds)
		{
			globalPosition = base.gameObject.transform.TransformPoint(localPosition);
			if (fromCenter)
			{
				float y = globalPosition.y;
				Vector2 worldCenterOfMass = body.worldCenterOfMass;
				float y2 = y - worldCenterOfMass.y;
				float x = globalPosition.x;
				Vector2 worldCenterOfMass2 = body.worldCenterOfMass;
				angle = Mathf.FloorToInt(57.29578f * Mathf.Atan2(y2, x - worldCenterOfMass2.x));
			}
			angleGlobal = angle;
			float num = angleGlobal;
			Vector3 eulerAngles = base.gameObject.transform.rotation.eulerAngles;
			angleGlobal = num + eulerAngles.z;
			if (Mathf.Abs(270f - angleGlobal) < 60f)
			{
				Vector3 position = entity.position;
				if (Mathf.Abs(position.y - globalPosition.y) < 1f)
				{
					totalElements = 0;
					goto IL_0334;
				}
			}
			randomAngle = UnityEngine.Random.Range(-deviationAngle, deviationAngle);
			speed = speedMin + UnityEngine.Random.value * (speedMax - speedMin);
			if (spreadSpeed)
			{
				speed = speedMin + (float)totalElements * stepSpeed;
			}
			blood = GetDropsList(drops);
			if (blood != null)
			{
				blood.gameObject.transform.position = globalPosition;
				blood.SetMove(speed, angleGlobal + randomAngle);
				BloodBase bloodBase = blood;
				Vector3 position2 = entity.position;
				bloodBase.SetFloor(position2.y);
				blood.SetSpeedBody(movePartBody);
				blood.SetScale(scale, scale);
			}
			timer = 0f;
			totalElements--;
			angleGlobal %= 360f;
		}
		goto IL_0334;
		IL_0334:
		movePartBody = base.gameObject.transform.position;
		if (totalElements <= 0)
		{
			UnityEngine.Object.Destroy(this);
		}
	}
}
