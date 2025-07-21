using UnityEngine;

public class SpliceTriggers : ComponentArea
{
	public Collider2D body;

	public float lineSplice;

	private Collider2D[] colliders2D;

	public override void OnDrawGizmosSelected()
	{
		if (body != null)
		{
			Gizmos.color = Color.red;
			Gizmos.DrawLine(body.transform.TransformPoint(-0.8f, lineSplice, 0f), body.transform.TransformPoint(0.6f, lineSplice, 0f));
		}
	}

	public override void Activation(AreaDamage area)
	{
		colliders2D = body.gameObject.GetComponents<Collider2D>();
		for (int i = 0; i < colliders2D.Length; i++)
		{
			Vector2 offset = colliders2D[i].offset;
			if (offset.y > lineSplice)
			{
				UnityEngine.Object.Destroy(colliders2D[i]);
			}
		}
	}
}
