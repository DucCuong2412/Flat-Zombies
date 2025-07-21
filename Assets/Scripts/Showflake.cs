using UnityEngine;

public class Showflake
{
	public SpriteRenderer renderer;

	public GameObject gameObject;

	public Vector2 gravity;

	public Rect ground;

	public float currentGround;

	public Vector3 startPosition;

	public float widthSlide = 1f;

	public Showflake(Transform container, SpriteRenderer original, Sprite sprite, Vector2 gravity, Rect ground)
	{
		gameObject = new GameObject("Showflake" + container.childCount.ToString());
		gameObject.transform.SetParent(container);
		renderer = gameObject.AddComponent<SpriteRenderer>();
		renderer.sprite = sprite;
		renderer.color = original.color;
		renderer.sortingLayerName = original.sortingLayerName;
		renderer.sortingOrder = original.sortingOrder;
		this.ground = ground;
		this.gravity = gravity;
		Reset();
	}

	public void Reset()
	{
		gameObject.transform.localPosition = new Vector3(Random.Range(ground.xMin, ground.xMax), 0f);
		currentGround = Random.Range(ground.yMin, ground.yMax);
		startPosition = gameObject.transform.position;
		widthSlide = (float)Random.Range(10, 100) * 0.01f;
	}

	public void Update()
	{
		startPosition.x += gravity.x * (Time.deltaTime / Time.timeScale);
		Transform transform = gameObject.transform;
		float x = startPosition.x;
		float num = widthSlide;
		Vector3 position = gameObject.transform.position;
		float x2 = x + num * Mathf.Cos(position.y);
		Vector3 position2 = gameObject.transform.position;
		transform.position = new Vector3(x2, position2.y + gravity.y * Time.deltaTime, 0f);
		Vector3 position3 = gameObject.transform.position;
		if (position3.y < currentGround)
		{
			Reset();
		}
	}
}
