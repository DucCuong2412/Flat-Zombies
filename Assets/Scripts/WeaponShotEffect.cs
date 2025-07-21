using PVold;
using System;
using UnityEngine;

public class WeaponShotEffect : MonoBehaviour
{
	public bool randomRotate;

	[Tooltip("Отражать случайно по горизонтали/вертикали")]
	public bool flipX;

	[Tooltip("Отражать случайно по горизонтали/вертикали")]
	public bool flipY;

	[Range(-180f, 180f)]
	public float direction;

	[Range(0f, 180f)]
	public float directionRange;

	public float speedMin;

	public float speedMax;

	public WeaponShotEffect nextEffectShot;

	private AnimatorSprite animator;

	private Vector3 startPosition = default(Vector3);

	private Vector2 currentSpeed = default(Vector2);

	private float randomAngle;

	private void OnDrawGizmosSelected()
	{
		float num = direction;
		Vector3 eulerAngles = base.gameObject.transform.parent.rotation.eulerAngles;
		randomAngle = num + eulerAngles.z;
		Vector3 position = base.transform.position;
		Vector3 position2 = base.transform.position;
		float x = position2.x + 1f * Mathf.Cos(randomAngle * ((float)Math.PI / 180f));
		Vector3 position3 = base.transform.position;
		Gizmos.DrawLine(position, new Vector3(x, position3.y + 1f * Mathf.Sin(randomAngle * ((float)Math.PI / 180f))));
	}

	private void Awake()
	{
		animator = base.gameObject.GetComponent<AnimatorSprite>();
		animator.Stop();
		animator.render.sprite = null;
		startPosition = base.transform.localPosition;
	}

	private void Start()
	{
	}

	public void OnShot()
	{
		base.gameObject.SetActive(value: true);
		base.transform.localPosition = startPosition;
		base.enabled = true;
		float num = speedMin + UnityEngine.Random.value * speedMax;
		randomAngle = (direction + UnityEngine.Random.Range(0f - directionRange, directionRange)) * ((float)Math.PI / 180f);
		currentSpeed.x = num * Mathf.Cos(randomAngle);
		currentSpeed.y = num * Mathf.Sin(randomAngle);
		if (currentSpeed.x == 0f && currentSpeed.y == 0f)
		{
			base.enabled = false;
		}
		else if (randomRotate)
		{
			base.gameObject.transform.localRotation = Quaternion.Euler(0f, 0f, UnityEngine.Random.Range(0, 360));
		}
		else
		{
			base.gameObject.transform.localRotation = Quaternion.Euler(0f, 0f, randomAngle);
		}
		if (animator.isPlaying)
		{
			animator.SwitchNextAnimation();
			animator.PlayRandomFrame(3);
		}
		else
		{
			animator.SwitchRandomAnimation();
			animator.GotoAndPlay(1);
		}
		if (flipX)
		{
			animator.render.flipX = (UnityEngine.Random.value > 0.5f);
		}
		if (flipY)
		{
			animator.render.flipY = (UnityEngine.Random.value > 0.5f);
		}
		if (nextEffectShot != null)
		{
			nextEffectShot.OnShot();
		}
	}

	private void Update()
	{
		Transform transform = base.gameObject.transform;
		Vector3 localPosition = base.transform.localPosition;
		float x = localPosition.x + currentSpeed.x * Time.deltaTime;
		Vector3 localPosition2 = base.transform.localPosition;
		transform.localPosition = new Vector3(x, localPosition2.y + currentSpeed.y * Time.deltaTime);
		if (!animator.isPlaying)
		{
			base.gameObject.SetActive(value: false);
		}
	}
}
