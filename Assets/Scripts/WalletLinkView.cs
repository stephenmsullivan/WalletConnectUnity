using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalletLinkView : MonoBehaviour
{
    public QrCodeView codeView;
    string link;

    public TMPro.TextMeshProUGUI linkText;

    private void Start()
    {
        this.gameObject.SetActive(false);
    }

    public void SetLink(string linkUrl)
    {
        link = linkUrl;
        linkText.text = linkUrl;
        codeView.UpdateQRCode(linkUrl);
    }

    public void OpenLink()
    {
        Debug.Log(link);
        Application.OpenURL(link);
    }

    public void CloseView()
    {
        this.gameObject.SetActive(false);
    }

}
