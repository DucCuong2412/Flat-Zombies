using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class GameVersion : MonoBehaviour
{
    public TMP_Text version;

    private void Start()
    {
        if (version == null)
        {
            version = GetComponent<TMP_Text>();
        }
        version.text = $"v {Application.version}";
    }
}
