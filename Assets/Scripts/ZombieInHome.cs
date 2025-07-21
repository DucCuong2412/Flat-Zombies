using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
public class ZombieInHome : Entity
{
	public enum ModeMove
	{
		Default,
		StandBreakLeg
	}

	public const float distStrikePlayer = 3.5f;

	public const float minDeltaTime = 0.05f;

	public const float minDist = 0.3f;

	public static ZombieInHome[] ragdolls = new ZombieInHome[6];

	public static ZombieInHome[] entities = new ZombieInHome[0];

	public static string lastPlaySound;

	private static ZombieInHome zombie = null;

	public static SortingGroup[] listSortGroup = new SortingGroup[0];

	public PhysicsMaterial2D materialBody;

	[Range(0f, 2f)]
	public float scaleSpeedMove;

	public float speedMove;

	public Color colorBody = new Color(1f, 1f, 1f);

	public Color colorLegs = new Color(1f, 1f, 1f);

	public SpriteRenderer[] spritesOfBody = new SpriteRenderer[0];

	public SpriteRenderer[] spritesOfLegs = new SpriteRenderer[0];

	public Material materialHolesSkin;

	[Space(10f)]
	public float floor;

	[HideInInspector]
	public Vector3 slideRagdoll = default(Vector3);

	public Rigidbody2D[] rigidbodies = new Rigidbody2D[0];

	public SpriteRenderer[] spritesRigidbodies = new SpriteRenderer[0];

	public Joint2D[] joints = new Joint2D[0];

	[Space(10f)]
	[Tooltip("Туловище для эффекта попаданий и для работы с физикой")]
	public Transform body;

	[Tooltip("Время для отключения физ.тел, используемые для пересечения с пулей")]
	public float timerBullet = 5f;

	public AudioClip[] soundsRage = new AudioClip[0];

	public AudioClip[] soundsPain = new AudioClip[0];

	public AudioClip[] soundsDie = new AudioClip[0];

	public AudioClip[] soundsSliceJoints = new AudioClip[0];

	public PartBodyDamage[] coefficientDamageBody = new PartBodyDamage[0];

	public ColliderTriggerAnimation[] triggersAnimations = new ColliderTriggerAnimation[0];

	[Tooltip("Триггеры. При подсчете очков не учитывать эти триггеры\n, когда у зомби сломаны ноги")]
	public Collider2D[] triggersBreakLegs = new Collider2D[0];

	[Space(5f)]
	public ZombieCharacteristic character;

	public float damageStrike = 1f;

	public Vector3 currentSpeed = default(Vector3);

	private Animator animator;

	private AnimatorStateInfo stateAnimator = default(AnimatorStateInfo);

	private AudioSource sound;

	[HideInInspector]
	public SortingGroup sorter;

	private Vector3 startPositionBody = default(Vector3);

	private float angleBodyBullet = -5f;

	private float angleBodyTimer;

	private Collider2D[] colliders;

	private SpriteRenderer[] sprites;

	private ModeMove modeMove;

	private Vector2 distPlayer = new Vector2(100f, 100f);

	private Vector2 distBarricade = new Vector2(100f, 100f);

	private float angleToPlayer;

	private bool strikeOnRun = true;

	private int numOfSrike;

	private int r;

	private Vector2 force;

	private Collider2D[] colldersHit;

	private SpriteRenderer[] spritiesHit;

	private float maxScores = 40f;

	private ZombieInHome sosed;

	private HingeJoint2D hingle;

	private SpliceBody spliceBodyArea;

	public static Rigidbody2D[] tempListBody = new Rigidbody2D[0];

	public static SpriteRenderer[] tempListRender = new SpriteRenderer[0];

	private bool rageIsPlayed;

	private Joint2D[] listDestroy;

	private float timeMarkStrikePlayer;

	public static int maxRagdolls
	{
		get
		{
			return ragdolls.Length;
		}
		set
		{
			ragdolls = new ZombieInHome[value];
		}
	}

