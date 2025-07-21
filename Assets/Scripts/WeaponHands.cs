using PVold;
using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AnimatorSprite))]
public class WeaponHands : MonoBehaviour
{
	public delegate void MethodHitsBullets(HitBullet hit);

	public const float maxDeltaTime = 0.04f;

	public static int numCreatedBullets;

	public static bool shells = true;

	[Tooltip("Идентификатор оружия")]
	public string ID;

	[HideInInspector]
	public float recoil;

	[Tooltip("Скорость увеличения отдачи, в градусах")]
	[Range(0f, 10f)]
	public float recoilSpeed;

	[Tooltip("Максимальный угол отдачи, в градусах")]
	[Range(0f, 10f)]
	public float recoilMax;

	[Tooltip("Скорость уменьшения отдачи, в градусы/секунду")]
	[Range(0f, 180f)]
	public float recoilDecrease;

	[Tooltip("Угол отклонения при стрельбе")]
	[Range(0f, 10f)]
	public float direction;

	[HideInInspector]
	public float currentDirection;

	[Space(6f)]
	[Tooltip("Высота ствола")]
	public float boltX;

	public float boltY;

	[Range(0f, 360f)]
	public float boltAngle;

	[Space(6f)]
	[Tooltip("Автоматическое оружие")]
	public bool automat;

	[Tooltip("Скорострельность. Кол-во выстрелов в минуту")]
	public int shotsInMinute = 750;

	[Tooltip("Кол-во патронов в одном магазине,\n0 - бесконечно")]
	public int magazineMax = 30;

	[Tooltip("Множитель угола для разброса пуль, если ствол оружия длинный, то разброс должен быть меньше")]
	[Range(0f, 2f)]
	public float scaleAngleScatter = 1f;

	[Tooltip("Задержка перед запуском пули после нажатия на курок, в скндх.")]
	public float delayBullet;

	public WeaponCartridge cartridge;

	[Space(8f)]
	public AudioClip audioShot;

	[Tooltip("Случайная анимация выстрела")]
	public NameAnimationFire[] shotAnimations = new NameAnimationFire[0];

	[Tooltip("Анимация добавления патронов, после завершения которой происходит добавление патронов")]
	public string addBulletAnim;

	[Tooltip("Воспроизведение анимации после addBulletAnim для повторного добавление патрона, если оружие не заражено полностью \nПри завершении анимации происходит добавление патронов")]
	public string anotherBulletAnim;

	[Tooltip("Анимация после добавления патронов, если в оружии уже есть патроны, то нет необходимости дергать затвор")]
	public string completeReloadAnim;

	[Tooltip("Анимация затвора после перезарядки, если магазин был пустым")]
	public string boltAnim;

	[Tooltip("Анимация когда перезарядка завершена")]
	public string reloadCompleteAnim;

	[Tooltip("Анимация при возникновении события OnEnabled")]
	public string onEnableAnim;

	[Space(8f)]
	[Tooltip("Добавление патронов в магазин после анимации перезарядки")]
	public int addBulletsReload = 1;

	[Tooltip("Число выстрелов для анимации")]
	public int stepAnimationShot;

	[Tooltip("Дополнительная задержка/заморозка оружия после выстрела")]
	public float timeFreezeShot;

	public SoundFrameAnimation[] soundsAnimation = new SoundFrameAnimation[0];

	public SourceWeaponShell shell = new SourceWeaponShell(0);

	public SourceWeaponShell magazineAnimation = new SourceWeaponShell(0);

	public SpriteRenderer renderInSpare;

	[Tooltip("Линия для лазера (необязательно)")]
	public LineRenderer laser;

	public float distLaser = 123f;

	[HideInInspector]
	public bool stateReload;

	[HideInInspector]
	public AnimatorSprite animator;

	[HideInInspector]
	public AudioSource audioSource;

	private float sizePauseShot;

	private float pauseShot;

	private bool laserLastValue = true;

	private bool currentTriggerShot = true;

