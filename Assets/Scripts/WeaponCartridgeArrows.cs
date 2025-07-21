using System;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapon Bullet Arrow")]
public class WeaponCartridgeArrows : WeaponCartridge
{
	public WeaponBaseArrow originalArrow;

	[Tooltip("Кол-в во пуль при одном выстреле")]
	public int numArrows = 1;

	[Tooltip("Максмлн отклонение пули в градусах, для создания разброса")]
	[Range(0f, 180f)]
	public float angleScatter;

	[Tooltip("Равномерное распределение пуль")]
	[Range(0f, 1f)]
	public float evenlySpread;

	public float timeTriggers = 0.8f;

	public float distHitArrow = 20f;

	[HideInInspector]
	public GameObject[] objectsHitStage = new GameObject[0];

	private WeaponHands weapon;

	private int idShotTrigger = -1994;

	private IEnumerator enumDeactive;

	public override void OnDrawGizmosSelectedWeapon(WeaponHands weapon)
	{
		base.OnDrawGizmosSelectedWeapon(weapon);
		float angleBolt = weapon.GetAngleBolt();
		Vector3 positionBolt = weapon.GetPositionBolt();
		float num = angleBolt * ((float)Math.PI / 180f);
		float num2 = 0f;
		float distance = sourceBullets.distance;
		float num3 = angleScatter * weapon.scaleAngleScatter;
		if (num3 != 0f)
		{
			Gizmos.color = new Color(1f, 0.5f, 0f);
			num -= num3 * ((float)Math.PI / 180f);
			Gizmos.DrawLine(positionBolt, new Vector2(positionBolt.x + Mathf.Cos(num) * distance, positionBolt.y + Mathf.Sin(num) * distance));
			num += num3 * ((float)Math.PI / 180f) * 2f;
			Gizmos.DrawLine(positionBolt, new Vector2(positionBolt.x + Mathf.Cos(num) * distance, positionBolt.y + Mathf.Sin(num) * distance));
			for (int i = 0; i < numArrows; i++)
			{
				num2 = GetAngle(angleBolt, num3, evenlySpread, i, numArrows);
				Gizmos.DrawLine(positionBolt, new Vector2(positionBolt.x + Mathf.Cos(num2) * distance, positionBolt.y + Mathf.Sin(num2) * distance));
			}
		}
	}

	public override void Shot(WeaponHands weapon, float distToAim, WeaponHands.MethodHitsBullets onHitBullet)
	{
		idShotTrigger = GetInstanceID();
		WeaponCartridge.ActivateTriggers(idShotTrigger);
		this.weapon = weapon;
		numArrows = Mathf.Max(1, numArrows);
		for (int i = 0; i < numArrows; i++)
		{
			WeaponBaseArrow weaponBaseArrow = UnityEngine.Object.Instantiate(originalArrow);
			weaponBaseArrow.transform.position = weapon.GetPositionBolt();
			weaponBaseArrow.transform.rotation = Quaternion.Euler(0f, 0f, GetAngle(weapon.GetAngleBolt(), angleScatter * weapon.scaleAngleScatter, evenlySpread, i, numArrows) * 57.29578f);
			weaponBaseArrow.OnShot(weapon, this, onHitBullet);
		}
		if (enumDeactive != null)
		{
			weapon.StopCoroutine(enumDeactive);
		}
		enumDeactive = DeactivateTriggers(idShotTrigger, timeTriggers);
		weapon.StartCoroutine(enumDeactive);
	}

	public override float GetScaleScoresBullet()
	{
		return 1f / (float)(sourceBullets.bullets + numArrows);
	}

	public void ActivateTriggers()
	{
		WeaponCartridge.ActivateTriggers(idShotTrigger);
	}

	public IEnumerator DeactivateTriggers(int idTrigger, float time)
	{
		yield return new WaitForSeconds(time);
		WeaponCartridge.DeactivateTriggers(idTrigger);
	}

	public void ShotArrow(WeaponBaseArrow arrow, float distToAim, WeaponHands.MethodHitsBullets onHitBullet)
	{
		if (weapon != null && Mathf.Abs(Vector3.Distance(weapon.transform.position, arrow.transform.position)) <= distHitArrow)
		{
			WeaponSourceBullets sourceBullets = base.sourceBullets;
			WeaponHands weaponHands = weapon;
			Vector2 rayPosition = arrow.transform.position;
			Vector3 eulerAngles = arrow.transform.rotation.eulerAngles;
			ShotBullet(sourceBullets, weaponHands, rayPosition, eulerAngles.z, distToAim, onHitBullet);
		}
		else
		{
			UnityEngine.Object.Destroy(arrow.gameObject);
		}
	}

	public bool HitToDead(Vector3 startPosition, float distLivedEntity)
	{
		for (int i = 0; i < WeaponCartridge.entities.Length && WeaponCartridge.entities[i] != null; i++)
		{
			if (!WeaponCartridge.entities[i].isDead && Mathf.Abs(Vector3.Distance(startPosition, WeaponCartridge.entities[i].transform.position)) < distLivedEntity)
			{
				return false;
			}
		}
		return true;
	}
}
