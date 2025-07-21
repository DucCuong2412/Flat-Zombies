using PVold;
using UnityEngine;

public class WeaponMine : MonoBehaviour
{
	public float radius;

	public WeaponLine[] weapons = new WeaponLine[0];

	private AnimatorSprite[] animators = new AnimatorSprite[0];

	private AudioSource audioSource;

	private void OnDrawGizmos()
	{
		Gizmos.color = new Color(1f, 0.2f, 0f);
		Gizmos.DrawWireSphere(base.transform.position, radius);
		if (weapons.Length == 0)
		{
			weapons = base.gameObject.GetComponents<WeaponLine>();
		}
	}

	private void Start()
	{
		audioSource = UnityEngine.Object.FindObjectOfType<AudioSource>();
		animators = base.gameObject.GetComponentsInChildren<AnimatorSprite>(includeInactive: true);
		for (int i = 0; i < animators.Length; i++)
		{
			animators[i].render.sprite = null;
			animators[i].gameObject.SetActive(value: false);
			animators[i].transform.Rotate(0f, 0f, UnityEngine.Random.Range(0, 36) * 10);
		}
	}

	private void Update()
	{
		for (int i = 0; i < ZombieInHome.entities.Length; i++)
		{
			if (ZombieInHome.entities[i] != null)
			{
				float num = Vector3.Distance(ZombieInHome.entities[i].transform.position, base.transform.position);
				if (num <= radius && !ZombieInHome.entities[i].isDead)
				{
					ZombieInHome.entities[i].gameObject.tag = base.tag;
					Activation();
				}
			}
		}
	}

	public void Activation()
	{
		for (int i = 0; i < weapons.Length; i++)
		{
			weapons[i].audioSource = audioSource;
			weapons[i].tagOfEntity = base.gameObject.tag;
			weapons[i].Shot(100f, OnHitBullet);
			UnityEngine.Object.Destroy(weapons[i]);
		}
		for (int j = 0; j < animators.Length; j++)
		{
			animators[j].gameObject.SetActive(value: true);
			animators[j].Play();
		}
		UnityEngine.Object.Destroy(this);
	}

	public void OnHitBullet(HitBullet hit)
	{
	}
}
