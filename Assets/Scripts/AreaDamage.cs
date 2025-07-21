using System;
using UnityEngine;

[AddComponentMenu("Scripts/Area Damage/AreaDamage")]
public class AreaDamage : MonoBehaviour
{
	public delegate void Method(AreaDamage area);

	[Tooltip("Имя/ID, для получения доступа к конкретной области")]
	public string areaName;

	[Tooltip("Использовать несколько раз")]
	public bool reuse;

	[Tooltip("Указать в position перед активацией координаты попадания, передаваемые в HitTestPoint() при проверке попадания")]
	public bool usePointHit;

	[Tooltip("Локальные координаты центра области в метрах")]
	public Vector2 position = default(Vector2);

	[HideInInspector]
	public Vector2 positionHit = default(Vector2);

	[HideInInspector]
	public Vector2 positionHitLocal = default(Vector2);

	public float radius = 0.5f;

	[Range(0f, 360f)]
	public float angle;

	[Range(0f, 360f)]
	public float limitAngle;

	[Tooltip("здоровье для активации и повторного использования\nПри health = 0 - активировать область при запуске")]
	public float health = 1f;

	[HideInInspector]
	public float startHealth;

	[Tooltip("Необходимый наносимый урон для активации")]
	public float minDamage;

	[HideInInspector]
	public bool updateListOnStart;

	[HideInInspector]
	public ComponentArea[] listComps = new ComponentArea[0];

	[HideInInspector]
	public ComponentAreaScriptable[] listScriptable = new ComponentAreaScriptable[0];

	[Tooltip("События-идентификаторы, по которым можно найти области, выполняющие определенную функцию")]
	public string[] events = new string[0];

	[Space(5f)]
	[Tooltip("Список функций, вызываемых при активации перед компонентами")]
	public AreaDamageEvent activation = new AreaDamageEvent();

	private Vector2 startPosition = default(Vector2);

	private bool startMade;

	private Vector3[] points = new Vector3[0];

	private Method[] listMethods = new Method[0];

	private bool hit;

	private float angleHit;

	private AreaDamage[] listAreas = new AreaDamage[0];

	public string ToString()
	{
		return "AreaDamage(" + position.x + ";" + position.y + " [" + radius + "])";
	}

	private void OnDrawGizmosSelected()
	{
		if (!base.enabled)
		{
			return;
		}
		Transform transform = base.gameObject.transform;
		float x = position.x;
		float y = position.y;
		Vector3 vector = base.gameObject.transform.position;
		Vector3 vector2 = transform.TransformPoint(new Vector3(x, y, vector.z));
		float num = radius;
		Vector3 localScale = base.gameObject.transform.localScale;
		float num2 = num * localScale.x;
		float num3 = angle;
		Vector3 eulerAngles = base.gameObject.transform.rotation.eulerAngles;
		float num4 = (num3 + eulerAngles.z) * ((float)Math.PI / 180f);
		float num5 = (limitAngle != 0f || angle != 0f) ? limitAngle : 360f;
		Gizmos.color = new Color(0f, 0.5f, 1f);
		points = new Vector3[41];
		float num6 = num5 / (float)(points.Length - 1) * ((float)Math.PI / 180f);
		for (int i = 0; i < points.Length; i++)
		{
			points[i].x = num2 * Mathf.Cos(num6 * (float)i + num4);
			points[i].y = num2 * Mathf.Sin(num6 * (float)i + num4);
			points[i] += vector2;
		}
		for (int j = 1; j < points.Length; j++)
		{
			Gizmos.DrawLine(points[j - 1], points[j]);
		}
		Gizmos.DrawLine(vector2, points[points.Length - 1]);
		Gizmos.color = new Color(0f, 0.05f, 1f);
		Gizmos.DrawLine(vector2, points[0]);
		Vector3 center = vector2;
		center.y += 0.05f * Mathf.Sin(angle * ((float)Math.PI / 180f));
		for (int k = 0; k < listComps.Length; k++)
		{
			if (listComps[k] != null && listComps[k].enabled && listComps[k].ShowIconGizmos(this))
			{
				center.x += 0.05f * Mathf.Cos(angle * ((float)Math.PI / 180f));
				Gizmos.DrawIcon(center, listComps[k].GetType().ToString() + " Icon.png", allowScaling: false);
				listComps[k].GizmosArea(this);
			}
		}
		for (int l = 0; l < listScriptable.Length; l++)
		{
			listScriptable[l].OnGizmosArea(this);
		}
		if (positionHitLocal.magnitude > 0f)
		{
			Gizmos.color = new Color(0.1f, 0.25f, 1f);
			Gizmos.DrawLine(vector2, base.transform.TransformPoint(positionHitLocal));
		}
	}

