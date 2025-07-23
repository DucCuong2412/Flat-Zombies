using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class LockReasonControl : MonoBehaviour
{
    public TMP_Text label;

    private void Start()
    {
        if (GmSoft.Instance == null || !GmSoft.Instance.enableLog) 
        {
            Destroy(gameObject);
            return;
        } 
        if(label == null) label = GetComponent<TMP_Text>(); 
        label.text = GmSoft.Instance.lockReason.ToString();
    }

    private void Update()
    {
        if (!GmSoft.Instance.enableLog) 
        {
            Destroy(gameObject);
        }
    }
}
