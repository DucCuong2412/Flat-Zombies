using System;
using UnityEngine;

public class WeaponCartridge : ScriptableObject
{
	public static bool effectsHitIsEnabled = true;

	private static int idTriggers = 0;

	public static Entity[] entities = new Entity[0];

	private static Entity[] tempList = new Entity[0];

	public WeaponSourceBullets sourceBullets = new WeaponSourceBullets(1, 2f, 120f);

	public SoundHitBullet[] soundsHit;

	public WeaponEffectsHits effectsHits;

	private float angleRad;

	private Ray rayGizmos = default(Ray);

	private float distLastAim;

	private HitBullet[] hits = new HitBullet[0];

	private HitBullet hitBullet = default(HitBullet);

	private RaycastHit2D[] hitRay = new RaycastHit2D[0];

	private Collider2D[] collidersOverlap = new Collider2D[0];

	private RaycastHit2D raycastHit;

	private Entity entity;

	private float timePlaySound;

	public static void ActivateTriggers(int idShot)
	{
		if (idTriggers == 0)
		{
			idTriggers = idShot;
			for (int i = 0; i < entities.Length && entities[i] != null; i++)
			{
				entities[i].SetActiveTriggers(value: true);
			}
		}
	}

	public static void DeactivateTriggers(int idShot)
	{
		if (idTriggers == idShot)
		{
			idTriggers = 0;
			for (int i = 0; i < entities.Length && entities[i] != null; i++)
			{
				entities[i].SetActiveTriggers(value: false);
			}
		}
	}

	public static void OnStartScene()
	{
		idTriggers = 0;
	}

	public static void AddEntityTriggers(Entity entity)
	{
		if (entities.Length == 0 || entities[0] == null)
		{
			OnStartScene();
		}
		for (int i = 0; i < entities.Length; i++)
		{
			if (entities[i] == null)
			{
				entities[i] = entity;
				entity.SetActiveTriggers(idTriggers != 0);
				return;
			}
		}
		tempList = entities;
		entities = new Entity[tempList.Length + 1];
		for (int j = 0; j < tempList.Length; j++)
		{
			entities[j] = tempList[j];
		}
		entities[tempList.Length] = entity;
		tempList = new Entity[0];
		entity.SetActiveTriggers(idTriggers != 0);
	}

	public static void RemoveEntityTriggers(Entity entity)
	{
		int num = 0;
		while (true)
		{
			if (num < entities.Length && entities[num] != null)
			{
				if (entities[num] == entity)
				{
					break;
				}
				num++;
				continue;
			}
			return;
		}
		entities[num] = null;
		entity = null;
		for (int i = num; i < entities.Length - 1; i++)
		{
			entities[i] = entities[i + 1];
			entities[i + 1] = null;
		}
	}

	public virtual void AwakeWeapon(WeaponHands weapon)
	{
		if (effectsHits != null)
		{
			effectsHits.AwakeWeapon();
		}
	}

	public virtual void OnDrawGizmosWeapon(WeaponHands weapon)
	{
	}

	public virtual void OnDrawGizmosSelectedWeapon(WeaponHands weapon)
	{
		Vector2 vector = default(Vector2);
		float num = 0f;
		Vector3 direction = rayGizmos.direction;
		if (direction.x != 0f)
		{
			Gizmos.color = new Color(1f, 0.4f, 0f);
			Gizmos.DrawLine(rayGizmos.origin, rayGizmos.origin + rayGizmos.direction * 20f);
			Gizmos.color = new Color(1f, 0.9f, 0f);
			Gizmos.DrawWireSphere(rayGizmos.origin + rayGizmos.direction * distLastAim, 0.16f);
		}
		if (hits != null && hits.Length != 0)
		{
			Gizmos.color = new Color(1f, 0.4f, 0f);
			for (int i = 0; i < hits.Length; i++)
			{
				Gizmos.DrawWireSphere(hits[i].raycastHit.point, 0.2f);
				Gizmos.DrawLine(hits[i].raycastHit.point, hits[i].raycastHit.point + hits[i].raycastHit.normal);
			}
		}
		Gizmos.color = new Color(1f, 0.2f, 0f);
		vector = weapon.transform.TransformPoint(weapon.boltX, weapon.boltY, 0f);
		float boltAngle = weapon.boltAngle;
		Vector3 eulerAngles = weapon.transform.rotation.eulerAngles;
		float num2 = boltAngle + eulerAngles.z;
		angleRad = num2 * ((float)Math.PI / 180f);
		Vector2 b = new Vector2(Mathf.Cos(angleRad) * sourceBullets.distance, Mathf.Sin(angleRad) * sourceBullets.distance);
		if (sourceBullets.distance <= 0f)
		{
			Gizmos.DrawLine(vector, weapon.transform.TransformPoint(weapon.boltX + 3f, weapon.boltY, 0f));
		}
		else
		{
			Gizmos.DrawLine(vector, vector + b);
			Gizmos.DrawWireSphere(vector + b, 0.2f);
			Gizmos.color = new Color(1f, 0.5f, 0f);
		}
		num = sourceBullets.angleScatter * weapon.scaleAngleScatter;
		if (num != 0f)
		{
			Gizmos.color = new Color(1f, 0.5f, 0f);
			angleRad -= num * ((float)Math.PI / 180f);
			float num3 = 30f;
			if (sourceBullets.distance != 0f)
			{
				num3 = sourceBullets.distance;
			}
			Gizmos.DrawLine(vector, new Vector2(vector.x + Mathf.Cos(angleRad) * num3, vector.y + Mathf.Sin(angleRad) * num3));
			angleRad += num * ((float)Math.PI / 180f) * 2f;
			Gizmos.DrawLine(vector, new Vector2(vector.x + Mathf.Cos(angleRad) * num3, vector.y + Mathf.Sin(angleRad) * num3));
			float num4 = 0f;
			float num5 = 0f;
			while (sourceBullets.bullets > 1 && num5 < (float)sourceBullets.bullets)
			{
				num4 = GetAngle(num2, num, sourceBullets.evenlySpread, num5, sourceBullets.bullets);
				Gizmos.DrawLine(vector, new Vector2(vector.x + Mathf.Cos(num4) * num3, vector.y + Mathf.Sin(num4) * num3));
				num5 += 1f;
			}
		}
	}

