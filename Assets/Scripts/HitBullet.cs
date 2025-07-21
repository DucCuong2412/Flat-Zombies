using UnityEngine;

public struct HitBullet
{
	public int id;

	public RaycastHit2D raycastHit;

	public Collider2D collider;

	public Vector2 localPoint;

	public float power;

	public float damage;

	public int countHits;

	public float impulse;

	public Vector2 rayPosition;

	public Vector2 rayDirection;

	public WeaponHands weapon;

	public WeaponCartridge cartridge;

	public WeaponSourceBullets cartridgeRay;

	public Entity entity;

	public int idShot;

	public void Reset()
	{
		weapon = null;
	}

	public Vector2 GetForceImpulse()
	{
		return raycastHit.normal * (-1f * power * impulse);
	}

	public bool IsEmpty()
	{
		return id > 0;
	}
}