	public static bool PlayerStrike(Transform player, int numHits, float timeStrike, string methodOnHit)
	{
		int num = numHits;
		int num2 = 0;
		int num3 = -1;
		for (num2 = 0; num2 < entities.Length; num2++)
		{
			if (numHits <= 0)
			{
				break;
			}
			zombie = entities[num2];
			if (!zombie.StrikePlayerIsActive() || !zombie.animator.GetCurrentAnimatorStateInfo(0).IsTag("attack"))
			{
				continue;
			}
			Vector3 position = player.position;
			float x = position.x;
			Vector3 position2 = zombie.body.position;
			if (Mathf.Abs(x - position2.x) <= 3.5f)
			{
				if (zombie.modeMove == ModeMove.Default)
				{
					zombie.animator.ResetTrigger("pain");
					zombie.animator.SetTrigger("pain");
					zombie.animator.Play("ПолучитьУдарПрисесть", 0, Random.value * 0.1f);
				}
				else if (zombie.modeMove == ModeMove.StandBreakLeg)
				{
					zombie.animator.Play("УпалПолучитьУдар", 0, Random.value * 0.05f);
				}
				zombie.StrikePlayerDisable(timeStrike);
				player.gameObject.SendMessage(methodOnHit);
				numHits--;
				if (numHits == 0 && num3 != num2 && num3 != -1 && Vector3.Distance(zombie.transform.position, entities[num3].transform.position) < 0.3f)
				{
					zombie.transform.position = entities[num3].transform.position;
					zombie.transform.position += new Vector3(0f, -0.3f, 0f);
					UpdateSortingOrder();
				}
				num3 = num2;
			}
		}
		for (num2 = 0; num2 < entities.Length; num2++)
		{
			if (numHits <= 0)
			{
				break;
			}
			zombie = entities[num2];
			if (!zombie.StrikePlayerIsActive())
			{
				continue;
			}
			Vector3 position3 = player.position;
			float x2 = position3.x;
			Vector3 position4 = zombie.body.position;
			if (Mathf.Abs(x2 - position4.x) <= 3.5f)
			{
				if (zombie.modeMove == ModeMove.Default && (!zombie.IsStand() || Random.Range(0, 4) == 1))
				{
					zombie.animator.ResetTrigger("pain");
					zombie.animator.SetTrigger("pain");
					zombie.animator.Play("ПолучитьУдарПрисесть", 0, Random.value * 0.1f);
				}
				else if (zombie.modeMove == ModeMove.Default)
				{
					zombie.animator.Play("ПолучитьУдар", 0, Random.value * 0.1f);
				}
				else if (zombie.modeMove == ModeMove.StandBreakLeg)
				{
					zombie.animator.Play("УпалПолучитьУдар", 0, Random.value * 0.05f);
				}
				zombie.StrikePlayerDisable(timeStrike);
				player.gameObject.SendMessage(methodOnHit);
				numHits--;
				if (numHits == 0 && num3 != num2 && num3 != -1 && Vector3.Distance(zombie.transform.position, entities[num3].transform.position) < 0.3f)
				{
					zombie.transform.position = entities[num3].transform.position;
					zombie.transform.position += new Vector3(0f, -0.3f, 0f);
					UpdateSortingOrder();
				}
				num3 = num2;
			}
		}
		Rigidbody2D rigidbody2D = null;
		int num4 = 0;
		while (num >= 0 && num4 < ragdolls.Length && ragdolls[num4] != null)
		{
			Vector3 position5 = player.position;
			float x3 = position5.x;
			Vector3 position6 = ragdolls[num4].body.position;
			if (Mathf.Abs(x3 - position6.x) <= 3.5f)
			{
				for (int i = 0; i < ragdolls[num4].rigidbodies.Length; i++)
				{
					if (rigidbody2D == null || rigidbody2D.mass < ragdolls[num4].rigidbodies[i].mass)
					{
						rigidbody2D = ragdolls[num4].rigidbodies[i];
					}
				}
				if (rigidbody2D != null)
				{
					Vector3 eulerAngles = rigidbody2D.transform.rotation.eulerAngles;
					if (Mathf.Abs(Mathf.DeltaAngle(eulerAngles.z, 0f)) <= 45f)
					{
						Vector3 position7 = player.position;
						float x4 = position7.x;
						Vector2 worldCenterOfMass = rigidbody2D.worldCenterOfMass;
						if (Mathf.Abs(x4 - worldCenterOfMass.x) <= 3.5f)
						{
							ragdolls[num4].DisablePhysicBody(5f);
							rigidbody2D.AddForce(new Vector2(5f, 3f), ForceMode2D.Impulse);
							num--;
							player.gameObject.SendMessage(methodOnHit);
							rigidbody2D = null;
						}
					}
				}
			}
			num4++;
		}
		rigidbody2D = null;
		zombie = null;
		return numHits == 0;
	}

	public void ShowMeat(Transform body)
	{
		int num = 0;
		while (true)
		{
			if (num < sprites.Length)
			{
				if (!sprites[num].gameObject.activeSelf && sprites[num].transform.parent == body)
				{
					break;
				}
				num++;
				continue;
			}
			return;
		}
		sprites[num].gameObject.SetActive(value: true);
	}

	public static void UpdateSortingOrder()
	{
		listSortGroup = Object.FindObjectsOfType<SortingGroup>();
		for (int i = 0; i < listSortGroup.Length; i++)
		{
			listSortGroup[i].sortingOrder = 1;
			for (int j = 0; j < listSortGroup.Length; j++)
			{
				Vector3 position = listSortGroup[i].transform.position;
				float y = position.y;
				Vector3 position2 = listSortGroup[j].transform.position;
				if (y < position2.y)
				{
					listSortGroup[i].sortingOrder++;
				}
			}
		}
	}

