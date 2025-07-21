using UnityEngine;
using UnityEngine.EventSystems;

public class ManagerZombies : MonoBehaviour, IEntityDeath, IEventSystemHandler
{
	public static int kills = 0;

	public static int aliveScene = 0;

	public static int aliveSceneTotal = 0;

	private static ZombieInHome[] tempList = new ZombieInHome[0];

	[Tooltip("Задержка перед активацией")]
	public float timeStart;

	[Tooltip("Количество добавляемых существ")]
	public int quantity = 1;

	[Tooltip("Временной промежуток для добавления")]
	public float periodAdd = 2f;

	[Tooltip("Максимальное кол-во живых одновременно на сцене")]
	public int limitOnStage = 7;

	public int total = 1000;

	[Tooltip("Мгновенно добавлять нового зомби, если на сцене нет ни одного живого")]
	public bool addImmediately;

	public bool randomEntity = true;

	public ZombieWithCharacter[] zombies = new ZombieWithCharacter[0];

	[Space(8f)]
	[Range(0.5f, 2f)]
	public float scaleSpeedMove = 1f;

	[Range(0.5f, 2f)]
	public float scaleSpeedMoveMax = 1f;

	public int addHealth;

	[Space(8f)]
	public bool randomPosition = true;

	public int idCurrentPosition;

	public Vector2[] positions;

	public float coefficient = 1f;

	private float timerAdd;

	public ZombieInHome[] entitiesOnStage = new ZombieInHome[0];

	private bool createIsActive = true;

	[HideInInspector]
	public int currentNumAlive;

	private float priorityTotal;

	private ManagerZombies[] managers = new ManagerZombies[0];

	private Vector2 position;

	private void OnDrawGizmos()
	{
		if (base.enabled)
		{
			Gizmos.color = new Color(0f, 0f, 0f, 0.1f);
			for (int i = 0; i < positions.Length; i++)
			{
				Gizmos.DrawWireSphere(base.transform.TransformPoint(positions[i]), 0.25f);
			}
		}
	}

	private void OnDrawGizmosSelected()
	{
		if (base.enabled)
		{
			Gizmos.color = new Color(0f, 0f, 0f);
			for (int i = 0; i < positions.Length; i++)
			{
				Gizmos.DrawWireSphere(base.transform.TransformPoint(positions[i]), 0.25f);
			}
			for (int j = 0; j < zombies.Length; j++)
			{
				zombies[j].priority = Mathf.Abs(zombies[j].priority);
			}
		}
	}

	private void Awake()
	{
		aliveSceneTotal = 0;
		aliveScene = 0;
		for (int i = 0; i < zombies.Length; i++)
		{
			zombies[i].SetLowerPriority(zombies, total);
		}
	}

	private void Start()
	{
		kills = 0;
		aliveScene += total;
		aliveSceneTotal += total;
		managers = base.gameObject.GetComponents<ManagerZombies>();
		currentNumAlive = 0;
		timerAdd = periodAdd;
		limitOnStage = Mathf.Min(limitOnStage, total);
		entitiesOnStage = new ZombieInHome[limitOnStage];
		if (randomPosition)
		{
			idCurrentPosition = UnityEngine.Random.Range(0, positions.Length);
		}
		if (timeStart == 0f && periodAdd == 0f)
		{
			FixedUpdate();
		}
	}

