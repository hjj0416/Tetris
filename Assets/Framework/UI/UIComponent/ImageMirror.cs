using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ImageMirror : BaseMeshEffect
{
    public enum MirrorType
    {
        LeftRight,
        UpDown,
        Quad,
    }

    [SerializeField]
    private MirrorType m_type = MirrorType.LeftRight;

    [SerializeField]
    private bool m_UseGraphicAlpha = true;

    private Vector2 size;

    public MirrorType Type
    {
        get { return m_type; }
        set
        {
            if (m_type == value)
                return;
            m_type = value;

            if (graphic != null)
                graphic.SetVerticesDirty();
        }
    }

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        size = this.GetComponent<RectTransform>().sizeDelta;
        base.OnValidate();
        if (graphic != null)
            graphic.SetVerticesDirty();
    }
#endif
    protected override void OnEnable()
    {
        base.OnEnable();
        if (graphic != null)
            graphic.SetVerticesDirty();
        size = this.GetComponent<RectTransform>().sizeDelta;
    }

    public bool useGraphicAlpha
    {
        get { return m_UseGraphicAlpha; }
        set
        {
            m_UseGraphicAlpha = value;
            if (graphic != null)
                graphic.SetVerticesDirty();
        }
    }

    protected void ApplyGhostDouble(List<UIVertex> verts, int start, int end, float x1, float y1, float x2, float y2, bool self = false)
    {
        UIVertex vt;

        var neededCpacity = verts.Count * 2;
        if (verts.Capacity < neededCpacity)
            verts.Capacity = neededCpacity;


        for (int i = start; i < end; i++)
        {
            vt = verts[i];
            if (!self)
                verts.Add(vt);//添加一遍mesh

            Vector3 v = vt.position;
            int offset = i % 6;
            switch (offset)
            {
                case 0:
                case 1:
                case 5: v.x += x1; break;
                case 3:
                case 4:
                case 2: v.x += x2; break;
            }
            switch (offset)
            {
                case 1:
                case 2:
                case 3: v.y += y1; break;
                case 0:
                case 4:
                case 5: v.y += y2; break;
            }

            vt.position = v;
            verts[i] = vt;
        }
    }

    protected void ApplyGhost(List<UIVertex> verts, int start, int end)
    {
        if (m_type == MirrorType.LeftRight)
        {
            var neededCpacity = verts.Count * 2;
            if (verts.Capacity < neededCpacity)
                verts.Capacity = neededCpacity;
            ApplyGhostDouble(verts, start, end, 0, 0, -size.x / 2, 0);
            start = end;
            end = verts.Count;
            ApplyGhostDouble(verts, start, end, size.x, 0, -size.x / 2, 0, true);
        }
        else if (m_type == MirrorType.UpDown)
        {
            var neededCpacity = verts.Count * 2;
            if (verts.Capacity < neededCpacity)
                verts.Capacity = neededCpacity;
            ApplyGhostDouble(verts, start, end, 0, 0, 0, size.y/2);
            start = end;
            end = verts.Count;
            ApplyGhostDouble(verts, start, end, 0, -size.y, 0, size.y/2, true);
        }
        else if (m_type == MirrorType.Quad)
        {
            var neededCpacity = verts.Count * 4;
            if (verts.Capacity < neededCpacity)
                verts.Capacity = neededCpacity;
            start = 0;
            end = verts.Count;
            ApplyGhostDouble(verts, start, end, 0, 0, -size.x / 2, size.y / 2);
            start = end;
            end = verts.Count;
            ApplyGhostDouble(verts, start, end, size.x, 0, -size.x / 2, size.y / 2);
            start = end;
            end = verts.Count;
            ApplyGhostDouble(verts, start, end, 0, -size.y, -size.x / 2, size.y / 2);
            start = end;
            end = verts.Count;
            ApplyGhostDouble(verts, start, end, size.x, -size.y, -size.x / 2, size.y / 2, true);
        }
    }

    public override void ModifyMesh(VertexHelper vh)
    {
        if (!IsActive())
            return;
        size = GetComponent<RectTransform>().sizeDelta;

        List<UIVertex> output = new List<UIVertex>();
        vh.GetUIVertexStream(output);
        ApplyGhost(output, 0, output.Count);
        vh.Clear();
        vh.AddUIVertexTriangleStream(output);
    }
}
