using UnityEngine;

public class AISimple : MonoBehaviour
{
	public AIAction[] actions = new AIAction[20];

	public AIAction currentAction;

	public AIAction[] addActions = new AIAction[20];

	private int a;

	private void Start()
	{
	}

	private void Update()
	{
		if (actions[0] != null)
		{
			if (actions[0].time < 0f && actions[0].repeat == 0)
			{
				a = 1;
				while (a < actions.Length && actions[a] != null)
				{
					actions[a - 1] = actions[a];
					a++;
				}
			}
			else
			{
				actions[0].Update();
			}
		}
		for (a = 0; a < addActions.Length; a++)
		{
			if (addActions[a] != null)
			{
				if (addActions[a].time < 0f && addActions[a].repeat == 0)
				{
					addActions[a] = null;
				}
				else
				{
					addActions[a].Update();
				}
			}
		}
	}

	public void AddAction(AIAction action)
	{
		action.ai = this;
		int num = 0;
		while (true)
		{
			if (num < actions.Length)
			{
				if (actions[num] != null && actions[num].name == action.name)
				{
					actions[num] = action;
					return;
				}
				if (actions[num] == null)
				{
					break;
				}
				num++;
				continue;
			}
			return;
		}
		actions[num] = action;
	}

	public void AddAction(string name, AIAction.Method method, float timeStart = 0f, float time = 0f, int repeat = 0)
	{
		AIAction action = new AIAction(name, method, timeStart, time, repeat);
		AddAction(action);
	}

	public void SetActiveAction(string name, bool value)
	{
		int num = 0;
		while (true)
		{
			if (num < actions.Length && actions[num] != null)
			{
				if (actions[num].name == name)
				{
					break;
				}
				num++;
				continue;
			}
			return;
		}
		actions[num].isActive = value;
	}

	public void SetActiveAction(AIAction.Method method, bool value)
	{
		int num = 0;
		while (true)
		{
			if (num < actions.Length && actions[num] != null)
			{
				if (actions[num].method == method)
				{
					break;
				}
				num++;
				continue;
			}
			return;
		}
		actions[num].isActive = value;
	}
}