	private void OnDrawGizmosSelected()
	{
		if (character != null)
		{
			character.OnDrawGizmosZombie(this);
			if (character.colorsBody.Length != 0)
			{
				colorBody = character.colorsBody[0];
			}
			if (character.colorsLegs.Length != 0)
			{
				colorLegs = character.colorsLegs[0];
			}
		}
		if (base.enabled && spritesOfBody.Length != 0 && spritesOfLegs.Length != 0 && (spritesOfLegs[0].color != colorLegs || spritesOfBody[0].color != colorBody))
		{
			for (int i = 0; i < spritesOfBody.Length; i++)
			{
				spritesOfBody[i].color = colorBody;
			}
			for (int j = 0; j < spritesOfLegs.Length; j++)
			{
				spritesOfLegs[j].color = colorLegs;
			}
		}
		Gizmos.color = new Color(1f, 0.5f, 0f);
		Vector3 position = base.gameObject.transform.position;
		Vector3 position2 = base.gameObject.transform.position;
		position.x = position2.x - 1.5f;
		position.y = floor;
		Gizmos.DrawLine(position, new Vector3(position.x + 3f, position.y, position.z));
	}

	private void Awake()
	{
		entities = Object.FindObjectsOfType<ZombieInHome>();
		if (DamageOfSprite.enabledComponents && materialHolesSkin != null)
		{
			SpriteHoles.TestMaterial(materialHolesSkin);
		}
		animator = GetComponent<Animator>();
		animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
		sound = GetComponent<AudioSource>();
		joints = GetComponentsInChildren<Joint2D>();
		sprites = GetComponentsInChildren<SpriteRenderer>(includeInactive: true);
		sorter = GetComponent<SortingGroup>();
		startPositionBody = body.localPosition;
		DisablePhysicBody(simulated: false);
	}

	private void Start()
	{
		for (int i = 0; i < spritesOfBody.Length; i++)
		{
			spritesOfBody[i].color = colorBody;
		}
		for (int i = 0; i < spritesOfLegs.Length; i++)
		{
			spritesOfLegs[i].color = colorLegs;
		}
		if (character != null)
		{
			character.ChangeZombie(this);
		}
		SetScaleSpeed(scaleSpeedMove);
		if (DamageOfSprite.enabledComponents && spritesRigidbodies.Length != 0)
		{
			Vector2[] array = new Vector2[2];
			int[] array2 = new int[array.Length];
			Collider2D component;
			for (int i = 0; i < array.Length; i++)
			{
				array2[i] = Random.Range(0, spritesRigidbodies.Length);
				component = spritesRigidbodies[array2[i]].GetComponent<Collider2D>();
				if (component != null && component.sharedMaterial == materialBody)
				{
					ref Vector2 reference = ref array[i];
					Vector3 min = component.bounds.min;
					float min2 = min.x * 100f;
					Vector3 max = component.bounds.max;
					reference.x = Random.Range(min2, max.x * 100f) / 100f;
					ref Vector2 reference2 = ref array[i];
					Vector3 min3 = component.bounds.min;
					float min4 = min3.y * 100f;
					Vector3 max2 = component.bounds.max;
					reference2.y = Random.Range(min4, max2.y * 100f) / 100f;
				}
				for (int j = 0; j < array2.Length; j++)
				{
					if (j != i && array2[i] == array2[j])
					{
						array[i] = Vector2.zero;
					}
				}
			}
			for (int k = 0; k < array.Length; k++)
			{
				if (array[k].x == 0f && array[k].y == 0f)
				{
					continue;
				}
				SpriteHoles.AddHole(spritesRigidbodies[array2[k]], array[k], materialHolesSkin);
				ShowMeat(spritesRigidbodies[array2[k]].transform);
				colliders = Physics2D.OverlapCircleAll(array[k], (float)SpriteHoles.GetTextureHole(materialHolesSkin).width / spritesRigidbodies[0].sprite.pixelsPerUnit);
				for (int l = 0; l < colliders.Length; l++)
				{
					for (int i = 0; i < spritesRigidbodies.Length; i++)
					{
						if (spritesRigidbodies[i].gameObject == colliders[l].gameObject && (spritesRigidbodies[i].transform.parent == spritesRigidbodies[array2[k]].transform || spritesRigidbodies[array2[k]].transform.parent == spritesRigidbodies[i].transform))
						{
							SpriteHoles.AddHole(spritesRigidbodies[i], array[k], materialHolesSkin);
							ShowMeat(spritesRigidbodies[i].transform);
							break;
						}
					}
				}
			}
			component = null;
		}
		if (coefficientDamageBody.Length == 0 || health == 0f)
		{
			Death();
		}
		for (int m = 0; m < coefficientDamageBody.Length; m++)
		{
			coefficientDamageBody[m].coefficient = Mathf.Floor(health / coefficientDamageBody[m].health * 100f) / 100f;
		}
		for (int n = 0; n < base.areas.Length; n++)
		{
			if (base.areas[n].ComponentInList<SpliceBody>())
			{
				base.areas[n].AddListener(UpdateRigidbody);
			}
		}
		UpdateSortingOrder();
		WeaponCartridge.AddEntityTriggers(this);
	}

