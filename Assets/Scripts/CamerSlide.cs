using UnityEngine;

public class CamerSlide : MonoBehaviour
{
	public Vector2 position;

	public Transform player;

	private void OnDrawGizmosSelected()
	{
		if (base.isActiveAndEnabled)
		{
			Gizmos.color = new Color(0f, 0f, 0f);
			Vector2 vector = base.gameObject.transform.TransformPoint(position);
			Gizmos.DrawLine(new Vector2(vector.x - 1f, vector.y), new Vector2(vector.x + 1f, vector.y));
			Gizmos.DrawLine(new Vector2(vector.x, vector.y - 1f), new Vector2(vector.x, vector.y + 1f));
			if (position.x == 0f && position.y == 0f && player != null)
			{
				position = base.gameObject.transform.InverseTransformPoint(player.position);
			}
		}
	}

	private void Start()
	{
	}
}
