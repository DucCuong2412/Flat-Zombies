using System;
using UnityEngine;

public class CudgelZombie : MonoBehaviour
{
	public int damage = 1;

	public int speedRotate;

	public float scaleGravity = 1f;

	public float floor;

	private Vector3 speed = default(Vector3);

	private Vector3 position;

	private void Start()
	{
	}

	private void FixedUpdate()
	{
		ref Vector3 reference = ref speed;
		float y = reference.y;
		float num = scaleGravity;
		Vector2 gravity = Physics2D.gravity;
		reference.y = y + num * gravity.y * Time.fixedDeltaTime;
		position = base.transform.position;
		position.x += speed.x * Time.fixedDeltaTime;
		position.y += speed.y * Time.fixedDeltaTime;
		base.transform.position = position;
		base.transform.Rotate(0f, 0f, (float)speedRotate * Time.fixedDeltaTime);
		if (position.y < floor)
		{
			base.enabled = false;
		}
	}

	public void SetSpeed(float speed, float angle)
	{
		Vector3 vector = base.transform.position;
		base.transform.SetParent(null, worldPositionStays: true);
		this.speed.x = Mathf.Cos(angle * ((float)Math.PI / 180f)) * speed;
		this.speed.y = Mathf.Sin(angle * ((float)Math.PI / 180f)) * speed;
	}
}