	private void OnDestroy()
	{
		WeaponCartridge.RemoveEntityTriggers(this);
		lastPlaySound = string.Empty;
		base.enabled = false;
		entities = Object.FindObjectsOfType<ZombieInHome>();
		if (isDead)
		{
		}
	}

	public void SetScaleSpeed(float scaleSpeed)
	{
		scaleSpeedMove = scaleSpeed;
		if (animator != null)
		{
			animator.SetFloat("speedMove", scaleSpeedMove);
		}
	}

	public void AddScaleSpeed()
	{
		SetScaleSpeed(Mathf.Lerp(scaleSpeedMove, 1.6f, Random.Range(50f, 100f) * 0.01f));
	}

	public void Stand()
	{
		animator.SetBool("move", value: false);
	}

	public void SwitchMoveLame()
	{
		animator.SetBool("lame", value: true);
	}

	public void SwitchMoveBreakLeg()
	{
		animator.SetBool("breakLeg", value: true);
	}

	public void SwitchStandBreakLeg()
	{
		if (modeMove != ModeMove.StandBreakLeg && !isDead)
		{
			animator.Play("УпастьНаМесте");
			modeMove = ModeMove.StandBreakLeg;
		}
	}

	public void SwitchMoveCrawling()
	{
	}

	private void Update()
	{
		stateAnimator = animator.GetCurrentAnimatorStateInfo(0);
		if (!isDead && speedMove != 0f)
		{
			scaleSpeedMove = stateAnimator.speed * stateAnimator.speedMultiplier;
			currentSpeed.x = speedMove * scaleSpeedMove * Mathf.Min(0.05f, Time.deltaTime);
			currentSpeed.y = 0f;
			if (modeMove == ModeMove.Default && distPlayer.x < 12f)
			{
				angleToPlayer = Mathf.Atan2(distPlayer.y, distPlayer.x);
				currentSpeed.y = Mathf.Sin(angleToPlayer) * Mathf.Abs(currentSpeed.x);
			}
			base.transform.position += currentSpeed;
		}
		if (!isDead && ZombieBarricade.onStage != null)
		{
			distBarricade = base.transform.position - ZombieBarricade.onStage.transform.position;
			distBarricade.x = Mathf.Floor(distBarricade.x * 100f) / 100f;
			if (distBarricade.x < 0f)
			{
				base.transform.position = ZombieBarricade.onStage.transform.position;
				base.transform.Translate(0f, distBarricade.y, 0f);
				distBarricade.x = 0f;
			}
			if (modeMove == ModeMove.Default && distBarricade.x == 0f && (stateAnimator.IsTag("move") || stateAnimator.normalizedTime >= 1f))
			{
				animator.Play("Удар", 0, 0f);
			}
			else if (modeMove == ModeMove.StandBreakLeg && distBarricade.x == 0f && (stateAnimator.IsTag("stand") || stateAnimator.normalizedTime >= 1f))
			{
				animator.Play("УпалУдар", 0, 0f);
			}
		}
		else
		{
			if (isDead || !(Player.onStage != null) || Player.onStage.isDead)
			{
				return;
			}
			distPlayer = Player.onStage.transform.position - base.transform.position;
			distPlayer.x = Mathf.Abs(distPlayer.x);
			distPlayer.x = Mathf.Floor(distPlayer.x * 100f) / 100f;
			if (modeMove == ModeMove.Default)
			{
				if (distPlayer.x < 3f && stateAnimator.IsName("Бег") && strikeOnRun)
				{
					animator.Play("УдарРазбег", 0, 0f);
					numOfSrike++;
					strikeOnRun = false;
				}
				else if (distPlayer.x < 2.5f && stateAnimator.IsName("ИдтиБыстро"))
				{
					animator.Play("УдарРазбег", 0, 0f);
				}
				else if (distPlayer.x < 2.5f && stateAnimator.normalizedTime >= 1f && stateAnimator.IsName("УдарРазбег"))
				{
					animator.Play("Удар", 0, 0f);
				}
				else if (distPlayer.x < 1.8f && (stateAnimator.normalizedTime >= 1f || stateAnimator.IsTag("move")))
				{
					animator.Play("Удар", 0, 0f);
				}
				else if (distPlayer.x > 3f && numOfSrike >= 3 && stateAnimator.normalizedTime >= 0.9f && stateAnimator.IsTag("attack"))
				{
					SetScaleSpeed(1f);
					animator.Play("ИдтиБыстро", 0, 0f);
				}
				else if (distPlayer.x >= 3f && speedMove == 0f && stateAnimator.normalizedTime >= 0.9f && stateAnimator.IsTag("attack"))
				{
					animator.CrossFade("Идти", 0.1f);
					numOfSrike++;
				}
				else if (distPlayer.x < 10f && stateAnimator.IsTag("move") && !IsInvoking("AddScaleSpeed"))
				{
					Invoke("AddScaleSpeed", 3f);
				}
			}
			else if (modeMove == ModeMove.StandBreakLeg)
			{
				if (distPlayer.x < 2.5f && stateAnimator.IsTag("stand"))
				{
					animator.Play("УпалУдар", 0, 0f);
				}
				else if (distPlayer.x >= 3f && stateAnimator.normalizedTime >= 0.9f && stateAnimator.IsTag("attack"))
				{
					animator.Play("УпалРуки", 0, 0f);
				}
			}
			if (distPlayer.x < 1.75f)
			{
				if (speedMove < 0f)
				{
					base.transform.position = Player.onStage.transform.position;
					base.transform.Translate(1.75f, 0f, 0f);
				}
				else
				{
					Player.onStage.transform.position = base.transform.position;
					Player.onStage.transform.Translate(-1.75f, distPlayer.y, 0f);
				}
			}
		}
	}

