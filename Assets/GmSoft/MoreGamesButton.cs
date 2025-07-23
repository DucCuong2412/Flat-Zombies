using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class MoreGamesButton : MonoBehaviour
{
    public Button btn;

    private void Start()
    {
        gameObject.SetActive(GmSoft.Instance.EnableMoreGame());
        if (btn == null)
        {
            btn = GetComponent<Button>();
        }
        btn.onClick.AddListener(() =>
        {
            string url = GmSoft.Instance.GetMoreGameUrl();
            Application.OpenURL(url);
        });
    }
}
