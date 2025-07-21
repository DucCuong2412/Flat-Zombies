using UnityEngine;

public class Entity : MonoBehaviour
{
	public static float distTimePriority = 2f;

	private AreaDamage[] areasCache = new AreaDamage[0];

	public float health;

	public Collider2D[] collidersBody = new Collider2D[0];

	[HideInInspector]
	public bool isDead;

	[HideInInspector]
	public float timeDeath;

	public IEntityDeath listenerDeath;

	private IEntityDeath objectEventDeath;

	public AreaDamage[] areas
	{
		get
		{
			if (areasCache.Length == 0 && base.gameObject != null)
			{
				areasCache = base.gameObject.GetComponentsInChildren<AreaDamage>(includeInactive: true);
			}
			return areasCache;
		}
	}

	public void SetActiveTriggers(bool value)
	{
		if (collidersBody.Length == 0)
		{
			collidersBody = GetComponentsInChildren<Collider2D>(includeInactive: true);
		}
		for (int i = 0; i < collidersBody.Length; i++)
		{
			if (collidersBody[i] != null && collidersBody[i].isTrigger && collidersBody[i].sharedMaterial != null)
			{
				collidersBody[i].enabled = value;
			}
		}
		OnActiveTriggers(value);
	}

	public virtual void OnActiveTriggers(bool value)
	{
	}

	public virtual bool IsLowPriorityBullet(RaycastHit2D[] raycastHit, bool hitFront)
	{
		return hitFront && isDead && timeDeath != Time.time;
	}

	public virtual void OnHitBullet(HitBullet hitBullet)
	{
	}

	public virtual void HitExplosion(GameObject gameObject, float damage, Vector2 point, float impulse)
	{
	}

	public void HitTestArea(Vector2 globalPoint, float damage, float addRadius, float scaleRadius)
	{
		for (int i = 0; i < areas.Length; i++)
		{
			if (areas[i] != null)
			{
				areas[i].HitTestPoint(globalPoint, damage, (areas[i].radius + addRadius) * scaleRadius);
			}
		}
	}

	public void HitTestArea(Vector2 globalPoint, float damage)
	{
		for (int i = 0; i < areas.Length; i++)
		{
			if (areas[i] != null)
			{
				areas[i].HitTestPoint(globalPoint, damage);
			}
		}
	}

	public void HitTestArea(Vector2 globalPoint, float damage, string idEvent)
	{
		for (int i = 0; i < areas.Length; i++)
		{
			if (areas[i] != null)
			{
				areas[i].HitTestPoint(globalPoint, damage, areas[i].radius, idEvent);
			}
		}
	}

	public float HealthDamage(float damage, float health)
	{
		health = Mathf.Max(0f, health - damage);
		if (!isDead && health == 0f)
		{
			Death();
		}
		return health;
	}

	public float HealthDamage(float damage)
	{
		health = HealthDamage(damage, health);
		return health;
	}

	public virtual void Death()
	{
		isDead = true;
		timeDeath = Time.time;
		if (listenerDeath != null)
		{
			listenerDeath.OnDeathEntity(this);
		}
	}

	public bool IsDeadNow()
	{
		return isDead && timeDeath != Time.time;
	}
}
