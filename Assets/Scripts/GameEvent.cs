using System;
using UnityEngine;
using UnityEngine.Events;

[AddComponentMenu("PVold/GameEvent")]
public class GameEvent : MonoBehaviour
{
	public class ListenerEvent
	{
		public string name;

		public Method method;

		public GameObject gameObject;

		public ListenerEvent(string name, Method method, GameObject gameObject = null)
		{
			this.name = name;
			this.method = method;
			this.gameObject = gameObject;
		}
	}

	[Serializable]
	public struct ListenerOfList
	{
		public string name;

		public UnityEvent call;

		public void Invoke(GameObject source)
		{
			call.Invoke();
		}
	}

	public delegate void Method(GameObject gameObject);

	public static ListenerEvent[] listeners = new ListenerEvent[100];

	public ListenerOfList[] listListeners;

	public static void AddEventListener(string name, Method method, GameObject gameObject = null)
	{
		int num = 0;
		for (num = 0; num < listeners.Length; num++)
		{
			if (listeners[num] == null)
			{
				listeners[num] = new ListenerEvent(name, method, gameObject);
				break;
			}
		}
		if (num + 1 == listeners.Length)
		{
			UnityEngine.Debug.LogError("GameEvent: Недостаточно места в массиве GameEvent.listeners[]");
		}
	}

	public static void RemoveEventListener(string name, Method method)
	{
		for (int i = 0; i < listeners.Length; i++)
		{
			if (listeners[i] != null && listeners[i].name == name && listeners[i].method == method)
			{
				listeners[i] = null;
			}
		}
	}

	public static void RemoveEventListener(string name)
	{
		for (int i = 0; i < listeners.Length; i++)
		{
			if (listeners[i] != null && listeners[i].name == name)
			{
				listeners[i] = null;
			}
		}
	}

	public static void RemoveEventListener(Method method)
	{
		for (int i = 0; i < listeners.Length; i++)
		{
			if (listeners[i] != null && listeners[i].method == method)
			{
				listeners[i] = null;
			}
		}
	}

	public static void RemoveEventListener(GameObject gameObject = null)
	{
		for (int i = 0; i < listeners.Length; i++)
		{
			if (listeners[i] != null && listeners[i].gameObject == gameObject)
			{
				listeners[i] = null;
			}
		}
	}

	public static void DeclareEvent(string name, GameObject source)
	{
		bool flag = false;
		for (int i = 0; i < listeners.Length; i++)
		{
			if (listeners[i] != null && listeners[i].name == name)
			{
				flag = true;
				listeners[i].method(source);
			}
		}
		if (!flag)
		{
			UnityEngine.Debug.LogWarning("GameEvent.DeclareEvent: Не найдено функции для события " + name + "\n от игрового объекта " + source.name);
		}
	}

	private void Awake()
	{
		for (int i = 0; i < listListeners.Length; i++)
		{
			AddEventListener(listListeners[i].name, listListeners[i].Invoke, base.gameObject);
		}
	}

	private void OnDestroy()
	{
		for (int i = 0; i < listeners.Length; i++)
		{
			if (listeners[i] != null && listeners[i].gameObject != null)
			{
				listeners[i] = null;
			}
		}
	}
}
