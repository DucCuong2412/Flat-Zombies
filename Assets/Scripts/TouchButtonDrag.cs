using UnityEngine;
using UnityEngine.EventSystems;

public class TouchButtonDrag : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IEventSystemHandler
{
	public PointerEventData eventDataBeginDrag;

	public PointerEventData eventDataDrag;

	public PointerEventData eventDataEndDrag;

	public TouchButtonEvent beginDrag = new TouchButtonEvent();

	public TouchButtonEvent drag = new TouchButtonEvent();

	public TouchButtonEvent endDrag = new TouchButtonEvent();

	private void Awake()
	{
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
	}
}
