using UnityEngine;
using UnityEngine.EventSystems;

public class HoverButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public float scale = 1.2f;
    public PlayIcon icon;

    public void OnPointerEnter(PointerEventData eventData)
    {
        //transform.DOScale(scale, 0.2f);
        if (icon != null) icon.PlayAnim();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //transform.DOScale(1, 0.2f);
        if (icon != null) icon.StopAnim();
    }
}
