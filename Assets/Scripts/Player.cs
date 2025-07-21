using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MonoBehaviour, IScreenApperancePlayExit, IEventSystemHandler
{
	public const int NONE = -1;

	public static int money = 0;

	public static string idLastWeapon = string.Empty;

	public static string idLastWeaponMain = string.Empty;

	public static string idLastWeaponBackup = string.Empty;

	public static int magazinMain = -1;

	public static int magazinBackup = -1;

	public static int currentHealth = 0;

	public static Vector3 positionDeath = default(Vector3);

	public const string DEATH = "смерть_игрока";

	public static Player onStage = null;

	[HideInInspector]
	public float _scores;

	public WeaponHands[] weapons = new WeaponHands[0];

	public WeaponAxe[] rotateBodyShot = new WeaponAxe[0];

	[HideInInspector]
	public WeaponHands currentWeapon;

	public WeaponHands mainWeapon;

	public WeaponHands backupWeapon;

	public bool clearOtherWeapon = true;

	public Transform head;

	private float startAngleHead;

	[Tooltip("Тело для отдачи от оружия")]
	public Transform body;

	public int health = 10;

	public int recoveryHealth;

	private int startHealth;

	private float speedHealth;

	private float timeRecoveryHealth = 2f;

	public PlayerMove mover;

	[Space(6f)]
	public AudioClip soundDamage;

	public AudioClip soundDeath;

	[Space(6f)]
	public Text textScores;

	public Text textMoney;

	public RectTransform displayHealth;

	[Space(6f)]
	public Canvas canvas;

	public AudioClip soundEathDeath;

	public AreaDamage areaEathDeath;

	[Space(6f)]
	public TouchButton buttonAimShot;

	public Button buttonMainWeapon;

	public Button buttonAddonWeapon;

	public Button buttonReload;

	public Button buttonUse;

	public string sceneReplay;

	public string sceneGameOver;

	[Space(10f)]
	public UnityEvent death = new UnityEvent();

	[HideInInspector]
	public int distanceMove = 15;

	[HideInInspector]
	public AudioSource sound;

	[HideInInspector]
	public bool isDead;

	private Animator animator;

	private float angleMouse = 8f;

	private Vector3 targetPoint = new Vector3(0.98f, 0.17f, 0f);

	private float distToAim;

	private float angleStrikeAxe;

	private float angleDistStrike;

	private float timerSwitchWeapon;

	private float timeScaleSwitch;

	private float startAngleSwitch;

	private float targetAngleSwitch;

	public float moneyHits;

	private WeaponBulletLine bulletLine;

	private int oldHealth = -123;

	private bool triggerShotWeapon;

	private Vector3 positionCamera;

	private float recoil;

	private int weaponSwitch;

	private float lastScale = 1f;

	[HideInInspector]
	public float scores
	{
		get
		{
			return _scores / 846f;
		}
		set
		{
			_scores = value * 846f;
		}
	}

	public static void ResetWeapon()
	{
		idLastWeapon = string.Empty;
		idLastWeaponMain = string.Empty;
		idLastWeaponBackup = string.Empty;
		magazinMain = -1;
		magazinBackup = -1;
	}

	private void Awake()
	{
		onStage = this;
		sound = base.gameObject.GetComponent<AudioSource>();
		animator = GetComponent<Animator>();
		weapons = base.gameObject.GetComponentsInChildren<WeaponHands>(includeInactive: true);
		if (StoreWeapons.mainWeapon != string.Empty)
		{
			for (int i = 0; i < weapons.Length; i++)
			{
				if (StoreWeapons.mainWeapon == weapons[i].ID)
				{
					mainWeapon = weapons[i];
				}
				else if (StoreWeapons.backupWeapon == weapons[i].ID)
				{
					backupWeapon = weapons[i];
				}
			}
		}
		if (idLastWeaponMain == mainWeapon.ID)
		{
			mainWeapon.magazine = magazinMain;
			magazinMain = mainWeapon.magazine;
		}
		if (backupWeapon != null && idLastWeaponBackup == backupWeapon.ID)
		{
			backupWeapon.magazine = magazinBackup;
			magazinBackup = backupWeapon.magazine;
		}
		for (int j = 0; j < weapons.Length; j++)
		{
			if (!clearOtherWeapon)
			{
				break;
			}
			if (weapons[j] != mainWeapon && weapons[j] != backupWeapon)
			{
				weapons[j].DestroyWeapon();
				weapons[j] = null;
			}
		}
	}

	private void Start()
	{
		if (backupWeapon != null && idLastWeapon != string.Empty && idLastWeapon == idLastWeaponBackup)
		{
			TakeAdditWeapon();
		}
		else
		{
			TakeMainWeapon();
		}
		Vector3 eulerAngles = head.rotation.eulerAngles;
		startAngleHead = eulerAngles.z;
		startHealth = health;
		if (currentHealth > 0)
		{
			health = currentHealth;
		}
		UpdateIndicatorHealth();
		UnityEngine.Debug.Log("SpriteWithDamage.sprites.Length:" + SpriteWithDamage.sprites.Length);
		buttonAimShot.pointerDown.AddListener(AimToPoint);
		buttonAimShot.pointerDown.AddListener(delegate
		{
			TriggerShot(keyDown: true);
		});
		buttonAimShot.drag.AddListener(AimToPoint);
		buttonAimShot.pointerUp.AddListener(delegate
		{
			TriggerShot();
		});
		buttonMainWeapon.onClick.AddListener(SwitchWeapon);
		buttonAddonWeapon.onClick.AddListener(SwitchWeapon);
		buttonReload.onClick.AddListener(delegate
		{
			ReloadWeapon();
		});
	}

	public void LoadScene(string sceneName)
	{
		SceneManager.LoadScene(sceneName);
	}

	public void DeathZombie(GameObject fromGameObject)
	{
	}

	public void StrikeZombie()
	{
		AxeStrike(15f);
		health -= startHealth / 6;
		UpdateIndicatorHealth();
		speedHealth = health;
		sound.PlayOneShot(soundDamage);
		if (health == 0)
		{
			Death();
		}
	}

	private void UpdateIndicatorHealth()
	{
		health = Mathf.Clamp(health, 0, startHealth);
		if (oldHealth != health)
		{
			float num = health;
			num /= (float)startHealth;
			UpdateIndicatorHealth(num);
			oldHealth = health;
		}
	}

	private void UpdateIndicatorHealth(float percent)
	{
		percent = Mathf.Ceil(percent * 1000f) / 1000f;
		percent = Mathf.Clamp01(percent);
		if (percent == 1f)
		{
			displayHealth.parent.gameObject.SetActive(value: false);
			return;
		}
		displayHealth.parent.gameObject.SetActive(value: true);
		displayHealth.localScale = new Vector3(percent, 1f, 1f);
		Image component = displayHealth.GetComponent<Image>();
		Color red = Color.red;
		Color yellow = Color.yellow;
		Vector3 localScale = displayHealth.localScale;
		component.color = Color.Lerp(red, yellow, localScale.x);
	}

	private void Update()
	{
		if (!isDead && recoveryHealth > 0 && displayHealth != null && speedHealth != 0f)
		{
			speedHealth += Time.deltaTime / timeRecoveryHealth;
			health = Mathf.FloorToInt(speedHealth);
			UpdateIndicatorHealth();
			if (health >= recoveryHealth)
			{
				speedHealth = 0f;
			}
		}
		if (!isDead && timeScaleSwitch != 0f)
		{
			timerSwitchWeapon += Time.deltaTime / Time.timeScale / timeScaleSwitch;
			if (timerSwitchWeapon >= 1f)
			{
				timerSwitchWeapon = 0f;
				timeScaleSwitch = 0f;
				WeaponEnable(value: true);
				angleMouse = targetAngleSwitch;
			}
			else
			{
				currentWeapon.transform.localRotation = Quaternion.Euler(0f, 0f, Mathf.LerpAngle(startAngleSwitch, targetAngleSwitch, timerSwitchWeapon));
			}
		}
	}

	private void LateUpdate()
	{
		if (isDead)
		{
			return;
		}
		recoil = currentWeapon.GetRecoil(Time.deltaTime);
		if (!currentWeapon.stateReload && buttonAimShot.gameObject.activeInHierarchy)
		{
			currentWeapon.transform.localRotation = Quaternion.Euler(0f, 0f, angleMouse);
			head.localRotation = Quaternion.Euler(0f, 0f, angleMouse / 2f + startAngleHead);
			if (recoil != 0f)
			{
				currentWeapon.transform.Rotate(0f, 0f, recoil);
				head.Rotate(0f, 0f, -5f * (recoil / currentWeapon.recoilMax));
			}
		}
		else if (currentWeapon.stateReload && (currentWeapon.animator.currentAnimation.name == currentWeapon.completeReloadAnim || currentWeapon.animator.currentAnimation.name == currentWeapon.boltAnim))
		{
			float num = currentWeapon.animator.currentFrame;
			num /= (float)currentWeapon.animator.totalFrames;
			num = Mathf.Clamp01(num - 0.75f) * 4f;
			float num2 = Mathf.Lerp(0f, angleMouse, num);
			currentWeapon.transform.localRotation = Quaternion.Euler(0f, 0f, num2);
			head.localRotation = Quaternion.Euler(0f, 0f, num2 / 2f + startAngleHead);
		}
		else if (currentWeapon.stateReload && currentWeapon.animator.currentFrame * 2 < currentWeapon.animator.totalFrames && currentWeapon.animator.currentAnimation.name == currentWeapon.addBulletAnim)
		{
			float num3 = (float)currentWeapon.animator.currentFrame / (float)currentWeapon.animator.totalFrames;
			float num4 = Mathf.Lerp(angleMouse, 0f, 10f * num3);
			currentWeapon.transform.localRotation = Quaternion.Euler(0f, 0f, num4);
			head.localRotation = Quaternion.Euler(0f, 0f, num4 / 2f + startAngleHead);
		}
		else if (currentWeapon.stateReload && string.IsNullOrEmpty(currentWeapon.boltAnim) && currentWeapon.animator.currentAnimation.name == currentWeapon.addBulletAnim)
		{
			float num5 = (float)currentWeapon.animator.currentFrame / (float)currentWeapon.animator.totalFrames;
			num5 = 1f - num5;
			float num6 = Mathf.Lerp(angleMouse, 0f, 10f * num5);
			currentWeapon.transform.localRotation = Quaternion.Euler(0f, 0f, num6);
			head.localRotation = Quaternion.Euler(0f, 0f, num6 / 2f + startAngleHead);
		}
		if (angleDistStrike != 0f)
		{
			body.localRotation = Quaternion.Euler(0f, 0f, 0f - Mathf.PingPong(2f * angleStrikeAxe, angleDistStrike));
			angleStrikeAxe += angleDistStrike * Time.deltaTime / 0.4f;
			if (angleStrikeAxe >= angleDistStrike)
			{
				angleDistStrike = 0f;
				angleStrikeAxe = 0f;
			}
		}
		else if (recoil != 0f)
		{
			body.localRotation = Quaternion.Euler(0f, 0f, 5f * (recoil / currentWeapon.recoilMax));
		}
		if (currentWeapon.IsReady(triggerShotWeapon))
		{
			currentWeapon.Shot(distToAim, OnHitBullet);
			AxeStrike();
		}
	}

	public void Death()
	{
		if (!isDead)
		{
			isDead = true;
			currentHealth = 0;
			ResetWeapon();
			positionDeath = base.transform.position;
			mover.enabled = false;
			TriggerShot();
			WeaponEnable(value: false);
			currentWeapon.enabled = false;
			currentWeapon.animator.GotoAndStop(1);
			sound.PlayOneShot(soundDeath);
			death.Invoke();
			UnityEngine.Object.Destroy(canvas.gameObject);
			AimTarget(0f);
			head.localRotation = default(Quaternion);
			animator.Play("Death");
			base.enabled = true;
			StopAllCoroutines();
			SettingsStartGame.OnDeathPlayer();
		}
	}

	public void ZombieBeginEath()
	{
		if (areaEathDeath.health != 0f)
		{
			areaEathDeath.Activation();
			sound.Stop();
			sound.clip = soundEathDeath;
			sound.Play();
		}
	}

	public void SceneGameOver()
	{
	}

	public void Quit()
	{
		Application.Quit();
	}

	private void OnDestroy()
	{
		if (!isDead)
		{
			idLastWeapon = currentWeapon.ID;
			idLastWeaponMain = mainWeapon.ID;
			magazinMain = mainWeapon.magazine;
			currentHealth = Mathf.Max(health, recoveryHealth);
			if (backupWeapon != null)
			{
				idLastWeaponBackup = backupWeapon.ID;
				magazinBackup = backupWeapon.magazine;
			}
		}
	}

	public void WeaponEnable(bool value)
	{
		currentWeapon.enabled = (value || currentWeapon.stateReload);
		buttonAimShot.gameObject.SetActive(value);
	}

	public void OnHitBullet(HitBullet hitBullet)
	{
	}

	public void AxeStrike()
	{
		int num = 0;
		while (true)
		{
			if (num < rotateBodyShot.Length)
			{
				if (currentWeapon.ID == rotateBodyShot[num].idWeapon)
				{
					break;
				}
				num++;
				continue;
			}
			return;
		}
		if (currentWeapon.animator.currentAnimation.frames.Length >= 3)
		{
			AxeStrike(rotateBodyShot[num].angleBody);
			mover.BlockStrikeLeg(currentWeapon.animator.currentAnimation.frames.Length, currentWeapon.animator.speed);
		}
	}

	public void AxeStrike(float angleBody)
	{
		angleStrikeAxe = 0f;
		angleDistStrike = angleBody;
	}

	public void OnBeginStrikeLeg()
	{
		TriggerShot();
		WeaponEnable(value: false);
	}

	public void OnStrikeLegComplete()
	{
		TriggerShot();
		WeaponEnable(value: true);
	}

	public void AimToPoint(PointerEventData eventData)
	{
		if (eventData == null)
		{
			targetPoint = new Vector3(1f, -1f, 45f);
		}
		else if (base.enabled && currentWeapon != null)
		{
			targetPoint = Camera.main.ScreenToWorldPoint(eventData.position);
			targetPoint = currentWeapon.transform.parent.InverseTransformPoint(targetPoint);
			ref Vector3 reference = ref targetPoint;
			float x = targetPoint.x;
			Vector3 localPosition = currentWeapon.transform.localPosition;
			reference.x = x - localPosition.x;
			ref Vector3 reference2 = ref targetPoint;
			float y = targetPoint.y;
			Vector3 localPosition2 = currentWeapon.transform.localPosition;
			reference2.y = y - (localPosition2.y + currentWeapon.boltY);
			targetPoint.z = 0f;
			distToAim = targetPoint.magnitude;
			targetPoint.z = 57.29578f * Mathf.Atan2(targetPoint.y, targetPoint.x);
			targetPoint.z = Mathf.Clamp(targetPoint.z, -45f, 45f);
		}
		angleMouse = targetPoint.z;
	}

	public void AimTarget(float angle)
	{
		currentWeapon.transform.localRotation = Quaternion.Euler(0f, 0f, angle);
		head.transform.localRotation = Quaternion.Euler(0f, 0f, angle / 2f);
		targetPoint.x = Mathf.Cos(angle * ((float)Math.PI / 180f));
		targetPoint.y = Mathf.Sin(angle * ((float)Math.PI / 180f));
		targetPoint.z = 0f;
		targetPoint = currentWeapon.transform.TransformPoint(targetPoint);
		targetPoint.z = angle;
		angleMouse = angle;
	}

	public float AddScoresHits(GameObject objectHit, float scaleScoresBullet, float maxScores, float scale)
	{
		float num = 0f;
		if (maxScores == 0f)
		{
			return 0f;
		}
		if (scaleScoresBullet < 1f)
		{
			scaleScoresBullet *= 2f;
		}
		if (objectHit.CompareTag("Zombie"))
		{
			if (objectHit.name == "Голова")
			{
				moneyHits += 10f * scaleScoresBullet;
				num = 5f * scaleScoresBullet;
			}
			else if (objectHit.name == "Тело")
			{
				moneyHits += 5f * scaleScoresBullet;
				num = 2.5f * scaleScoresBullet;
			}
			else
			{
				moneyHits += 2f * scaleScoresBullet;
				num = 1f * scaleScoresBullet;
			}
			if (num != 0f)
			{
				num *= scale;
				scores += num;
				if (textMoney != null)
				{
					textMoney.text = "$" + Mathf.FloorToInt(moneyHits).ToString();
				}
				if (textScores != null)
				{
					textScores.text = Mathf.FloorToInt(scores).ToString();
				}
			}
		}
		return Mathf.Max(0f, maxScores - num);
	}

	public void TriggerShot(bool keyDown = false)
	{
		currentWeapon.OnKeyShot(keyDown);
		triggerShotWeapon = (keyDown && currentWeapon.magazine != 0 && !currentWeapon.stateReload);
	}

	public void TriggerShotTrue()
	{
		TriggerShot(keyDown: true);
	}

	public void TriggerShotFalse()
	{
		TriggerShot();
	}

	public void ReloadWeapon()
	{
		currentWeapon.StartReload();
	}

	public void SelectWeapon(WeaponHands weapon)
	{
		weapons = base.gameObject.GetComponentsInChildren<WeaponHands>(includeInactive: true);
		for (int i = 0; i < weapons.Length; i++)
		{
			if (weapon.ID != weapons[i].ID)
			{
				weapons[i].gameObject.SetActive(value: false);
				if (weapons[i] == mainWeapon || weapons[i] == backupWeapon)
				{
					weapons[i].SetActiveRenderSpare(value: true);
				}
				else
				{
					weapons[i].SetActiveRenderSpare(value: false);
				}
			}
		}
		currentWeapon = weapon;
		currentWeapon.gameObject.SetActive(value: true);
		currentWeapon.SetActiveRenderSpare(value: false);
		if (backupWeapon != null)
		{
			buttonMainWeapon.gameObject.SetActive(mainWeapon.ID != weapon.ID);
			buttonAddonWeapon.gameObject.SetActive(backupWeapon.ID != weapon.ID);
		}
		else
		{
			buttonMainWeapon.gameObject.SetActive(value: false);
			buttonAddonWeapon.gameObject.SetActive(value: false);
		}
		if (weaponSwitch != 0)
		{
			EffectSelectWeapon();
		}
		weaponSwitch = currentWeapon.GetInstanceID();
	}

	public void SelectWeapon(string id)
	{
		int num = 0;
		while (true)
		{
			if (num < weapons.Length)
			{
				if (weapons[num].ID == id)
				{
					break;
				}
				num++;
				continue;
			}
			return;
		}
		SelectWeapon(weapons[num]);
	}

	public void SelectWeapon(string idMain, string idBackup)
	{
		mainWeapon = null;
		backupWeapon = null;
		for (int i = 0; i < weapons.Length; i++)
		{
			if (weapons[i].ID == idMain)
			{
				mainWeapon = weapons[i];
			}
			else if (weapons[i].ID == idBackup)
			{
				backupWeapon = weapons[i];
			}
		}
		SelectWeapon(mainWeapon);
	}

	public void EffectSelectWeapon()
	{
		if (currentWeapon.stateReload)
		{
			WeaponEnable(value: false);
			targetAngleSwitch = 0f;
			startAngleSwitch = -60f;
			timeScaleSwitch = Mathf.Abs((startAngleSwitch - targetAngleSwitch) / 360f);
			timerSwitchWeapon = 0f;
		}
		else
		{
			WeaponEnable(value: false);
			targetAngleSwitch = angleMouse;
			startAngleSwitch = -80f;
			timeScaleSwitch = Mathf.Abs((startAngleSwitch - targetAngleSwitch) / 360f);
			timerSwitchWeapon = 0f;
		}
	}

	public void SwitchWeapon()
	{
		if (currentWeapon != null && currentWeapon.ID == mainWeapon.ID)
		{
			SelectWeapon(backupWeapon);
		}
		else if (currentWeapon != null && currentWeapon.ID == backupWeapon.ID)
		{
			SelectWeapon(mainWeapon);
		}
		else
		{
			SelectWeapon(mainWeapon);
		}
	}

	public void TakeMainWeapon()
	{
		SelectWeapon(mainWeapon);
	}

	public void TakeAdditWeapon()
	{
		if (backupWeapon != null)
		{
			SelectWeapon(backupWeapon);
		}
	}

	public void PlaySound(AudioClip clip)
	{
		sound.PlayOneShot(clip);
	}

	public void OnActiveArea(InteractionArea area)
	{
		mover.buttonStrikeLeg.gameObject.SetActive(value: false);
	}

	public void OnInactiveArea(InteractionArea area)
	{
		mover.buttonStrikeLeg.gameObject.SetActive(value: true);
	}

	public void OnStartMove()
	{
		float num = lastScale;
		Vector3 localScale = base.transform.localScale;
		if (num != localScale.x)
		{
			AimTarget(10f);
			EffectSelectWeapon();
			TriggerShot();
			Vector3 localScale2 = base.transform.localScale;
			lastScale = localScale2.x;
		}
	}

	public void OnScreenPlayExit(string nameObject)
	{
		MonoBehaviour[] components = base.gameObject.GetComponents<MonoBehaviour>();
		for (int i = 0; i < components.Length; i++)
		{
			components[i].enabled = false;
		}
		components = null;
	}
}
