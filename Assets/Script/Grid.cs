using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour {

    public static int w = 10;
    public static int h = 20;
    public static Transform[,] grid = new Transform[w,h];

    //设置坐标都为整数
    public static Vector2 RoundVec2(Vector2 v)
    {
        return new Vector2(Mathf.Round(v.x), Mathf.Round(v.y));
    }

    //检查某个方块是否在边界之内
    public static bool InsideBorder(Vector2 pos)
    {
        return ((int)pos.x >= 0 && (int)pos.x < w && (int)pos.y >= 0&&(int)pos.y<h);
    }

    //用于删除某一被堆满方块的行
    public static void DeleteRow(int y)
    {
        for(int x=0;x<w;x++)
        {
            Destroy(grid[x, y].gameObject);
            grid[x, y] = null;
        }
    }

    //当某一行被删除后，便让上一行的所有方块移到这一行上
    public static void DecreaseRow(int y)
    {
        for (int x = 0; x < w; x++)
        {
            if (grid[x, y] != null)
            {
                grid[x, y - 1] = grid[x, y];
                grid[x, y] = null;

                grid[x, y - 1].position += new Vector3(0, -1, 0);
            }
        }
    }


    //将所有被删除行之上的所有方块都往下移动一行
    public static void DecreaseRowsAbove(int y)
    {
        for (int i = y; i < h; i++)
            DecreaseRow(i);
    }

    //判断某一行是否被填满方块
    public static bool IsRowFull(int y)
    {
        for (int x = 0; x < w; x++)
            if (grid[x, y] == null)
                return false;
        return true;
    }

    //删除所有被填满的行上的方块
    public static void DeleteFullRows()
    {
        int num = 0;
        for (int y = 0; y < h; y++)
        {
            if (IsRowFull(y))
            {
                DeleteRow(y);
                DecreaseRowsAbove(y + 1);
                --y;
                num++;
            }
        }
        if(num>0)
            PlayerDataMgr.Instance.playerData.AddScore((int)Mathf.Pow(2,num));
    }

}
