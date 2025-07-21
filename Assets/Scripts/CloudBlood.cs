using PVold;
using System;
using UnityEngine;

public class CloudBlood : MonoBehaviour, IEffectHitBullet
{
	public const float maxDist = 0.75f;

	private static int lastIdShot = 0;

	private static Vector3 lastPosition = default(Vector3);

	public bool randomRotate;

	[Range(-180f, 180f)]
	public float direction;

	[Range(0f, 180f)]
	public float directionRange;

	public bool localAngle;

	public float speed;

	public float maxSpeed;

	private AnimatorSprite animator;

	private Vector3 startPosition = default(Vector3);

	private Vector2 currentSpeed = default(Vector2);

	private float randomAngle;

	private void OnDrawGizmosSelected()
	{
		if (localAngle)
		{
			float num = direction;
			Vector3 eulerAngles = base.gameObject.transform.parent.rotation.eulerAngles;
			randomAngle = num + eulerAngles.z;
		}
		Vector3 position = base.transform.position;
		Vector3 position2 = base.transform.position;
		float x = position2.x + 1f * Mathf.Cos(randomAngle * ((float)Math.PI / 180f));
		Vector3 position3 = base.transform.position;
		Gizmos.DrawLine(position, new Vector3(x, position3.y + 1f * Mathf.Sin(randomAngle * ((float)Math.PI / 180f))));
		Gizmos.color = Color.green;
		Gizmos.DrawLine(lastPosition, base.transform.position);
	}

	private void Awake()
	{
		animator = base.gameObject.GetComponent<AnimatorSprite>();
		startPosition = base.transform.localPosition;
	}

	public void OnEffectHitBullet(HitBullet hitBullet)
	{
		float num = speed + UnityEngine.Random.value * maxSpeed;
		randomAngle = (direction + UnityEngine.Random.Range(0f - directionRange, directionRange)) * ((float)Math.PI / 180f);
		if (localAngle)
		{
			float num2 = randomAngle;
			Vector3 eulerAngles = base.gameObject.transform.parent.rotation.eulerAngles;
			randomAngle = num2 + eulerAngles.z;
		}
		currentSpeed.x = num * Mathf.Cos(randomAngle);
		currentSpeed.y = num * Mathf.Sin(randomAngle);
		if (randomRotate)
		{
			base.gameObject.transform.rotation = Quaternion.Euler(0f, 0f, UnityEngine.Random.Range(0, 360));
		}
		else
		{
			Transform transform = base.gameObject.transform;
			Vector2 normal = hitBullet.raycastHit.normal;
			float y = 0f - normal.y;
			Vector2 normal2 = hitBullet.raycastHit.normal;
			transform.rotation = Quaternion.Euler(0f, 0f, 57.29578f * Mathf.Atan2(y, 0f - normal2.x));
		}
		base.transform.position = hitBullet.raycastHit.point;
		if (animator != null)
		{
			animator.GotoAndPlay(1);
		}
		if (lastIdShot == hitBullet.idShot)
		{
			lastPosition = hitBullet.raycastHit.point;
			base.transform.Translate(0.375f, Mathf.Floor(UnityEngine.Random.value * 0.75f * 200f) / 200f - 0.75f, 0f);
		}
		lastIdShot = hitBullet.idShot;
	}

	private void FixedUpdate()
	{
		Transform transform = base.gameObject.transform;
		Vector3 position = base.transform.position;
		float x = position.x + currentSpeed.x * Time.fixedDeltaTime;
		Vector3 position2 = base.transform.position;
		transform.position = new Vector3(x, position2.y + currentSpeed.y * Time.fixedDeltaTime);
		if (!animator.isPlaying)
		{
			base.transform.localPosition = startPosition;
			base.gameObject.SetActive(value: false);
		}
	}
}
