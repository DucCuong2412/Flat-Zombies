using PVold;
using UnityEngine;
using UnityEngine.UI;

public class ZombieBarricade : MonoBehaviour
{
	public const float maxHealth = 100f;

	public static ZombieBarricade onStage;

	public static float currentHealth = -1f;

	public float health = 100f;

	public RectTransform indicatorHealth;

	public PlayerMove player;

	public float targetPositionX;

	public AnimatorSprite animatorWall;

	public AudioSource sourceAudio;

	public AudioClip soundDamage;

	public AudioClip soundBreak;

	public float angleBreak;

	public GameObject[] buttonsControlls = new GameObject[0];

	private float rotate;

	private Vector3 rotationWallStart = default(Vector3);

	private Vector3 rotationWallEnd = default(Vector3);

	private void OnDrawGizmos()
	{
		if (player != null)
		{
			Vector3 position = player.transform.position;
			Gizmos.DrawLine(position, new Vector3(targetPositionX, position.y, position.z));
		}
	}

	private void Start()
	{
		onStage = this;
		if (currentHealth < 0f)
		{
			currentHealth = health;
		}
		currentHealth = Mathf.Clamp(currentHealth, 0f, 100f);
		health = currentHealth;
		UpdateInidicatorHealth(health);
		if (health == 0f)
		{
			UnityEngine.Object.Destroy(sourceAudio);
			sourceAudio = null;
			animatorWall.GotoAndStop(animatorWall.totalFrames);
			OnStrikeZombie(0f);
		}
	}

	private void Update()
	{
		if (health == 0f)
		{
			float num = targetPositionX;
			Vector3 position = player.transform.position;
			if (num < position.x)
			{
				player.MoveLeft();
				return;
			}
			player.MoveStop();
			for (int i = 0; i < buttonsControlls.Length; i++)
			{
				buttonsControlls[i].SetActive(value: true);
			}
			UnityEngine.Object.Destroy(indicatorHealth.gameObject);
			UnityEngine.Object.Destroy(animatorWall);
			UnityEngine.Object.Destroy(this);
		}
		else if (rotate != 0f)
		{
			rotate -= 4f * Time.deltaTime;
			rotate = Mathf.Max(0f, rotate);
			animatorWall.transform.rotation = Quaternion.Euler(Vector3.Lerp(rotationWallEnd, rotationWallStart, rotate));
		}
	}

	private void OnDestroy()
	{
		onStage = null;
	}

	public void OnStrikeZombie(float damage)
	{
		health -= damage;
		health = Mathf.Max(health, 0f);
		currentHealth = Mathf.Round(health);
		if (health == 0f)
		{
			indicatorHealth.parent.gameObject.SetActive(value: false);
			onStage = null;
			animatorWall.Play();
			animatorWall.transform.rotation = Quaternion.identity;
			UnityEngine.Object.Destroy(base.gameObject.GetComponent<Collider2D>());
			if (sourceAudio != null && soundBreak != null)
			{
				sourceAudio.clip = soundBreak;
				sourceAudio.Play();
			}
		}
		else if (damage > 0f)
		{
			float t = UpdateInidicatorHealth(health);
			float num = Mathf.Lerp(angleBreak, 0f, t);
			animatorWall.transform.rotation = Quaternion.Euler(0f, 0f, num);
			ref Vector3 reference = ref rotationWallEnd;
			Vector3 eulerAngles = animatorWall.transform.rotation.eulerAngles;
			reference.z = eulerAngles.z;
			rotationWallStart.z = UnityEngine.Random.Range(num + 1f, num + 2.5f);
			rotate = 1f;
			sourceAudio.clip = soundDamage;
			if (soundBreak.length * 0.25f < sourceAudio.time || !sourceAudio.isPlaying)
			{
				sourceAudio.Play();
			}
		}
	}

	private float UpdateInidicatorHealth(float health)
	{
		float value = Mathf.Floor(health / 100f * 100f) / 100f;
		value = Mathf.Clamp01(value);
		if (value == 1f)
		{
			indicatorHealth.parent.gameObject.SetActive(value: false);
		}
		else if (!indicatorHealth.gameObject.activeInHierarchy)
		{
			indicatorHealth.parent.SetAsLastSibling();
			indicatorHealth.parent.gameObject.SetActive(value: true);
		}
		indicatorHealth.localScale = new Vector3(value, 1f, 1f);
		indicatorHealth.GetComponent<Image>().color = Color.Lerp(Color.yellow, Color.green, value);
		return value;
	}
}
