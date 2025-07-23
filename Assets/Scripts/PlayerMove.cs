using System;
using UnityEngine;
using UnityEngine.Events;

public class PlayerMove : MonoBehaviour
{
	[Serializable]
	public struct ListenerEventAnimation
	{
		public string name;

		public UnityEvent call;
	}

	public enum Move
	{
		NONE,
		LEFT,
		RIGHT,
		STRIKE
	}

	public float leftWall;

	public float rightWall;

	public float speedAimMode = 4f;

	public float speedFreeMode = 4f;

	public TouchButtonPointer buttonMoveRight;

	public TouchButtonPointer buttonMoveLeft;

	public TouchButtonPointer buttonStrikeLeg;

	public string stateMove = string.Empty;

	public string stateMoveFree = string.Empty;

	public string stateMoveBack = string.Empty;

	public string stateStand = string.Empty;

	public MoveModePlayer modeMove;

	[Space(6f)]
	public AudioClip soundLeg;

	public AudioClip soundHitLeg;

	private AudioSource sound;

	private Animator animator;

	private Vector3 line;

	public Move move;

	private Move wait;

	private float currentSpeed;

	private Vector3 localScale;

	private bool unlockButtonLeg;

	private bool stopStrike;

	private bool playSoundHit = true;

	private void OnDrawGizmosSelected()
	{
		if (base.enabled)
		{
			Gizmos.color = new Color(0f, 1f, 0.7f);
			line = base.transform.position;
			Gizmos.DrawLine(new Vector3(leftWall, line.y, line.z), new Vector3(leftWall, line.y + 14f, line.z));
			Gizmos.DrawLine(new Vector3(rightWall, line.y, line.z), new Vector3(rightWall, line.y + 14f, line.z));
		}
	}

	private void Start()
	{
		sound = base.gameObject.GetComponent<AudioSource>();
		animator = base.gameObject.GetComponent<Animator>();
		if (buttonMoveLeft != null && buttonMoveRight != null)
		{
			buttonMoveLeft.pointerDown.baseEvent.AddListener(MoveLeft);
			buttonMoveLeft.pointerUp.baseEvent.AddListener(MoveStop);
			buttonMoveRight.pointerDown.baseEvent.AddListener(MoveRight);
			buttonMoveRight.pointerUp.baseEvent.AddListener(MoveStop);
		}
		if (buttonStrikeLeg != null)
		{
			buttonStrikeLeg.pointerDown.baseEvent.AddListener(PlayStrikeLeg);
		}
	}

