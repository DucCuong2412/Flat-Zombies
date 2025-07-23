using System.Collections;
using TMPro;
using UnityEngine;

public class LockSceneControl : MonoBehaviour
{
    public SpriteRenderer gameImage;
    public TMP_Text productName;
    public TMP_Text description;

    private void Start()
    {
        if (GmSoft.Instance == null) return;
        SDKGameInfo gameInfo = GmSoft.Instance.GetGameInfo();
        if (gameInfo == null) return;
        productName.text = gameInfo.name;
        description.text = gameInfo.description;
        StartCoroutine(SetGameImage());
        Debug.Log($"set game image");
    }

    private IEnumerator SetGameImage()
    {
        gameImage.enabled = false;
        yield return new WaitUntil(() => GmSoft.Instance.loadedGameImage);
        Debug.Log($"apply game image");
        gameImage.enabled = true;
        gameImage.sprite = GmSoft.Instance.GetGameImage();  ////new game image
    }
}
