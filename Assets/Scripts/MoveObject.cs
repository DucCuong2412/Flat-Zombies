using UnityEngine;

public class MoveObject : MonoBehaviour
{
	[Tooltip("Активировать при запуске")]
	public bool activation = true;

	public Transform objectMove;

	public Vector2 speed = default(Vector2);

	private Vector3 startPosition = default(Vector3);

	private void Start()
	{
		if (objectMove == null)
		{
			objectMove = base.gameObject.transform;
		}
		startPosition = objectMove.position;
	}

	private void Update()
	{
		if (activation && (bool)objectMove)
		{
			objectMove.Translate(speed.x * Time.deltaTime, speed.y * Time.deltaTime, 0f);
		}
	}

	public void Reset()
	{
		activation = !activation;
		objectMove.position = startPosition;
	}
}