	private bool abortAddBullets;

	private int lastFrameID = -1;

	private int numShots;

	private float timeMarkAnimationShot;

	private int magazCurrent = 1000000;

	private Vector2 globalPosition = default(Vector2);

	private float angleRad;

	private RaycastHit2D[] hitRay;

	private RaycastHit2D hitLaser;

	private Vector3 localPoint;

	private Vector2 rayDirection;

	private bool emptyMagazine;

	private string nameAnimationBolt;

	private bool runShell;

	private bool runMagazin;

	public int magazine
	{
		get
		{
			magazCurrent = Mathf.Min(magazineMax, magazCurrent);
			if (magazineMax == 0)
			{
				magazCurrent = 1;
			}
			return magazCurrent;
		}
		set
		{
			magazineMax = Mathf.Max(0, magazineMax);
			magazCurrent = Mathf.Min(value, magazineMax);
			if (magazineMax == 0)
			{
				magazCurrent = 1;
			}
		}
	}

	private void Awake()
	{
		animator = base.gameObject.GetComponent<AnimatorSprite>();
		animator.loop = false;
		audioSource = base.gameObject.GetComponentInParent<AudioSource>();
		sizePauseShot = 1f / ((float)shotsInMinute / 60f);
		sizePauseShot = Mathf.Round(sizePauseShot * 100000f) / 100000f;
		recoilMax = Mathf.Max(recoilMax, recoilSpeed);
		shell.Awake(base.gameObject.name);
		magazineAnimation.Awake(base.gameObject.name);
		cartridge.AwakeWeapon(this);
		SetActiveLaser(active: false);
	}

	private void OnDrawGizmos()
	{
		if (base.enabled)
		{
			if (cartridge != null)
			{
				cartridge.OnDrawGizmosWeapon(this);
			}
			if (shell.angle != shell.angleScatter)
			{
				Gizmos.color = new Color(1f, 0.7f, 0f);
				globalPosition = base.transform.TransformPoint(shell.position.x, shell.position.y, 0f);
				Vector3 eulerAngles = base.transform.rotation.eulerAngles;
				angleRad = (eulerAngles.z + (float)shell.angle) * ((float)Math.PI / 180f);
				Gizmos.DrawLine(globalPosition, new Vector2(globalPosition.x + Mathf.Cos(angleRad) * 1f, globalPosition.y + Mathf.Sin(angleRad) * 1f));
				angleRad += (float)shell.angleScatter * ((float)Math.PI / 180f);
				Gizmos.DrawLine(globalPosition, new Vector2(globalPosition.x + Mathf.Cos(angleRad) * 1f, globalPosition.y + Mathf.Sin(angleRad) * 1f));
				Vector3 eulerAngles2 = base.transform.rotation.eulerAngles;
				angleRad = (eulerAngles2.z + (float)shell.angleRotation) * ((float)Math.PI / 180f);
				Gizmos.DrawLine(globalPosition, new Vector2(globalPosition.x + Mathf.Cos(angleRad) * 0.25f, globalPosition.y + Mathf.Sin(angleRad) * 0.25f));
			}
			if (magazineAnimation.angle != magazineAnimation.angleScatter)
			{
				Gizmos.color = new Color(1f, 1f, 1f);
				globalPosition = base.transform.TransformPoint(magazineAnimation.position.x, magazineAnimation.position.y, 0f);
				Vector3 eulerAngles3 = base.transform.rotation.eulerAngles;
				angleRad = (eulerAngles3.z + (float)magazineAnimation.angle) * ((float)Math.PI / 180f);
				Gizmos.DrawLine(globalPosition, new Vector2(globalPosition.x + Mathf.Cos(angleRad) * 1f, globalPosition.y + Mathf.Sin(angleRad) * 1f));
				angleRad += (float)magazineAnimation.angleScatter * ((float)Math.PI / 180f);
				Gizmos.DrawLine(globalPosition, new Vector2(globalPosition.x + Mathf.Cos(angleRad) * 1f, globalPosition.y + Mathf.Sin(angleRad) * 1f));
				Vector3 eulerAngles4 = base.transform.rotation.eulerAngles;
				angleRad = (eulerAngles4.z + (float)magazineAnimation.angleRotation) * ((float)Math.PI / 180f);
				Gizmos.DrawLine(globalPosition, new Vector2(globalPosition.x + Mathf.Cos(angleRad) * 0.25f, globalPosition.y + Mathf.Sin(angleRad) * 0.25f));
			}
		}
	}