	private void FixedUpdate()
	{
		if (!isDead)
		{
			return;
		}
		for (r = 0; r < rigidbodies.Length; r++)
		{
			if (spritesRigidbodies[r].transform.position != rigidbodies[r].gameObject.transform.position)
			{
				spritesRigidbodies[r].transform.position = rigidbodies[r].gameObject.transform.position - slideRagdoll;
				spritesRigidbodies[r].transform.rotation = rigidbodies[r].gameObject.transform.rotation;
			}
		}
	}

	private void LateUpdate()
	{
		angleBodyTimer = Mathf.Max(0f, angleBodyTimer - Time.deltaTime);
		if (angleBodyTimer != 0f && speedMove < 0f)
		{
			body.rotation = Quaternion.Euler(0f, 0f, Mathf.Lerp(0f, angleBodyBullet, angleBodyTimer / 0.25f));
		}
	}

	public override void OnActiveTriggers(bool value)
	{
		int num = 0;
		for (num = 0; num < triggersAnimations.Length; num++)
		{
			triggersAnimations[num].trigger.enabled = false;
		}
		if (value && !isDead)
		{
			stateAnimator = animator.GetCurrentAnimatorStateInfo(0);
			for (num = 0; num < triggersAnimations.Length; num++)
			{
				triggersAnimations[num].trigger.enabled = (triggersAnimations[num].trigger.enabled || stateAnimator.IsName(triggersAnimations[num].nameOrTag) || stateAnimator.IsTag(triggersAnimations[num].nameOrTag));
			}
		}
	}

	public override void OnHitBullet(HitBullet hitBullet)
	{
		int num = 0;
		int num2 = 0;
		bool flag = hitBullet.raycastHit.collider.sharedMaterial.name == materialBody.name;
		if (!CheckTriggerModeMove(hitBullet.raycastHit.collider))
		{
			if (flag && IsDeadNow())
			{
				maxScores = Player.onStage.AddScoresHits(hitBullet.raycastHit.collider.gameObject, hitBullet.weapon.cartridge.GetScaleScoresBullet(), maxScores, 0.5f);
			}
			else if (flag)
			{
				maxScores = Player.onStage.AddScoresHits(hitBullet.raycastHit.collider.gameObject, hitBullet.weapon.cartridge.GetScaleScoresBullet(), maxScores, 1f);
			}
		}
		if (hitBullet.raycastHit.collider.sharedMaterial.name != materialBody.name && PhysicsMaterialMultiply.GetMultiplyPower(hitBullet.collider.sharedMaterial, hitBullet.cartridgeRay.damageMultiply, 0f) != 0f)
		{
			flag = true;
		}
		HitTestArea(hitBullet.raycastHit.point, hitBullet.damage);
		num = 0;
		while (flag && num < coefficientDamageBody.Length)
		{
			if (hitBullet.raycastHit.collider.gameObject == coefficientDamageBody[num].partBody.gameObject)
			{
				health = HealthDamage(hitBullet.damage * coefficientDamageBody[num].coefficient, health);
				break;
			}
			num++;
		}
		colldersHit = Physics2D.OverlapCircleAll(hitBullet.raycastHit.point, 0.3f, hitBullet.cartridgeRay.layerBodies);
		spritiesHit = new SpriteRenderer[colldersHit.Length];
		for (num = 0; num < colldersHit.Length; num++)
		{
			for (int i = 0; i < spritesRigidbodies.Length; i++)
			{
				if (colldersHit[num].gameObject == spritesRigidbodies[i].gameObject && (colldersHit[num].gameObject == hitBullet.raycastHit.collider.gameObject || colldersHit[num].gameObject == hitBullet.raycastHit.collider.transform.parent.gameObject || colldersHit[num].transform.parent.gameObject == hitBullet.raycastHit.collider.gameObject || colldersHit[num].transform.parent == hitBullet.raycastHit.collider.transform.parent))
				{
					spritiesHit[num] = spritesRigidbodies[i];
					break;
				}
			}
		}
		for (num2 = 0; num2 < spritiesHit.Length; num2++)
		{
			for (num = 0; num < spritiesHit.Length; num++)
			{
				if (!(spritiesHit[num2] != null))
				{
					break;
				}
				if (num2 != num && spritiesHit[num2] == spritiesHit[num])
				{
					spritiesHit[num] = null;
				}
			}
		}
		if (flag && DamageOfSprite.enabledComponents)
		{
			for (num = 0; num < spritiesHit.Length; num++)
			{
				if (spritiesHit[num] != null)
				{
					SpriteHoles.AddHole(spritiesHit[num], hitBullet.raycastHit.point, materialHolesSkin);
					ShowMeat(spritiesHit[num].transform);
				}
			}
		}
		Vector3 eulerAngles = body.rotation.eulerAngles;
		float num3 = Mathf.Abs(Mathf.DeltaAngle(eulerAngles.z, 0f));
		if (isDead && colldersHit.Length != 0 && rigidbodies.Length == spritesRigidbodies.Length)
		{
			force = hitBullet.rayDirection * (hitBullet.impulse / (float)colldersHit.Length);
			for (num2 = 0; num2 < spritesRigidbodies.Length; num2++)
			{
				for (num = 0; num < spritiesHit.Length; num++)
				{
					if (spritiesHit[num] == spritesRigidbodies[num2])
					{
						if (num3 < 30f)
						{
							rigidbodies[num2].AddForceAtPosition(force * 0.05f, hitBullet.raycastHit.point, ForceMode2D.Impulse);
						}
						else
						{
							rigidbodies[num2].AddForceAtPosition(force, hitBullet.raycastHit.point, ForceMode2D.Force);
						}
						break;
					}
				}
			}
		}
		if (flag && Random.Range(0, 2) == 0)
		{
			PlaySound(soundsPain[Random.Range(0, soundsPain.Length)]);
		}
		if (health > 0f)
		{
			Vector3 eulerAngles2 = body.rotation.eulerAngles;
			if (eulerAngles2.z == 0f && (hitBullet.raycastHit.collider.gameObject == body.gameObject || hitBullet.raycastHit.collider.transform.parent == body.gameObject))
			{
				angleBodyBullet = (float)(-Random.Range(500, 1000)) / 100f;
				angleBodyTimer = 0.25f;
			}
		}
		colldersHit = new Collider2D[0];
		spritiesHit = new SpriteRenderer[0];
	}

