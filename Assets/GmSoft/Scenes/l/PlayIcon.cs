//using DG.Tweening;
using UnityEngine;

public class PlayIcon : MonoBehaviour
{
    private Vector3 startPosition;

    public void PlayAnim() 
    {
        //transform.DOLocalMoveX(8f, 0.4f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
    }

    public void StopAnim() 
    {
        //transform.DOKill();
        transform.localPosition = Vector3.zero;
    }
}