	private void CreateEntity(int numZombie)
	{
		if (createIsActive && zombies.Length != 0)
		{
			createIsActive = false;
			for (int i = 0; i < entitiesOnStage.Length; i++)
			{
				if (numZombie <= 0)
				{
					break;
				}
				if (total <= 0)
				{
					break;
				}
				if (!(entitiesOnStage[i] == null))
				{
					continue;
				}
				int num = 0;
				priorityTotal = 0f;
				for (int j = 0; j < zombies.Length; j++)
				{
					zombies[j].rangePriorityMin = priorityTotal;
					zombies[j].rangePriorityMax = priorityTotal + zombies[j].priority;
					priorityTotal += zombies[j].priority;
				}
				float num2 = UnityEngine.Random.value * priorityTotal;
				for (num = 0; num < zombies.Length; num++)
				{
					if (zombies[num].rangePriorityMin < num2 && num2 <= zombies[num].rangePriorityMax)
					{
						if (!randomEntity)
						{
							zombies[num].LowerPriority();
						}
						break;
					}
				}
				num = Mathf.Min(num, zombies.Length - 1);
				entitiesOnStage[i] = UnityEngine.Object.Instantiate(zombies[num].zombie);
				entitiesOnStage[i].character = zombies[num].characters;
				entitiesOnStage[i].listenerDeath = this;
				entitiesOnStage[i].SetScaleSpeed(Mathf.Floor(UnityEngine.Random.Range(scaleSpeedMove * 100f, scaleSpeedMoveMax * 100f)) * 0.01f);
				entitiesOnStage[i].health += addHealth;
				entitiesOnStage[i].transform.position = base.transform.TransformPoint(positions[idCurrentPosition]);
				createIsActive = true;
				numZombie--;
				currentNumAlive++;
				idCurrentPosition++;
				idCurrentPosition = Mathf.FloorToInt(Mathf.Repeat(idCurrentPosition, positions.Length));
				total--;
			}
		}
		if (total != 0)
		{
			return;
		}
		int num3 = limitOnStage / (managers.Length - 1);
		limitOnStage = 0;
		int num4 = 0;
		while (num3 >= 1 && num4 < managers.Length)
		{
			if (managers[num4].total != 0 && managers[num4] != this)
			{
				managers[num4].SetLimitOnStage(managers[num4].limitOnStage + num3);
			}
			num4++;
		}
	}

	private void FixedUpdate()
	{
		if (createIsActive && quantity != 0 && total != 0 && Time.timeScale != 0f)
		{
			timerAdd += Time.fixedDeltaTime / Time.timeScale;
			if (timerAdd >= (timeStart + periodAdd) * coefficient)
			{
				timeStart = 0f;
				timerAdd = 0f;
				CreateEntity(quantity);
			}
		}
	}

	public void OnDeathEntity(Entity entity)
	{
		int num = 0;
		while (true)
		{
			if (num < entitiesOnStage.Length)
			{
				if (entitiesOnStage[num] != null && entitiesOnStage[num].gameObject == entity.gameObject)
				{
					break;
				}
				num++;
				continue;
			}
			return;
		}
		kills++;
		currentNumAlive = Mathf.Max(0, currentNumAlive - 1);
		aliveScene = Mathf.Max(0, aliveScene - 1);
		aliveSceneTotal = Mathf.Max(0, aliveSceneTotal - 1);
		entitiesOnStage[num] = null;
		createIsActive = true;
		if (currentNumAlive == 0)
		{
			timerAdd = Mathf.Min(timerAdd, periodAdd * 0.4f);
		}
	}

	public void AddTimePeriod(float time)
	{
		if (periodAdd > 2f)
		{
			periodAdd += time;
			periodAdd = Mathf.Floor(periodAdd * 1000f) / 1000f;
		}
	}

	public void SetLimitOnStage(int value)
	{
		if (value < entitiesOnStage.Length)
		{
			for (int i = 0; i < entitiesOnStage.Length; i++)
			{
				if (entitiesOnStage[i] != null)
				{
					UnityEngine.Debug.LogError("Ошибка. ManagerZombies.SetLimitOnStage(): в массиве есть живые зомби");
					break;
				}
			}
			entitiesOnStage = new ZombieInHome[value];
			limitOnStage = value;
		}
		else if (value > entitiesOnStage.Length)
		{
			tempList = entitiesOnStage;
			entitiesOnStage = new ZombieInHome[value];
			for (int j = 0; j < tempList.Length; j++)
			{
				entitiesOnStage[j] = tempList[j];
			}
			tempList = new ZombieInHome[0];
			limitOnStage = value;
		}
		else if (entitiesOnStage.Length == 0)
		{
			entitiesOnStage = new ZombieInHome[value];
			limitOnStage = value;
		}
	}

	private void OnDestroy()
	{
		aliveScene -= total + currentNumAlive;
		aliveScene = Mathf.Max(0, aliveScene);
	}
}