	private bool CheckTriggerModeMove(Collider2D trigger)
	{
		if (modeMove == ModeMove.Default)
		{
			return false;
		}
		for (int i = 0; i < triggersBreakLegs.Length; i++)
		{
			if (triggersBreakLegs[i].GetInstanceID() == trigger.GetInstanceID())
			{
				return true;
			}
		}
		return false;
	}

	public override bool IsLowPriorityBullet(RaycastHit2D[] raycastHit, bool hitFront)
	{
		if (base.IsLowPriorityBullet(raycastHit, hitFront))
		{
			return true;
		}
		Vector3 localPosition = base.transform.localPosition;
		float num = localPosition.y;
		for (int i = 0; i < raycastHit.Length; i++)
		{
			if (!(raycastHit[i].collider != null))
			{
				continue;
			}
			sosed = raycastHit[i].collider.GetComponentInParent<ZombieInHome>();
			if (sosed != null && !sosed.isDead)
			{
				float num2 = Mathf.Floor(localPosition.x * 10f);
				Vector3 localPosition2 = sosed.transform.localPosition;
				if (num2 == Mathf.Floor(10f * localPosition2.x))
				{
					float a = num;
					Vector3 localPosition3 = sosed.transform.localPosition;
					num = Mathf.Min(a, localPosition3.y);
				}
			}
		}
		sosed = null;
		return localPosition.y > num;
	}

