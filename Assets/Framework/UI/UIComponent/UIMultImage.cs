using UnityEngine;
using UnityEngine.UI;

public class UIMultImage : Image
{
    [SerializeField]private bool nativeSize;
    public Sprite[] sprites;

    /// <summary>
    /// 设置状态 0-n
    /// </summary>
    /// <param name="index"></param>
    public void SetSprite(int index)
    {
        if (index >= 0 && index < sprites.Length)
        {
            this.sprite = sprites[index];
            if (nativeSize)
                SetNativeSize();
        }
    }
}
