using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PromotionDemo : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image img;
    [HideInInspector]
    public Promotion promotion;
    public Button btn;
    [HideInInspector]
    public string url;
    public Transform playNowText;
    public CanvasGroup promotionCanvas;
    public GameObject callToAction;
    public float hoverScaleFactor = 1.2f;

    private void Start()
    {
        promotionCanvas.alpha = 0;
        PlayTextAnimation();
#if UNITY_EDITOR
        HidePromotion();
        return;
#endif
        btn.onClick.AddListener(() =>
        {
            Application.OpenURL(url);
        });
        promotion = GmSoft.Instance.GetPromotion();
        if (promotion == null) return;
        if (promotion.enable == "no" || promotion.promotionList.Length <= 0)
        {
            HidePromotion();
            return;
        }
        Refresh();
    }

    public void HidePromotion()
    {
        promotionCanvas.alpha = 0;
        promotionCanvas.blocksRaycasts = false;
    }

    private void PlayTextAnimation()
    {
        //playNowText.DOScale(1.1f, 0.6f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
    }

    public void Refresh()
    {
        if (promotion == null) return;
        if (promotion.enable == "no")
        {
            HidePromotion();
            return;
        }
        StartCoroutine(RefreshCourotine());
    }

    private IEnumerator RefreshCourotine()
    {
        if (promotion == null || promotion.promotionList.Length <= 0) yield break;
        PromotionData randomGame = promotion.GetRandom();
        if (randomGame == null) yield break;
        yield return new WaitUntil(() => GmSoft.Instance.GetPromotion().DownloadedAllImages()); // đợi cho đến khi download hết ảnh
        url = randomGame.url;
        img.sprite = randomGame.sprite;
        callToAction.SetActive(promotion.callToAction == 1);
        //promotionCanvas.DOFade(1, 0.5f).SetEase(Ease.OutQuint);
        promotionCanvas.alpha = 1;
        promotionCanvas.blocksRaycasts = true;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //transform.DOKill();
        //transform.DOScale(hoverScaleFactor, 0.15f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //transform.DOKill();
        //transform.DOScale(1, 0.15f);
    }
}