	public override void Death()
	{
		if (isDead)
		{
			return;
		}
		base.Death();
		animator.enabled = false;
		ref Vector3 reference = ref slideRagdoll;
		float num = floor;
		Vector3 position = base.gameObject.transform.position;
		reference.y = num - position.y;
		CancelInvoke();
		Invoke("DisablePhysicBody", timerBullet);
		angleBodyTimer = 0f;
		for (int i = 0; i < rigidbodies.Length; i++)
		{
			rigidbodies[i].gameObject.SetActive(value: true);
			rigidbodies[i].transform.SetParent(base.transform);
			spritesRigidbodies[i].transform.SetParent(base.transform);
			rigidbodies[i].transform.position = spritesRigidbodies[i].transform.position + slideRagdoll;
			rigidbodies[i].transform.rotation = spritesRigidbodies[i].transform.rotation;
			rigidbodies[i].simulated = true;
			if (Mathf.Abs(distBarricade.x) <= 1f)
			{
				rigidbodies[i].AddForce(new Vector2(2f, 0f), ForceMode2D.Impulse);
			}
			else if (!stateAnimator.IsTag("attack"))
			{
				rigidbodies[i].AddForce(new Vector2(speedMove * 0.5f, 0f), ForceMode2D.Impulse);
			}
			hingle = rigidbodies[i].gameObject.GetComponent<HingeJoint2D>();
			if (hingle == null)
			{
				rigidbodies[i].angularVelocity = Mathf.Clamp(rigidbodies[i].angularVelocity, -45f, 45f);
				continue;
			}
			Vector3 eulerAngles = hingle.transform.rotation.eulerAngles;
			if (Mathf.Abs(Mathf.DeltaAngle(eulerAngles.z, 180f)) < 40f)
			{
				hingle.transform.rotation = hingle.connectedBody.transform.rotation;
				hingle.transform.Rotate(0f, 0f, 45f);
			}
		}
		for (int j = 0; j < triggersAnimations.Length; j++)
		{
			UnityEngine.Object.Destroy(triggersAnimations[j].trigger);
		}
		triggersAnimations = new ColliderTriggerAnimation[0];
		UpdateSortingOrder();
		if (!sound.isPlaying || StopSound(sound.clip, soundsPain) || StopSound(sound.clip, soundsRage))
		{
			sound.Stop();
			sound.clip = soundsDie[Random.Range(0, soundsDie.Length)];
			sound.Play();
			UnityEngine.Debug.Log("sound.Play (soundsDie)");
		}
		coefficientDamageBody = new PartBodyDamage[0];
		if (ragdolls[ragdolls.Length - 1] != null)
		{
			UnityEngine.Object.Destroy(ragdolls[ragdolls.Length - 1].gameObject);
		}
		for (int num2 = ragdolls.Length - 1; num2 > 0; num2--)
		{
			ragdolls[num2] = ragdolls[num2 - 1];
		}
		ragdolls[0] = this;
	}

	private bool StopSound(AudioClip currentClip, AudioClip[] clips)
	{
		for (int i = 0; i < clips.Length; i++)
		{
			if (currentClip == clips[i])
			{
				return true;
			}
		}
		return false;
	}

	public void DisablePhysicBody(bool simulated)
	{
		base.enabled = (!isDead || simulated);
		sound.Stop();
		sound.enabled = !isDead;
		int num = 0;
		while (isDead && rigidbodies != null && num < rigidbodies.Length && !simulated)
		{
			if (rigidbodies[num].angularVelocity > 5f || rigidbodies[num].velocity.magnitude > 0.1f)
			{
				DisablePhysicBody(timerBullet);
				simulated = true;
				break;
			}
			num++;
		}
		if (isDead && !simulated)
		{
			for (int i = 0; i < collidersBody.Length; i++)
			{
				UnityEngine.Object.Destroy(collidersBody[i]);
			}
			for (int j = 0; j < joints.Length; j++)
			{
				UnityEngine.Object.Destroy(joints[j]);
			}
			for (int k = 0; k < rigidbodies.Length; k++)
			{
				UnityEngine.Object.Destroy(rigidbodies[k].gameObject);
			}
			collidersBody = new Collider2D[0];
			joints = new Joint2D[0];
			rigidbodies = new Rigidbody2D[0];
			UpdateSortingOrder();
		}
		else
		{
			if (simulated)
			{
				return;
			}
			for (int l = 0; l < rigidbodies.Length; l++)
			{
				rigidbodies[l].simulated = false;
			}
			for (int m = 0; m < collidersBody.Length; m++)
			{
				if (collidersBody[m].attachedRigidbody != null)
				{
					collidersBody[m].enabled = false;
				}
			}
		}
	}

	public void DisablePhysicBody()
	{
		DisablePhysicBody(simulated: false);
	}

	public void DisablePhysicBody(float time)
	{
		DisablePhysicBody(simulated: true);
		CancelInvoke("DisablePhysicBody");
		Invoke("DisablePhysicBody", time);
	}

	public void UpdateRigidbody(AreaDamage area)
	{
		tempListBody = (rigidbodies.Clone() as Rigidbody2D[]);
		tempListRender = (spritesRigidbodies.Clone() as SpriteRenderer[]);
		rigidbodies = new Rigidbody2D[tempListBody.Length + 1];
		spritesRigidbodies = new SpriteRenderer[tempListRender.Length + 1];
		int num = 0;
		for (num = 0; num < tempListBody.Length; num++)
		{
			rigidbodies[num] = tempListBody[num];
		}
		for (num = 0; num < tempListRender.Length; num++)
		{
			spritesRigidbodies[num] = tempListRender[num];
		}
		spliceBodyArea = area.GetComponentArea<SpliceBody>();
		rigidbodies[tempListBody.Length] = spliceBodyArea.newBody;
		spritesRigidbodies[tempListRender.Length] = spliceBodyArea.newRenderBody;
		collidersBody = new Collider2D[0];
	}

	public void DamageFromArea(int damage = 1)
	{
		health = HealthDamage(damage, health);
	}