    private void LateUpdate()
    {
        if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D))
        {
            MoveStop();
        }
    }
    private void Update()
	{
        if (Input.GetKeyDown(KeyCode.A))
        {
            MoveLeft();
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            MoveRight();
        }
		//else if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D))
		//{
		//	MoveStop();
		//}

		if (Input.GetKeyDown(KeyCode.Space))
        {
            PlayStrikeLeg();
        }
        if (move == Move.RIGHT || move == Move.LEFT)
		{
			base.transform.Translate(Mathf.Floor(currentSpeed * Time.deltaTime * 1000f) * 0.001f, 0f, 0f);
			Vector3 position = base.transform.position;
			if (position.x < leftWall)
			{
				Transform transform = base.transform;
				float x = leftWall;
				Vector3 position2 = base.transform.position;
				float y = position2.y;
				Vector3 position3 = base.transform.position;
				transform.position = new Vector3(x, y, position3.z);
				return;
			}
			Vector3 position4 = base.transform.position;
			if (position4.x > rightWall)
			{
				Transform transform2 = base.transform;
				float x2 = rightWall;
				Vector3 position5 = base.transform.position;
				float y2 = position5.y;
				Vector3 position6 = base.transform.position;
				transform2.position = new Vector3(x2, y2, position6.z);
			}
		}
		else if (wait == Move.LEFT)
		{
			MoveLeft();
		}
		else if (wait == Move.RIGHT)
		{
			MoveRight();
		}
	}

	public void OnDisable()
	{
		CancelInvoke();
		MoveStop();
	}

	public void DeclareEvent(string name)
	{
		GameEvent.DeclareEvent(name, base.gameObject);
	}

	public void CrossFade(string state)
	{
		animator.CrossFade(state, 0.1f);
	}

	public void MoveLeft()
	{
		wait = Move.LEFT;
		if (animator.GetCurrentAnimatorStateInfo(0).IsName("Stand") && base.enabled && move == Move.NONE)
		{
			move = Move.LEFT;
			if (modeMove == MoveModePlayer.FREE)
			{
				animator.Play(stateMoveFree);
				localScale = base.transform.localScale;
				localScale.x = 0f - Mathf.Abs(localScale.x);
				base.transform.localScale = localScale;
				currentSpeed = 0f - speedFreeMode;
			}
			else
			{
				animator.Play(stateMoveBack);
				currentSpeed = 0f - speedAimMode;
			}
			base.gameObject.SendMessage("OnStartMove");
		}
	}

	public void MoveRight()
	{
		wait = Move.RIGHT;
		if (animator.GetCurrentAnimatorStateInfo(0).IsName("Stand") && base.enabled && move == Move.NONE)
		{
			move = Move.RIGHT;
			if (modeMove == MoveModePlayer.FREE)
			{
				animator.Play(stateMoveFree);
				localScale = base.transform.localScale;
				localScale.x = Mathf.Abs(localScale.x);
				base.transform.localScale = localScale;
				currentSpeed = speedFreeMode;
			}
			else
			{
				animator.Play(stateMove);
				currentSpeed = speedAimMode;
			}
			base.gameObject.SendMessage("OnStartMove");
		}
	}

	public void MoveStop()
	{
		wait = Move.NONE;
		if (move == Move.LEFT || move == Move.RIGHT)
		{
			move = Move.NONE;
			animator.Play(stateStand, 0, 0f);
		}
	}

	public void SwitchMoveMode(MoveModePlayer mode)
	{
		modeMove = mode;
	}

	public void BlockStrikeLeg(float frames, int speedAnimation)
	{
		if (buttonStrikeLeg != null && buttonStrikeLeg.isActiveAndEnabled)
		{
			buttonStrikeLeg.gameObject.SetActive(value: false);
			frames *= 0.5f;
			Invoke("UnlockStrikeLeg", Mathf.Floor(frames / (float)speedAnimation * 1000f) / 1000f);
			frames = 0f;
			unlockButtonLeg = true;
		}
	}

	public void UnlockStrikeLeg()
	{
		if (buttonStrikeLeg != null && unlockButtonLeg)
		{
			buttonStrikeLeg.gameObject.SetActive(value: true);
			CancelInvoke("UnlockStrikeLeg");
		}
	}

	public void PlayStrikeLeg()
	{
		if (move != Move.STRIKE && base.enabled)
		{
			MoveStop();
			base.gameObject.SendMessage("OnBeginStrikeLeg");
			playSoundHit = true;
			animator.Play("StrikeLeg", 0, 0f);
			move = Move.STRIKE;
			stopStrike = false;
			sound.PlayOneShot(soundLeg);
			CancelInvoke("StrikeLeg");
			CancelInvoke("StrikeLegComplete");
			Invoke("StrikeLeg", 0.15f);
			Invoke("StrikeLeg", 0.2f);
			Invoke("StrikeLeg", 0.25f);
			Invoke("StrikeLeg", 0.3f);
			Invoke("StrikeLeg", 0.35f);
			Invoke("StrikeLegComplete", 0.7f);
		}
	}

	public void StrikeLeg()
	{
		if (base.enabled && !stopStrike && ZombieInHome.PlayerStrike(base.transform, 2, animator.GetCurrentAnimatorStateInfo(0).length, "OnHitStrikeZombie"))
		{
			stopStrike = true;
			CancelInvoke("StrikeLeg");
		}
	}

	public void OnHitStrikeZombie()
	{
		if (playSoundHit)
		{
			sound.PlayOneShot(soundHitLeg);
			playSoundHit = false;
		}
	}

	public void StrikeLegComplete()
	{
		CancelInvoke("StrikeLeg");
		move = Move.NONE;
		base.gameObject.SendMessage("OnStrikeLegComplete");
	}
}
