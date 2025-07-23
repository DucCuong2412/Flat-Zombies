using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class APILogoSetter : MonoBehaviour
{
    public Image img;
    public bool nativeSize;

    private void Start()
    {
        if (img == null) img = GetComponent<Image>();
        StartCoroutine(SetLogo());
    }
     
    private IEnumerator SetLogo() 
    {
        //if (nativeSize) img.SetNativeSize();
        img.sprite = GmSoft.Instance.GetLogo(); //default logo
        yield return new WaitForEndOfFrame();
        if (nativeSize) img.SetNativeSize();
        yield return new WaitUntil(() => GmSoft.Instance.loadedLogo);
        img.sprite = GmSoft.Instance.GetLogo();  ////new loaded logo
    }
}
