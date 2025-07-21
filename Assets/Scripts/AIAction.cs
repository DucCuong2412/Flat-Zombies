using UnityEngine;

public class AIAction
{
	public delegate void Method(AIAction action);

	public string name = "name_action";

	public Method method;

	public float timeStart;

	public float time;

	public int repeat;

	public float timeRepeat;

	public float timeStartRepeat;

	public AISimple ai;

	public bool isActive = true;

	public AIAction(string name, Method method, float timeStart = 0f, float time = 0f, int repeat = 0)
	{
		this.name = name;
		this.timeStart = timeStart;
		this.time = time;
		this.method = method;
		this.repeat = repeat;
		timeRepeat = time;
		timeStartRepeat = timeStart;
	}

	public void Update()
	{
		if (!isActive)
		{
			return;
		}
		if (timeStart <= 0f && repeat > 0)
		{
			repeat--;
			if (repeat >= 0)
			{
				method(this);
			}
		}
		else if (timeStart <= 0f && time >= 0f)
		{
			if (time > 0f)
			{
				time -= Time.deltaTime;
			}
			if (time >= 0f)
			{
				method(this);
			}
		}
		else
		{
			timeStart -= Time.deltaTime;
			UnityEngine.Debug.Log(name);
		}
	}

	public void Remove()
	{
		time = -1f;
		repeat = 0;
	}
}
