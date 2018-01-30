using UnityEngine;
using UnityEngine.UI;
using System.Collections;

//便于图片压缩，将A*B(A>1024)的图片切割为两张图，然后组合在一起
public class SplitRawImage : RawImage
{
    private float _uv1AdaptW = -0.0005f;
    private float _uv2AdaptY = 0.03120f;

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        float rateW = 1f / uvRect.width;
        float rateH = uvRect.height;
        Texture tex = mainTexture;
        vh.Clear();
        if (tex != null)
        {
            var r = GetPixelAdjustedRect();
            var rect1 = new Vector4(r.x, r.y, r.x + (r.width + 0.5f) * rateW, r.y + r.height);
            var rect2 = new Vector4(r.x + r.width * rateW, r.y, r.x + r.width, r.y + r.height);
            var uv1 = new Rect(0, 0, 1f + _uv1AdaptW, rateH);
            var uv2 = new Rect(0, rateH + _uv2AdaptY, rateH, uvRect.width - 1f);
            var scaleX = tex.width * tex.texelSize.x;
            var scaleY = tex.height * tex.texelSize.y;
            {
                var color32 = color;
                //0123
                vh.AddVert(new Vector3(rect1.x, rect1.y), color32, new Vector2(uv1.xMin * scaleX, uv1.yMin * scaleY));
                vh.AddVert(new Vector3(rect1.x, rect1.w), color32, new Vector2(uv1.xMin * scaleX, uv1.yMax * scaleY));
                vh.AddVert(new Vector3(rect1.z, rect1.w), color32, new Vector2(uv1.xMax * scaleX, uv1.yMax * scaleY));
                vh.AddVert(new Vector3(rect1.z, rect1.y), color32, new Vector2(uv1.xMax * scaleX, uv1.yMin * scaleY));

                //4567
                vh.AddVert(new Vector3(rect2.x, rect2.y), color32, new Vector2(uv2.xMax * scaleX, uv2.yMin * scaleY));
                vh.AddVert(new Vector3(rect2.x, rect2.w), color32, new Vector2(uv2.xMin * scaleX, uv2.yMin * scaleY));
                vh.AddVert(new Vector3(rect2.z, rect2.w), color32, new Vector2(uv2.xMin * scaleX, uv2.yMax * scaleY));
                vh.AddVert(new Vector3(rect2.z, rect2.y), color32, new Vector2(uv2.xMax * scaleX, uv2.yMax * scaleY));

                vh.AddTriangle(4, 5, 6);
                vh.AddTriangle(6, 7, 4);
                vh.AddTriangle(0, 1, 2);
                vh.AddTriangle(2, 3, 0);
            }
        }
    }
}
