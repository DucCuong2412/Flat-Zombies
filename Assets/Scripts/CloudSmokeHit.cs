using PVold;
using System;
using System.Collections;
using UnityEngine;

public class CloudSmokeHit : MonoBehaviour, IEffectHitBullet
{
	private static CloudSmokeHit[] list = new CloudSmokeHit[0];

	public bool randomRotate;

	[Range(-180f, 180f)]
	public float direction;

	[Range(0f, 180f)]
	public float directionRange;

	public float speed;

	public float maxSpeed;

	public float distanceFade = 2f;

	public Color colorStart = new Color(1f, 1f, 1f, 1f);

	public Color colorFade = new Color(1f, 1f, 1f, 1f);

	public float timeLive = 5f;

	public float minRadius = 0.25f;

	[HideInInspector]
	public Transform body;

	[HideInInspector]
	public Vector3 localPositionHit = default(Vector3);

	private float delayStart = 0.5f;

	private float currentTimerLive;

	private AnimatorSprite animator;

	private Vector2 currentSpeed = default(Vector2);

	private Vector3 position = default(Vector3);

	private void OnDrawGizmosSelected()
	{
		Vector3 from = base.transform.position;
		Vector3 vector = base.transform.position;
		float x = vector.x + distanceFade * Mathf.Cos(direction * ((float)Math.PI / 180f));
		Vector3 vector2 = base.transform.position;
		Gizmos.DrawLine(from, new Vector3(x, vector2.y + distanceFade * Mathf.Sin(direction * ((float)Math.PI / 180f))));
	}

	private void Awake()
	{
		animator = base.gameObject.GetComponent<AnimatorSprite>();
		animator.render.sprite = null;
		delayStart = 0.25f + Mathf.Floor(UnityEngine.Random.value * 1000f) / 1000f;
	}

	private void Start()
	{
	}

	public void OnEffectHitBullet(HitBullet hitBullet)
	{
		body = hitBullet.collider.transform;
		localPositionHit = hitBullet.localPoint;
		currentTimerLive = timeLive;
		animator.Stop();
		animator.render.sprite = null;
		animator.render.color = colorStart;
		if (list.Length == 0 || list[0] == null)
		{
			list = hitBullet.cartridge.effectsHits.parent.GetComponentsInChildren<CloudSmokeHit>(includeInactive: true);
		}
		for (int i = 0; i < list.Length; i++)
		{
			if (list[i] != this && list[i].body == body && Mathf.Abs(Vector3.Distance(list[i].localPositionHit, localPositionHit)) < minRadius)
			{
				list[i].Disable();
			}
		}
		StopAllCoroutines();
		StartCoroutine(ResetWithDelay(hitBullet.raycastHit.point));
	}

	public IEnumerator ResetWithDelay(Vector3 globalPosition)
	{
		yield return new WaitForSeconds(delayStart);
		if (body != null)
		{
			Reset(body.TransformPoint(localPositionHit));
		}
		else
		{
			Disable();
		}
	}

	private void Reset(Vector3 globalPosition)
	{
		float num = speed + UnityEngine.Random.value * maxSpeed;
		float f = (direction + UnityEngine.Random.Range(0f - directionRange, directionRange)) * ((float)Math.PI / 180f);
		currentSpeed.x = num * Mathf.Cos(f);
		currentSpeed.y = num * Mathf.Sin(f);
		if (randomRotate)
		{
			base.gameObject.transform.rotation = Quaternion.Euler(0f, 0f, UnityEngine.Random.Range(0, 360));
		}
		base.transform.position = globalPosition;
		animator.speed = Mathf.RoundToInt(num / distanceFade * (float)animator.totalFrames);
		animator.GotoAndPlay(1);
	}

	public void Disable()
	{
		base.gameObject.SetActive(value: false);
	}

	private void FixedUpdate()
	{
		if (animator.isPlaying)
		{
			position = base.gameObject.transform.position;
			position = new Vector3(position.x + currentSpeed.x * Time.fixedDeltaTime, position.y + currentSpeed.y * Time.fixedDeltaTime);
			base.transform.position = position;
			float num = animator.currentFrame;
			num /= (float)animator.totalFrames;
			animator.render.color = Color.Lerp(colorStart, colorFade, num);
			currentTimerLive -= Time.fixedDeltaTime;
			if (num == 1f && (currentTimerLive <= 0f || body == null))
			{
				Disable();
			}
			else if (num == 1f && body != null)
			{
				Reset(body.TransformPoint(localPositionHit));
				float b = currentTimerLive / timeLive;
				b = Mathf.Max(0.25f, b);
				currentSpeed.x *= b;
				currentSpeed.y *= b;
				b = Mathf.Max(0.75f, b);
				animator.speed = Mathf.CeilToInt((float)animator.speed * b);
			}
		}
	}

	private void OnDestroy()
	{
		list = new CloudSmokeHit[0];
	}
}
