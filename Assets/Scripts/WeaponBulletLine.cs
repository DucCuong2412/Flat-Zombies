using UnityEngine;

[CreateAssetMenu(menuName = "Weapon Bullet Line")]
public class WeaponBulletLine : WeaponCartridge
{
	[Tooltip("Дистанция в пределах камеры")]
	public bool distCamera;

	public override void StartWeapon(WeaponHands weapon)
	{
		if (distCamera)
		{
			ref WeaponSourceBullets sourceBullets = ref base.sourceBullets;
			float num = 1f + Camera.main.aspect * Camera.main.orthographicSize;
			Vector3 position = weapon.transform.position;
			float x = position.x;
			Vector3 position2 = Camera.main.transform.position;
			sourceBullets.distance = num + Mathf.Abs(x - position2.x);
			base.sourceBullets.distance = Mathf.Floor(base.sourceBullets.distance * 100f) / 100f;
		}
	}

	public override void Shot(WeaponHands weapon, float distToAim, WeaponHands.MethodHitsBullets onHitBullet)
	{
		ShotBullet(sourceBullets, weapon, distToAim, onHitBullet);
	}
}