	public void AttackStrike()
	{
		if (distBarricade.x <= 0f && !isDead && ZombieBarricade.onStage != null)
		{
			ZombieBarricade.onStage.OnStrikeZombie(damageStrike);
		}
		else
		{
			if (!(distPlayer.x <= 3f) || isDead || Player.onStage.isDead)
			{
				return;
			}
			Player.onStage.StrikeZombie();
			if (Player.onStage.isDead)
			{
				base.transform.position = Player.onStage.transform.position;
				base.transform.Translate(1.75f, -0.01f, 0f);
				UpdateSortingOrder();
				for (int i = 0; i < entities.Length; i++)
				{
					entities[i].PlayerIsDead();
				}
			}
		}
	}

	public void PlayerIsDead()
	{
		CancelInvoke();
		Vector3 vector = Player.onStage.transform.position - base.transform.position;
		vector.x = Mathf.Abs(vector.x);
		if (modeMove == ModeMove.Default && stateAnimator.IsTag("attack"))
		{
			if (vector.x <= 3f)
			{
				animator.CrossFade("Жрать", 0.1f);
			}
			else
			{
				animator.CrossFade("Идти", 0.1f);
			}
		}
		else if (modeMove == ModeMove.StandBreakLeg && stateAnimator.IsTag("attack"))
		{
			animator.Play("УпалЖрать");
		}
		else if (vector.x >= 6f && Random.Range(0, 6) != 1)
		{
			Stand();
		}
	}

	public void PlayerEventEath()
	{
		Player.onStage.ZombieBeginEath();
	}

	public void PlaySound(AudioClip audio)
	{
		if (!AudioListener.pause && sound != null && !isDead && sound.enabled && !sound.isPlaying)
		{
			sound.Stop();
			sound.clip = audio;
			sound.Play();
		}
	}

	public void PlaySoundOnce(AudioClip audio)
	{
		if (!AudioListener.pause && lastPlaySound != audio.name && sound != null && sound.enabled)
		{
			sound.Stop();
			sound.clip = audio;
			sound.Play();
			lastPlaySound = audio.name;
		}
	}

	public void PlaySoundOneClip(AudioClip audio)
	{
		if ((!sound.isPlaying || !(sound.clip != null) || !(sound.clip.name == audio.name)) && !AudioListener.pause && lastPlaySound != audio.name)
		{
			sound.Stop();
			sound.clip = audio;
			sound.Play();
			lastPlaySound = audio.name;
		}
	}

	public void PlaySoundRage()
	{
		if (!rageIsPlayed && soundsRage.Length != 0)
		{
			PlaySoundOneClip(soundsRage[Random.Range(0, soundsRage.Length)]);
			rageIsPlayed = true;
		}
	}

	public void BreakHingeJoint(HingeJoint2D joint)
	{
		Death();
		JointAngleLimits2D limits = default(JointAngleLimits2D);
		if (joint != null)
		{
			limits.min = joint.limits.min - (float)Random.Range(30, 40);
			limits.max = joint.limits.max + (float)Random.Range(15, 40);
			if (limits.min > -360f && limits.max < 540f)
			{
				joint.limits = limits;
			}
		}
	}

	public void DestroyAllJoints(Rigidbody2D body)
	{
		Death();
		listDestroy = body.transform.parent.GetComponentsInChildren<Joint2D>();
		for (int i = 0; i < listDestroy.Length; i++)
		{
			if (listDestroy[i].gameObject == body.gameObject || listDestroy[i].connectedBody == body)
			{
				listDestroy[i].enabled = false;
			}
		}
	}

	public void AnimatorCrossFade(string state)
	{
		if (!isDead && !animator.GetCurrentAnimatorStateInfo(0).IsName(state))
		{
			animator.CrossFade(state, 0.2f);
		}
	}

	public void AnimatorPlayInMove(string state)
	{
		if (modeMove == ModeMove.Default)
		{
			if (speedMove <= 0f && !isDead && animator.GetCurrentAnimatorStateInfo(0).IsTag("move"))
			{
				animator.Play(state);
			}
			else if (ZombieBarricade.onStage != null)
			{
				AnimatorPlayStand(state);
			}
		}
	}

	public void AnimatorPlay(string state)
	{
		if (modeMove == ModeMove.Default && !isDead && (!animator.GetCurrentAnimatorStateInfo(0).IsName(state) || animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.5f))
		{
			animator.Play(state);
		}
	}

	public void AnimatorPlayStand(string state)
	{
		if (IsStand())
		{
			AnimatorPlay(state);
		}
	}

	public bool IsStand()
	{
		return Mathf.Abs(Vector3.Distance(body.localPosition, startPositionBody)) < 0.5f;
	}

	public void StrikePlayerDisable(float timeStrike)
	{
		timeMarkStrikePlayer = Time.time + timeStrike;
	}

	public bool StrikePlayerIsActive()
	{
		return Time.time >= timeMarkStrikePlayer && !isDead;
	}

	public void StartMove(float speedMove)
	{
		this.speedMove = speedMove;
	}
}
