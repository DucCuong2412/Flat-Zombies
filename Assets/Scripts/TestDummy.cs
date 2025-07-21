using UnityEngine;

public class TestDummy : Entity
{
	public static int kills = 0;

	public static TestDummy[] ragdolls = new TestDummy[4];

	public static TestDummy[] entities = new TestDummy[2];

	public Material materialHolesSkin;

	public SpriteRenderer[] spritesRigidbodies;

	public Rigidbody2D[] rigidbodies;

	private Vector2 force;

	private Collider2D[] colldersHit;

	public static int maxRagdolls
	{
		get
		{
			return ragdolls.Length;
		}
		set
		{
			ragdolls = new TestDummy[Mathf.Max(1, value)];
		}
	}

	public static bool PlayerStrike(Transform player, int hits)
	{
		int num = 0;
		for (int i = 0; i < entities.Length; i++)
		{
			if (num >= hits)
			{
				break;
			}
			if (entities[i] != null && !entities[i].isDead)
			{
				Vector3 position = player.position;
				float x = position.x;
				Vector3 position2 = entities[i].transform.position;
				if (Mathf.Abs(x - position2.x) < 3.5f)
				{
					entities[i].Death();
					num++;
				}
			}
		}
		return num != 0;
	}

	private void Start()
	{
		if (DamageOfSprite.enabledComponents && materialHolesSkin != null && SpriteHoles.testedNameShader == string.Empty)
		{
			SpriteHoles.TestMaterial(materialHolesSkin);
		}
		for (int i = 0; i < rigidbodies.Length; i++)
		{
			rigidbodies[i].isKinematic = true;
		}
		int num = 0;
		while (true)
		{
			if (num < entities.Length)
			{
				if (entities[num] == null)
				{
					break;
				}
				num++;
				continue;
			}
			return;
		}
		entities[num] = this;
	}

	private void Update()
	{
	}

	public override void OnHitBullet(HitBullet hitBullet)
	{
		bool flag = hitBullet.raycastHit.collider.gameObject.CompareTag(base.tag);
		colldersHit = Physics2D.OverlapCircleAll(hitBullet.raycastHit.point, 0.2f, hitBullet.cartridgeRay.layerBodies);
		int num = 0;
		if (flag && DamageOfSprite.enabledComponents)
		{
			for (int i = 0; i < spritesRigidbodies.Length; i++)
			{
				for (num = 0; num < colldersHit.Length; num++)
				{
					if (colldersHit[num].gameObject == spritesRigidbodies[i].gameObject)
					{
						SpriteHoles.AddHole(spritesRigidbodies[i], hitBullet.raycastHit.point, materialHolesSkin);
						break;
					}
				}
			}
		}
		if (colldersHit.Length == 0)
		{
			return;
		}
		force = hitBullet.raycastHit.normal * (-1f * hitBullet.power * hitBullet.weapon.cartridge.sourceBullets.impulse / (float)colldersHit.Length);
		for (int j = 0; j < rigidbodies.Length; j++)
		{
			for (num = 0; num < colldersHit.Length; num++)
			{
				if (colldersHit[num].gameObject == rigidbodies[j].gameObject)
				{
					rigidbodies[j].AddForceAtPosition(force, hitBullet.raycastHit.point);
					break;
				}
			}
		}
	}

	public override void Death()
	{
		base.Death();
		for (int i = 0; i < rigidbodies.Length; i++)
		{
			rigidbodies[i].isKinematic = false;
		}
		if (ragdolls[ragdolls.Length - 1] != null)
		{
			UnityEngine.Object.Destroy(ragdolls[ragdolls.Length - 1].gameObject);
		}
		for (int num = ragdolls.Length - 1; num > 0; num--)
		{
			ragdolls[num] = ragdolls[num - 1];
		}
		ragdolls[0] = this;
		kills++;
	}
}
