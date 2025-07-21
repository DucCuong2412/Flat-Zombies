using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[Serializable]
public class TouchButtonEvent : UnityEvent<PointerEventData>
{
	public UnityEvent baseEvent = new UnityEvent();
}