	public Vector2 GetWorldPosition()
	{
		return base.gameObject.transform.TransformPoint(position);
	}

	public void AddListener(Method method)
	{
		if (listMethods.Length == 0)
		{
			listMethods = new Method[10];
		}
		int num = 0;
		while (true)
		{
			if (num < listMethods.Length)
			{
				if (listMethods[num] == null)
				{
					break;
				}
				num++;
				continue;
			}
			return;
		}
		listMethods[num] = method;
	}

	private void Awake()
	{
	}

	private void Start()
	{
		startPosition = position;
		if (!startMade)
		{
			position.x = Mathf.Ceil(position.x * 1000f) / 1000f;
			position.y = Mathf.Ceil(position.y * 1000f) / 1000f;
			startHealth = health;
			int num = 0;
			for (num = 0; num < listComps.Length; num++)
			{
				if (listComps[num] != null)
				{
					listComps[num].InitArea(this);
				}
			}
			for (num = 0; num < listScriptable.Length; num++)
			{
				listScriptable[num].OnStartArea(this);
			}
		}
		startMade = true;
		for (int i = 0; i < activation.GetPersistentEventCount(); i++)
		{
			if (activation.GetPersistentMethodName(i) == string.Empty || activation.GetPersistentTarget(i) == null)
			{
				UnityEngine.Debug.LogError("Не указан метод для Activaton()\n" + ToString() + ":" + base.transform.parent.gameObject.name + "." + base.gameObject.name);
			}
		}
		if (health == 0f)
		{
			Activation(0f);
		}
	}

	public void Activation(float damage)
	{
		if (!startMade)
		{
			Start();
		}
		health -= damage;
		if (!startMade || !(health <= 0f) || !base.enabled)
		{
			return;
		}
		health = 0f;
		activation.Invoke(this);
		for (int i = 0; i < listComps.Length; i++)
		{
			if (listComps[i] != null && listComps[i].enabled)
			{
				listComps[i].Activation(this);
			}
		}
		for (int j = 0; j < listScriptable.Length; j++)
		{
			listScriptable[j].OnActivation(this);
		}
		for (int k = 0; k < listMethods.Length; k++)
		{
			if (listMethods[k] != null)
			{
				listMethods[k](this);
			}
		}
		base.enabled = reuse;
		if (reuse)
		{
			health = startHealth;
		}
	}

	public void Activation(string name)
	{
		AreaDamage[] components = base.gameObject.GetComponents<AreaDamage>();
		bool flag = false;
		for (int i = 0; i < components.Length; i++)
		{
			if (components[i].areaName == name)
			{
				components[i].Activation(components[i].health);
				components[i].health = 0f;
				flag = true;
			}
		}
		if (!flag)
		{
			UnityEngine.Debug.LogWarning("AreaDamage.Activation: область " + name + " не найдена в " + base.gameObject.name);
		}
		components = null;
	}

	public void Activation(GameObject gameObject)
	{
		AreaDamage[] components = gameObject.GetComponents<AreaDamage>();
		for (int i = 0; i < components.Length; i++)
		{
			components[i].Activation(components[i].health);
			components[i].health = 0f;
		}
		components = null;
	}

