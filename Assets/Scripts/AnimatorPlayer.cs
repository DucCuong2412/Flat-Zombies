using System;
using UnityEngine;
using UnityEngine.Events;

public class AnimatorPlayer : MonoBehaviour
{
	[Serializable]
	public struct ListenerEventAnimation
	{
		public string name;

		public UnityEvent call;
	}

	[HideInInspector]
	public bool isMove;

	public float speed;

	[Space(10f)]
	[Tooltip("События анимации")]
	public ListenerEventAnimation[] listenersEventsAnimation;

	private Animator animator;

	private void Start()
	{
		animator = base.gameObject.GetComponent<Animator>();
	}

	private void Update()
	{
		if (UnityEngine.Input.GetKeyDown(KeyCode.D))
		{
			animator.Play("Move");
			speed = Mathf.Abs(speed);
			isMove = true;
		}
		else if (UnityEngine.Input.GetKeyDown(KeyCode.A))
		{
			animator.Play("MoveBack");
			speed = 0f - Mathf.Abs(speed);
			UnityEngine.Debug.Log("speed:" + speed.ToString());
			isMove = true;
		}
		if (UnityEngine.Input.GetKeyUp(KeyCode.A) || UnityEngine.Input.GetKeyUp(KeyCode.D))
		{
			animator.Play("Stand");
			isMove = false;
		}
	}

	private void FixedUpdate()
	{
		if (isMove)
		{
			base.transform.Translate(speed, 0f, 0f);
		}
	}

	public void DeclareEvent(string name)
	{
		GameEvent.DeclareEvent(name, base.gameObject);
		for (int i = 0; i < listenersEventsAnimation.Length; i++)
		{
			if (listenersEventsAnimation[i].name == name)
			{
				listenersEventsAnimation[i].call.Invoke();
			}
		}
	}

	public void CrossFade(string state)
	{
		animator.CrossFade(state, 0.1f);
	}
}
