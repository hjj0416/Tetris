#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 绑定到 ui 的父对象身上，可以在scene界面查看
/// 凡是勾选了raycast target的对象，会显示蓝色矩形框
/// </summary>
public class ShowRaycast : MonoBehaviour
{
	static Vector3[] fourCorners = new Vector3[4];
	void OnDrawGizmos()
	{
		foreach (MaskableGraphic g in GameObject.FindObjectsOfType<MaskableGraphic>())
		{
			if (g.raycastTarget)
			{
				RectTransform rectTransform = g.transform as RectTransform;
				rectTransform.GetWorldCorners(fourCorners);
				Gizmos.color = Color.blue;
			    for (int i = 0; i < 4; i++)
			    {
			        Gizmos.DrawLine(fourCorners[i], fourCorners[(i + 1) % 4]);
			    }
			}
		}
	}
}
#endif