	private void OnDrawGizmosSelected()
	{
		if (base.enabled && cartridge != null)
		{
			cartridge.OnDrawGizmosSelectedWeapon(this);
		}
	}

	private void Start()
	{
		cartridge.StartWeapon(this);
	}

	private void OnEnable()
	{
		animator.enabled = true;
		if (magazine == 0)
		{
			StartReload();
		}
	}

	private void OnDisable()
	{
		if (!stateReload && !string.IsNullOrEmpty(onEnableAnim))
		{
			animator.SwitchAnimation(onEnableAnim);
			animator.GotoAndPlay(1);
		}
		animator.enabled = false;
		SetActiveLaser(active: false);
	}

	private void FixedUpdate()
	{
		if (!(laser != null) || stateReload || animator.isPlaying)
		{
			return;
		}
		SetActiveLaser(active: true);
		rayDirection = base.transform.right;
		ref Vector2 reference = ref rayDirection;
		float x = rayDirection.x;
		Vector3 lossyScale = base.transform.lossyScale;
		reference.x = x * lossyScale.x;
		ref Vector2 reference2 = ref rayDirection;
		float y = rayDirection.y;
		Vector3 lossyScale2 = base.transform.lossyScale;
		reference2.y = y * lossyScale2.x;
		laser.SetPosition(1, new Vector3(distLaser, boltY));
		if (string.IsNullOrEmpty(cartridge.sourceBullets.tagEntity))
		{
			hitLaser = Physics2D.Raycast(GetPositionBolt(), rayDirection, distLaser, cartridge.sourceBullets.layerBodies.value);
			if (hitLaser.collider != null)
			{
				localPoint = base.transform.InverseTransformPoint(hitLaser.point);
				localPoint.y = boltY;
				laser.SetPosition(1, localPoint);
			}
			return;
		}
		hitRay = Physics2D.RaycastAll(GetPositionBolt(), rayDirection, distLaser, cartridge.sourceBullets.layerBodies.value);
		int num = 0;
		while (true)
		{
			if (num < hitRay.Length)
			{
				if (hitRay[num].collider.gameObject.CompareTag(cartridge.sourceBullets.tagEntity))
				{
					break;
				}
				num++;
				continue;
			}
			return;
		}
		localPoint = base.transform.InverseTransformPoint(hitRay[num].point);
		localPoint.y = boltY;
		laser.SetPosition(1, localPoint);
	}

	private void LateUpdate()
	{
		if (animator.isPlaying && lastFrameID != animator.render.sprite.GetInstanceID())
		{
			lastFrameID = animator.render.sprite.GetInstanceID();
			for (int i = 0; i < soundsAnimation.Length; i++)
			{
				if (soundsAnimation[i].frame.GetInstanceID() == lastFrameID && audioSource != null)
				{
					audioSource.PlayOneShot(soundsAnimation[i].sound);
					break;
				}
			}
		}
		if (stateReload && animator.currentFrame == animator.totalFrames)
		{
			if (animator.currentAnimation.name == addBulletAnim || animator.currentAnimation.name == anotherBulletAnim)
			{
				AddBullets(addBulletsReload);
			}
			if (animator.currentAnimation.name == boltAnim || animator.currentAnimation.name == completeReloadAnim)
			{
				StopReload();
			}
			else if (!string.IsNullOrEmpty(anotherBulletAnim) && magazine < magazineMax && !abortAddBullets)
			{
				animator.SwitchAnimation(anotherBulletAnim);
				animator.GotoAndPlay(1);
			}
			else if (!string.IsNullOrEmpty(boltAnim) && emptyMagazine)
			{
				animator.SwitchAnimation(boltAnim);
				animator.GotoAndPlay(1);
			}
			else if (!string.IsNullOrEmpty(completeReloadAnim))
			{
				animator.SwitchAnimation(completeReloadAnim);
				animator.GotoAndPlay(1);
			}
			else
			{
				StopReload();
			}
			UnityEngine.Debug.Log("AddBullets:" + animator.render.sprite.name + "," + emptyMagazine.ToString());
		}
		if (runShell && shell.frame.name == animator.render.sprite.name)
		{
			shell.Reset(base.transform);
			runShell = false;
		}
		if (stateReload && runMagazin && magazineAnimation.frame.name == animator.render.sprite.name)
		{
			magazineAnimation.Reset(base.transform);
			runMagazin = false;
		}
	}

