using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ZXing;
using ZXing.QrCode;

public class QrCodeView : MonoBehaviour
{
    public UnityEngine.UI.Image image;
    Texture2D encoded;

    private WebCamTexture camTexture;

    private Color32[] c;
    private int W, H;

    private Rect screenRect;

    public void UpdateQRCode(string textForEncoding)
    {
        encoded = new Texture2D(256, 256);
        encoded.filterMode = FilterMode.Point;
        encoded.wrapMode = TextureWrapMode.Clamp;

        var color32 = Encode(textForEncoding, encoded.width, encoded.height);
        encoded.SetPixels32(color32);
        encoded.Apply();

        encoded.filterMode = FilterMode.Point;
        encoded.wrapMode = TextureWrapMode.Clamp;

        image.sprite = Sprite.Create(encoded, new Rect(0, 0, encoded.width, encoded.height), new Vector2(0.5f, 0.5f), 1);
    }

    private static Color32[] Encode(string textForEncoding, int width, int height)
    {
        var writer = new BarcodeWriter
        {
            Format = BarcodeFormat.QR_CODE,
            Options = new QrCodeEncodingOptions
            {
                Height = height,
                Width = width
            }
        };
        return writer.Write(textForEncoding);
    }
}