	public float GetAngle(float globalAngleWeapon, float angleScatter, float evenlySpread, float currentArrow, float totalArrows)
	{
		float a = angleScatter - angleScatter * 2f * UnityEngine.Random.value;
		float num = angleScatter - angleScatter * 2f * (currentArrow / totalArrows);
		num -= angleScatter / totalArrows;
		float num2 = Mathf.LerpAngle(a, num, evenlySpread);
		num2 = globalAngleWeapon + num2;
		return num2 * ((float)Math.PI / 180f);
	}

	public virtual void StartWeapon(WeaponHands weapon)
	{
	}

	public virtual void Shot(WeaponHands weapon, float distToAim, WeaponHands.MethodHitsBullets onHitBullet)
	{
	}

	public virtual void OnHitBullet(HitBullet hit)
	{
	}

	public virtual float GetScaleScoresBullet()
	{
		return 1f / (float)sourceBullets.bullets;
	}

	public void ShotBullet(WeaponSourceBullets bullet, WeaponHands weapon, Vector2 rayPosition, float globalAngle, float distToAim, WeaponHands.MethodHitsBullets onHitBullet)
	{
		ActivateTriggers(weapon.GetInstanceID());
		bullet.angleScatter *= weapon.scaleAngleScatter;
		distLastAim = distToAim;
		float num = 0f;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		int num5 = 0;
		int num6 = 0;
		int num7 = 0;
		int num8 = 1;
		bullet.hitsBullet.numsMax = Mathf.Max(1, bullet.hitsBullet.numsMax);
		hits = new HitBullet[bullet.bullets * bullet.hitsBullet.numsMax];
		Vector2 vector = default(Vector2);
		collidersOverlap = Physics2D.OverlapPointAll(rayPosition, bullet.layerBodies.value);
		for (float num9 = 0f; num9 < (float)bullet.bullets; num9 += 1f)
		{
			num7 = 0;
			num8 = bullet.hitsBullet.GetRandomNumHits();
			num = GetAngle(globalAngle, bullet.angleScatter, bullet.evenlySpread, num9, bullet.bullets);
			vector.x = Mathf.Cos(num);
			vector.y = Mathf.Sin(num);
			hitRay = Physics2D.RaycastAll(rayPosition, vector, bullet.distance, bullet.layerBodies.value);
			for (int i = 0; i < collidersOverlap.Length; i++)
			{
				for (num4 = 0; num4 < hitRay.Length - 1; num4++)
				{
					if (hitRay[num4].collider == collidersOverlap[i])
					{
						hitRay[num4] = hitRay[num4 + 1];
					}
				}
			}
			for (num4 = 0; num4 < hitRay.Length - num2; num4++)
			{
				if (!(hitRay[num4].collider.sharedMaterial != null))
				{
					continue;
				}
				entity = hitRay[num4].collider.GetComponentInParent<Entity>();
				if (entity != null && Mathf.Floor(entity.timeDeath * 100f) != Mathf.Floor(Time.time * 100f) && entity.IsLowPriorityBullet(hitRay, hitRay[num4].distance < distToAim))
				{
					raycastHit = hitRay[num4];
					for (num3 = num4; num3 < hitRay.Length - 1; num3++)
					{
						hitRay[num3] = hitRay[num3 + 1];
					}
					hitRay[hitRay.Length - 1] = raycastHit;
					num4--;
					num2++;
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
			for (num4 = num5 + 1; num4 < hitRay.Length; num4++)
			{
				if (!bullet.hitsBullet.IsHitRangeDist(hitRay[num5].distance, hitRay[num4].distance))
				{
					hitRay[num4] = default(RaycastHit2D);
				}
				else if (hitRay[num5].collider.gameObject == hitRay[num4].collider.gameObject || hitRay[num5].collider.transform == hitRay[num4].collider.transform.parent || hitRay[num5].collider.transform.parent == hitRay[num4].collider.transform)
				{
					hitRay[num4] = default(RaycastHit2D);
				}
				else
				{
					num5 = num4;
				}
			}
			num5 = 0;
			if (hitRay.Length != 0 && (bullet.hitsBullet.modeHit == HitsBullet.ModeHit.RANDOM_ONCE || bullet.hitsBullet.modeHit == HitsBullet.ModeHit.RANDOM_FIRST))
			{
				num5 = UnityEngine.Random.Range(0, hitRay.Length);
				if (bullet.hitsBullet.modeHit == HitsBullet.ModeHit.RANDOM_FIRST && hitRay[num5].collider == null)
				{
					num5 = 0;
				}
			}
			for (num4 = num5; num4 < hitRay.Length; num4++)
			{
				if (num7 >= num8)
				{
					break;
				}
				if (hitRay[num4].collider != null && hitRay[num4].collider.sharedMaterial != null)
				{
					WeaponHands.numCreatedBullets++;
					hitBullet.id = WeaponHands.numCreatedBullets;
					hitBullet.raycastHit = hitRay[num4];
					hitBullet.collider = hitRay[num4].collider;
					hitBullet.localPoint = hitRay[num4].collider.transform.InverseTransformPoint(hitRay[num4].point);
					hitBullet.power = PhysicsMaterialMultiply.GetMultiplyPower(hitRay[num4].collider.sharedMaterial, bullet.damageMultiply, num7, 1f);
					hitBullet.damage = bullet.damage / (float)bullet.bullets * hitBullet.power;
					hitBullet.impulse = bullet.impulse / (float)bullet.bullets * hitBullet.power;
					hitBullet.rayPosition = rayPosition;
					hitBullet.rayDirection = vector;
					hitBullet.countHits = num7;
					hitBullet.weapon = weapon;
					hitBullet.cartridge = this;
					hitBullet.cartridgeRay = bullet;
					hitBullet.idShot = Mathf.FloorToInt(Time.time) + weapon.magazine * 1000;
					hitBullet.entity = hitRay[num4].collider.GetComponentInParent<Entity>();
					hits[num6] = hitBullet;
					num6++;
					num7++;
					if (hitBullet.entity != null && (string.IsNullOrEmpty(bullet.tagEntity) || hitBullet.entity.CompareTag(bullet.tagEntity)))
					{
						hitBullet.entity.OnHitBullet(hitBullet);
					}
					onHitBullet(hitBullet);
					OnHitBullet(hitBullet);
					PlaySoundHit(weapon, hitBullet.raycastHit.collider.sharedMaterial.name);
					if (effectsHits != null)
					{
						effectsHits.OnHitBullet(hitBullet);
					}
					if (bullet.hitsBullet.modeHit == HitsBullet.ModeHit.RANDOM_ONCE || bullet.hitsBullet.modeHit == HitsBullet.ModeHit.RANDOM_FIRST || PhysicsMaterialMultiply.StopBullet(hitRay[num4].collider.sharedMaterial, bullet.damageMultiply))
					{
						break;
					}
				}
			}
		}
		DeactivateTriggers(weapon.GetInstanceID());
		if (hits[0].raycastHit.collider != null)
		{
			hits[0].raycastHit.collider.enabled = true;
		}
		rayGizmos = new Ray(rayPosition, vector);
		collidersOverlap = null;
		entity = null;
	}

	public void ShotBullet(WeaponSourceBullets bullet, WeaponHands weapon, float distToAim, WeaponHands.MethodHitsBullets onHitBullet)
	{
		ShotBullet(bullet, weapon, weapon.GetPositionBolt(), weapon.GetAngleBolt(), distToAim, onHitBullet);
	}

	public void PlaySoundHit(WeaponHands weapon, string sharedMaterialName)
	{
		if (timePlaySound == Mathf.Floor(Time.time * 1000f) || soundsHit.Length == 0 || !(weapon.audioSource != null))
		{
			return;
		}
		timePlaySound = Mathf.Floor(Time.time * 1000f);
		if (soundsHit.Length == 1 && soundsHit[0].material.name == sharedMaterialName)
		{
			weapon.audioSource.PlayOneShot(soundsHit[0].sound);
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
					if (soundsHit[num].material.name == sharedMaterialName)
					{
						break;
					}
					num++;
					continue;
				}
				return;
			}
			weapon.audioSource.PlayOneShot(soundsHit[num].sound);
		}
	}
}
