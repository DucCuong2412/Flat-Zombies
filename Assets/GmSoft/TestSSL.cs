using Assets.GMDev.Utilities;
using System;
using UnityEngine;

public class TestSSL : MonoBehaviour
{
    [Serializable]
    public class Player
    {
        public string name;
        public string status;
        public int score;
        public string date;
    }


    private void Start()
    {
        string passphrase = "gmdev@123!?000:))))";
        Player player = new Player()
        {
            name = "ABCXYZ",
            status = "Xin chào tất cả anh em ở công ty này",
            score = 1000,
            date = DateTime.Now.ToShortDateString()
        };
        var json = JsonUtility.ToJson(player);
        string enryptStr = OpenSSL.OpenSSLEncrypt(json, passphrase);
        string decryptStr = OpenSSL.OpenSSLDecrypt(enryptStr, passphrase);
        Debug.Log(enryptStr);
        Debug.Log(decryptStr);

    }
}