	public void Activation()
	{
		Activation(health);
	}

	public bool HitTestPoint(Vector2 point, float damage, float radius)
	{
		if (!base.enabled || damage < minDamage)
		{
			return false;
		}
		positionHit = point;
		point = base.gameObject.transform.InverseTransformPoint(point);
		positionHitLocal = point;
		hit = (radius >= Vector2.Distance(position, point));
		if (hit && (angle != 0f || limitAngle != 0f))
		{
			angleHit = Mathf.Atan2(point.y - position.y, point.x - position.x) * 57.29578f;
			angleHit = Mathf.Repeat(angleHit + 360f, 360f);
			hit = (angle <= angleHit && angleHit <= angle + limitAngle);
			angleHit += 360f;
			hit = (hit || (angle <= angleHit && angleHit <= angle + limitAngle));
		}
		if (hit)
		{
			if (usePointHit)
			{
				position = point;
			}
			Activation(damage);
			position = startPosition;
		}
		return hit;
	}

	public bool HitTestPoint(Vector2 globalPoint, float damage)
	{
		return HitTestPoint(globalPoint, damage, radius);
	}

	public bool HitTestPoint(Vector2 globalPoint, float damage, float radius, string idEvent)
	{
		if (EventExists(idEvent))
		{
			return HitTestPoint(globalPoint, damage, radius);
		}
		return false;
	}

	public bool EventExists(string name)
	{
		if (events.Length == 0 && string.IsNullOrEmpty(name))
		{
			return true;
		}
		if (areaName == name)
		{
			return true;
		}
		for (int i = 0; i < events.Length; i++)
		{
			if (name == events[i])
			{
				return true;
			}
		}
		return false;
	}

	public bool IsConnected(ComponentArea component)
	{
		for (int i = 0; i < listComps.Length; i++)
		{
			if (listComps[i] == component)
			{
				return true;
			}
		}
		return false;
	}

	public bool IsConnected(ComponentAreaScriptable component)
	{
		for (int i = 0; i < listScriptable.Length; i++)
		{
			if (listScriptable[i] == component)
			{
				return true;
			}
		}
		return false;
	}

	public bool ComponentInList<Type>() where Type : class
	{
		int num = 0;
		for (num = 0; num < listComps.Length; num++)
		{
			if (listComps[num] != null && listComps[num] is Type)
			{
				return true;
			}
		}
		for (num = 0; num < listScriptable.Length; num++)
		{
			if (listScriptable[num] != null && listScriptable[num] is Type)
			{
				return true;
			}
		}
		return false;
	}

	public Type GetComponentArea<Type>() where Type : class
	{
		for (int i = 0; i < listComps.Length; i++)
		{
			if (listComps[i] != null && listComps[i] is Type)
			{
				return listComps[i] as Type;
			}
		}
		return (Type)null;
	}

	public void Disable(string name)
	{
		listAreas = base.gameObject.GetComponents<AreaDamage>();
		for (int i = 0; i < listAreas.Length; i++)
		{
			if (listAreas[i].areaName == name)
			{
				listAreas[i].enabled = false;
			}
		}
		listAreas = new AreaDamage[0];
	}

	public void Enable(string name)
	{
		listAreas = base.gameObject.GetComponents<AreaDamage>();
		for (int i = 0; i < listAreas.Length; i++)
		{
			if (listAreas[i].areaName == name)
			{
				listAreas[i].enabled = true;
			}
		}
		listAreas = new AreaDamage[0];
	}

	public void Destroy(string name)
	{
		listAreas = base.gameObject.GetComponents<AreaDamage>();
		for (int i = 0; i < listAreas.Length; i++)
		{
			if (listAreas[i].areaName == name)
			{
				UnityEngine.Object.Destroy(listAreas[i]);
			}
		}
		listAreas = new AreaDamage[0];
	}
}
