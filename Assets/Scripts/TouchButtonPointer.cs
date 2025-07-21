using UnityEngine;
using UnityEngine.EventSystems;

public class TouchButtonPointer : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IEventSystemHandler
{
	public delegate void ListenerMethod(TouchButtonPointer source);

	public PointerEventData eventDataDown;

	public PointerEventData eventDataUp;

	public TouchButtonEvent pointerDown = new TouchButtonEvent();

	public TouchButtonEvent pointerUp = new TouchButtonEvent();

	[Space(6f)]
	public bool pointerUpOnDisable = true;

	private ListenerMethod listenerPointerDown;

	private ListenerMethod listenerPointerUp;

	private void Awake()
	{
	}

	public void SetListenerPointerDown(ListenerMethod listener)
	{
		listenerPointerDown = listener;
	}

	public void SetListenerPointerUp(ListenerMethod listener)
	{
		listenerPointerUp = listener;
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		if (eventDataDown == null)
		{
			eventDataDown = eventData;
			pointerDown.Invoke(eventData);
			pointerDown.baseEvent.Invoke();
			eventDataUp = null;
			if (listenerPointerDown != null)
			{
				listenerPointerDown(this);
			}
		}
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		if (eventDataUp == null)
		{
			eventDataUp = eventData;
			eventDataDown = null;
			pointerUp.Invoke(eventData);
			pointerUp.baseEvent.Invoke();
			if (listenerPointerUp != null)
			{
				listenerPointerUp(this);
			}
		}
	}

	private void OnDisable()
	{
		if (pointerUpOnDisable && eventDataDown != null)
		{
			OnPointerUp(eventDataDown);
		}
	}
}
