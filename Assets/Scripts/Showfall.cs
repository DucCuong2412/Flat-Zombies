using UnityEngine;

public class Showfall : MonoBehaviour
{
	public static bool enable = true;

	public int quantity = 50;

	public Vector2 gravity;

	public Rect ground;

	public Sprite[] sprities;

	private Showflake[] showflakes;

	private SpriteRenderer render;

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = new Color(0f, 1f, 0.2f);
		Vector2 center = ground.center;
		float x = center.x;
		Vector3 position = base.transform.position;
		float y = position.y;
		Vector3 position2 = base.transform.position;
		Vector3 center2 = new Vector3(x, y - (position2.y - ground.yMin) / 2f);
		float width = ground.width;
		Vector3 position3 = base.transform.position;
		Gizmos.DrawWireCube(center2, new Vector3(width, position3.y - ground.yMin, 0f));
		Gizmos.DrawWireCube(ground.center, new Vector3(ground.width, ground.height, 0f));
	}

	private void Awake()
	{
		base.gameObject.SetActive(enable);
	}

	private void Start()
	{
		showflakes = new Showflake[quantity];
		render = GetComponent<SpriteRenderer>();
		for (int i = 0; i < showflakes.Length; i++)
		{
			showflakes[i] = new Showflake(base.transform, render, sprities[Random.Range(0, sprities.Length)], gravity, ground);
			Transform transform = showflakes[i].gameObject.transform;
			float x = UnityEngine.Random.Range(ground.xMin, ground.xMax);
			float yMin = ground.yMin;
			Vector3 position = base.transform.position;
			transform.position = new Vector3(x, UnityEngine.Random.Range(yMin, position.y));
		}
	}

	private void Update()
	{
		for (int i = 0; i < showflakes.Length; i++)
		{
			showflakes[i].Update();
		}
	}
}