	public bool IsReady(bool trigger)
	{
		if (Time.time < timeMarkAnimationShot)
		{
			return false;
		}
		if (pauseShot < 0f)
		{
			pauseShot += Time.deltaTime;
		}
		if (trigger == currentTriggerShot)
		{
			currentTriggerShot = true;
		}
		else
		{
			trigger = false;
		}
		trigger = (trigger && base.enabled && pauseShot >= 0f && magazine > 0);
		return trigger && currentTriggerShot;
	}

	public void OnKeyShot(bool keyDown)
	{
		float num = animator.currentFrame;
		float num2 = animator.totalFrames;
		num /= num2;
		if (!abortAddBullets && stateReload && !string.IsNullOrEmpty(completeReloadAnim) && animator.currentAnimation.name == anotherBulletAnim && magazine > 0 && keyDown)
		{
			abortAddBullets = true;
			if (!string.IsNullOrEmpty(boltAnim) && emptyMagazine)
			{
				animator.SwitchAnimation(boltAnim);
				animator.GotoAndPlay(1);
			}
			else if (!string.IsNullOrEmpty(completeReloadAnim))
			{
				animator.SwitchAnimation(completeReloadAnim);
				animator.GotoAndPlay(1);
			}
		}
		if (keyDown && magazine <= 0)
		{
			StartReload();
		}
	}

	public void Shot(float distToAim, MethodHitsBullets onHitBullet)
	{
		if (audioSource != null)
		{
			audioSource.clip = audioShot;
			audioSource.Play();
		}
		pauseShot = 0f - sizePauseShot;
		recoil = Mathf.Min(recoil + recoilSpeed, recoilMax + currentDirection);
		currentDirection = 0f - direction + Mathf.Ceil(UnityEngine.Random.value * direction * 2f);
		runShell = (shells && shell.frame != null);
		if (shells && shell.frame == null)
		{
			shell.Reset(base.transform);
		}
		if (magazineMax > 0)
		{
			magazine--;
		}
		currentTriggerShot = automat;
		if (magazineMax == 1 && !stateReload)
		{
			StartReload();
		}
		else
		{
			AnimationShot();
		}
		if (delayBullet == 0f)
		{
			cartridge.Shot(this, distToAim, onHitBullet);
		}
		else
		{
			StartCoroutine(ShotDelay(distToAim, onHitBullet));
		}
	}

	public IEnumerator ShotDelay(float distToAim, MethodHitsBullets onHitBullet)
	{
		yield return new WaitForSeconds(delayBullet);
		cartridge.Shot(this, distToAim, onHitBullet);
	}

