using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ScreenApperance : MonoBehaviour
{
	public enum Action
	{
		None,
		Enter,
		Exit
	}

	public static ScreenApperance main;

	public static string nameObject = string.Empty;

	[Tooltip("Действие при старте - скрыть/показать")]
	public Action action = Action.Enter;

	[Tooltip("Время появления, в секундах")]
	public float time = 1f;

	public bool slideFront = true;

	private Image image;

	private Transform[] objectsEvent;

	private void Start()
	{
		if (slideFront)
		{
			base.transform.SetAsLastSibling();
		}
		image = base.gameObject.GetComponent<Image>();
		image.raycastTarget = true;
		image.enabled = true;
		main = this;
		nameObject = string.Empty;
		if (action == Action.None)
		{
			Image obj = image;
			Color color = image.color;
			float r = color.r;
			Color color2 = image.color;
			float g = color2.g;
			Color color3 = image.color;
			obj.color = new Color(r, g, color3.b, 0f);
			action = Action.Enter;
			FixedUpdate();
		}
	}

	private void FixedUpdate()
	{
		if (action == Action.Enter)
		{
			Image obj = image;
			Color color = image.color;
			float r = color.r;
			Color color2 = image.color;
			float g = color2.g;
			Color color3 = image.color;
			float b = color3.b;
			Color color4 = image.color;
			obj.color = new Color(r, g, b, color4.a - Time.fixedDeltaTime * (1f / time));
			Color color5 = image.color;
			if (color5.a <= 0f)
			{
				base.gameObject.SetActive(value: false);
				objectsEvent = UnityEngine.Object.FindObjectsOfType<Transform>();
				for (int i = 0; i < objectsEvent.Length; i++)
				{
					ExecuteEvents.Execute(objectsEvent[i].gameObject, null, delegate(IScreenApperance handler, BaseEventData data)
					{
						handler.OnScreenApperance(nameObject);
					});
				}
				nameObject = string.Empty;
			}
		}
		else
		{
			if (action != Action.Exit)
			{
				return;
			}
			Image obj2 = image;
			Color color6 = image.color;
			float r2 = color6.r;
			Color color7 = image.color;
			float g2 = color7.g;
			Color color8 = image.color;
			float b2 = color8.b;
			Color color9 = image.color;
			obj2.color = new Color(r2, g2, b2, color9.a + Time.fixedDeltaTime * (1f / time));
			Color color10 = image.color;
			if (color10.a >= 1f)
			{
				objectsEvent = UnityEngine.Object.FindObjectsOfType<Transform>();
				for (int j = 0; j < objectsEvent.Length; j++)
				{
					ExecuteEvents.Execute(objectsEvent[j].gameObject, null, delegate(IScreenApperanceExit handler, BaseEventData data)
					{
						handler.OnScreenExit(nameObject);
					});
				}
				nameObject = string.Empty;
			}
		}
	}

	public void PlayEnter(string gameObjName)
	{
		if (nameObject == string.Empty || nameObject == gameObjName)
		{
			base.gameObject.SetActive(value: true);
			action = Action.Enter;
			nameObject = gameObjName;
		}
	}

	public void PlayExit(string gameObjName)
	{
		if (nameObject == string.Empty || nameObject == gameObjName)
		{
			base.gameObject.SetActive(value: true);
			action = Action.Exit;
			nameObject = gameObjName;
			objectsEvent = UnityEngine.Object.FindObjectsOfType<Transform>();
			for (int i = 0; i < objectsEvent.Length; i++)
			{
				ExecuteEvents.Execute(objectsEvent[i].gameObject, null, delegate(IScreenApperancePlayExit handler, BaseEventData data)
				{
					handler.OnScreenPlayExit(nameObject);
				});
			}
		}
	}
}
