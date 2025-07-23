using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class APIGameImageSetter : MonoBehaviour
{
    public Image img;
    public bool nativeSize;

    private void Start()
    {
        if(img == null) img = GetComponent<Image>();
        StartCoroutine(SetImage());
    }

    private IEnumerator SetImage()
    {
        img.enabled = false;
        yield return new WaitUntil(() => GmSoft.Instance.loadedGameImage);
        img.enabled = true;
        img.sprite = GmSoft.Instance.GetGameImage();  ////new game image
        yield return new WaitForSeconds(0.1f);
        if (nativeSize) img.SetNativeSize();
    }
}
