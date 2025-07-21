using UnityEngine;

[CreateAssetMenu(menuName = "Zombie Character")]
public class ZombieCharacteristic : ScriptableObject
{
	public static int randomSkin;

	public Vector2 slide = default(Vector2);

	public float damageStrike = 1f;

	public string[] animationStart;

	public Color[] colorsBody;

	public Color[] colorsLegs;

	public RandomSpriteName[] randomSprites = new RandomSpriteName[0];

	public bool resetColorRender;

	public PhysicMaterialSprite[] physicsMaterialSprites = new PhysicMaterialSprite[0];

	public Sprite[] switchRenders = new Sprite[0];

	public AreaNameEnabled[] areaDamageDisable = new AreaNameEnabled[0];

	private SpriteRenderer[] renders = new SpriteRenderer[0];

	private Collider2D[] colliders;

	public void OnDrawGizmosZombie(ZombieInHome zombie)
	{
		Gizmos.color = new Color(0f, 1f, 0.25f);
		Gizmos.DrawWireCube(zombie.gameObject.transform.position, new Vector3(slide.x * 2f, slide.y * 2f));
		Vector3 to = zombie.gameObject.transform.position + new Vector3(slide.x, slide.y);
		Gizmos.DrawLine(zombie.gameObject.transform.position, to);
	}

	public void ChangeZombie(ZombieInHome zombie)
	{
		float num = 0f - slide.x + Mathf.Floor(100f * UnityEngine.Random.value) / 100f * (slide.x * 2f);
		float num2 = 0f - slide.y + Mathf.Floor(100f * UnityEngine.Random.value) / 100f * (slide.y * 2f);
		Transform transform = zombie.transform;
		Vector3 position = zombie.transform.position;
		float x = position.x + num;
		Vector3 position2 = zombie.transform.position;
		float y = position2.y + num2;
		Vector3 position3 = zombie.transform.position;
		transform.position = new Vector3(x, y, position3.z);
		zombie.damageStrike = damageStrike;
		Color newColorBody = zombie.colorBody;
		Color newColorLegs = zombie.colorLegs;
		if (colorsBody.Length != 0)
		{
			newColorBody = colorsBody[Random.Range(0, colorsBody.Length)];
		}
		if (colorsLegs.Length != 0)
		{
			newColorLegs = colorsLegs[Random.Range(0, colorsLegs.Length)];
		}
		if (animationStart.Length != 0)
		{
			zombie.AnimatorPlay(animationStart[Random.Range(0, animationStart.Length)]);
		}
		ChangeSprite(zombie.gameObject, zombie.colorBody, newColorBody, zombie.colorLegs, newColorLegs);
		for (int i = 0; i < areaDamageDisable.Length; i++)
		{
			for (int j = 0; j < zombie.areas.Length; j++)
			{
				if (areaDamageDisable[i].areaName == zombie.areas[j].areaName)
				{
					zombie.areas[j].enabled = areaDamageDisable[i].enabled;
				}
			}
		}
		randomSkin++;
		if (randomSkin == 1)
		{
			randomSkin = UnityEngine.Random.Range(1, 400);
		}
	}

	public void ChangeSprite(GameObject parent, Color currentColorBody, Color newColorBody, Color currentColorLegs, Color newColorLegs)
	{
		renders = parent.GetComponentsInChildren<SpriteRenderer>(includeInactive: true);
		for (int i = 0; i < renders.Length; i++)
		{
			if (renders[i].color == currentColorBody)
			{
				renders[i].color = newColorBody;
			}
			else if (renders[i].color == currentColorLegs)
			{
				renders[i].color = newColorLegs;
			}
			for (int j = 0; j < randomSprites.Length; j++)
			{
				if (!(randomSprites[j].replace == renders[i].sprite))
				{
					continue;
				}
				renders[i].sprite = randomSprites[j].sprites[randomSkin % randomSprites[j].sprites.Length];
				if (resetColorRender)
				{
					renders[i].color = Color.white;
				}
				colliders = renders[i].GetComponents<Collider2D>();
				for (int k = 0; k < physicsMaterialSprites.Length; k++)
				{
					int num = 0;
					while (renders[i].sprite == physicsMaterialSprites[k].sprite && num < colliders.Length)
					{
						colliders[num].sharedMaterial = physicsMaterialSprites[k].material;
						num++;
					}
				}
			}
			for (int l = 0; l < switchRenders.Length; l++)
			{
				if (renders[i].sprite.GetInstanceID() == switchRenders[l].GetInstanceID())
				{
					renders[i].gameObject.SetActive(!renders[i].gameObject.activeSelf);
					break;
				}
			}
		}
	}
}