	private void AnimationShot()
	{
		numShots++;
		if (shotAnimations.Length == 1)
		{
			animator.SwitchAnimation(shotAnimations[0].animation);
			if (shotAnimations[0].fire != null)
			{
				shotAnimations[0].fire.OnShot();
			}
		}
		else if (shotAnimations.Length == 2)
		{
			if (animator.currentAnimation.name == shotAnimations[0].animation)
			{
				animator.SwitchAnimation(shotAnimations[1].animation);
				if (shotAnimations[1].fire != null)
				{
					shotAnimations[1].fire.OnShot();
				}
			}
			else
			{
				animator.SwitchAnimation(shotAnimations[0].animation);
				if (shotAnimations[0].fire != null)
				{
					shotAnimations[0].fire.OnShot();
				}
			}
		}
		else if (shotAnimations.Length >= 3)
		{
			int num = UnityEngine.Random.Range(0, shotAnimations.Length);
			animator.SwitchAnimation(shotAnimations[num].animation);
			if (shotAnimations[num].fire != null)
			{
				shotAnimations[num].fire.OnShot();
			}
		}
		animator.GotoAndStop(1);
		stepAnimationShot = Mathf.Max(1, stepAnimationShot);
		if (numShots % stepAnimationShot == 0)
		{
			animator.GotoAndPlay(1);
			timeMarkAnimationShot = Time.time + timeFreezeShot;
			if (animator.currentAnimation.frames.Length >= 6)
			{
				SetActiveLaser(active: false);
			}
		}
	}

	public bool StartReload()
	{
		if (base.enabled && !stateReload && magazineMax > 0 && magazine < magazineMax && !string.IsNullOrEmpty(addBulletAnim))
		{
			animator.SwitchAnimation(addBulletAnim);
			animator.GotoAndPlay(1);
			SetActiveLaser(active: false);
			emptyMagazine = (magazine == 0);
			stateReload = true;
			runMagazin = (shells && magazineAnimation.frame != null);
			if (string.IsNullOrEmpty(anotherBulletAnim))
			{
				numShots = 0;
			}
			return true;
		}
		return false;
	}

	public void AddBullets(int bullets)
	{
		bullets = Mathf.Min(bullets, magazineMax);
		magazine += bullets;
	}

	public void StopReload()
	{
		if (!stateReload)
		{
			base.transform.localRotation = default(Quaternion);
		}
		animator.SwitchAnimation(reloadCompleteAnim);
		animator.GotoAndStop(1);
		stateReload = false;
		abortAddBullets = false;
		base.enabled = true;
		SetActiveLaser(active: true);
	}

	public float GetRecoil(float deltaTime)
	{
		deltaTime = Mathf.Min(0.04f, deltaTime);
		recoil = Mathf.Max(recoil - recoilDecrease * deltaTime, 0f);
		return recoil;
	}

	public Vector3 GetPositionBolt()
	{
		return base.transform.TransformPoint(boltX, boltY, 0f);
	}

	public float GetAngleBolt()
	{
		float num = boltAngle;
		Vector3 eulerAngles = base.transform.rotation.eulerAngles;
		return num + eulerAngles.z;
	}

	public void SetActiveLaser(bool active)
	{
		if (laser == null)
		{
			laserLastValue = false;
		}
		else if (active != laser.enabled)
		{
			laserLastValue = laser.enabled;
			laser.enabled = active;
			laser.sortingLayerName = animator.render.sortingLayerName;
			laser.sortingOrder = animator.render.sortingOrder - 1;
		}
	}

	public void SetActiveLaser()
	{
		SetActiveLaser(laserLastValue);
	}

	public void CreateBullet(Vector2 rayPosition, Vector2 rayDirection, float dist, int layerMask, string tagEntity, float damage, float impulse, HitsBullet hitsBullet, PhysicsMaterialMultiply[] damageMultiply, MethodHitsBullets onHitBullet)
	{
	}

	public void StartBullets()
	{
	}

	public void SetActiveRenderSpare(bool value)
	{
		if (renderInSpare != null)
		{
			renderInSpare.gameObject.SetActive(value);
		}
	}

	public void DestroyWeapon()
	{
		if (renderInSpare != null)
		{
			UnityEngine.Object.Destroy(renderInSpare.gameObject);
		}
		UnityEngine.Object.Destroy(base.gameObject);
	}
}
