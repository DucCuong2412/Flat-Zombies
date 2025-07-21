using UnityEngine;
using UnityEngine.EventSystems;

public class TouchButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler, IBeginDragHandler, IEndDragHandler, IEventSystemHandler
{
	public PointerEventData eventDataDown;

	public PointerEventData eventDataUp;

	public PointerEventData eventDataBeginDrag;

	public PointerEventData eventDataDrag;

	public PointerEventData eventDataEndDrag;

	public TouchButtonEvent pointerDown = new TouchButtonEvent();

	public TouchButtonEvent pointerUp = new TouchButtonEvent();

	public TouchButtonEvent beginDrag = new TouchButtonEvent();

	public TouchButtonEvent drag = new TouchButtonEvent();

	public TouchButtonEvent endDrag = new TouchButtonEvent();

	public void OnPointerDown(PointerEventData eventData)
	{
		if (eventDataDown == null)
		{
			eventDataDown = eventData;
			pointerDown.Invoke(eventData);
			pointerDown.baseEvent.Invoke();
			eventDataUp = null;
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
		}
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		if (eventDataBeginDrag == null)
		{
			eventDataBeginDrag = eventData;
			eventDataDrag = eventData;
			eventDataEndDrag = null;
			beginDrag.Invoke(eventData);
			beginDrag.baseEvent.Invoke();
		}
	}

	public void OnDrag(PointerEventData eventData)
	{
		if (eventDataDrag == null)
		{
			eventDataDrag = eventData;
			eventDataBeginDrag = null;
			eventDataEndDrag = null;
		}
		drag.Invoke(eventDataDrag);
		drag.baseEvent.Invoke();
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		if (eventDataEndDrag == null)
		{
			eventDataEndDrag = eventData;
			eventDataBeginDrag = null;
			eventDataDrag = null;
			endDrag.Invoke(eventData);
			endDrag.baseEvent.Invoke();
		}
	}

	private void OnDisable()
	{
		if (eventDataDrag != null)
		{
			OnEndDrag(eventDataDrag);
		}
		if (eventDataDown != null)
		{
			OnPointerUp(eventDataDown);
		}
	}
}
