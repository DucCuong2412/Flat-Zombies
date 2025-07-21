using UnityEngine;
using UnityEngine.Events;

[AddComponentMenu("PVold/EventSimpleLocalisation")]
public class EventSimpleLocalisation : MonoBehaviour
{
	public UnityEvent russian;

	public UnityEvent english = new UnityEvent();

	public bool invoke = true;

	private void Awake()
	{
		if (invoke)
		{
			Invoke();
		}
	}

	public void Invoke()
	{
		if (SimpleLocalisation.isRussia())
		{
			russian.Invoke();
		}
		else
		{
			english.Invoke();
		}
	}
}
