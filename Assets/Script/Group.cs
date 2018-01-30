using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Group : MonoBehaviour {

    float lastFall = 0;

    void Start()
    {
        //如果生成的方块位置超过了高度h，则游戏结束
        if (!IsValidGridPos())
        {
            Debug.Log("GAME OVER");
            Destroy(gameObject);
        }
    }

    void Update()
    {
        //向左移动
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            // 位置向左移动1
            transform.position += new Vector3(-1, 0, 0);

            //检查是否越界
            if (IsValidGridPos())
                //没有越界，则移动
                UpdateGrid();
            else
                //如果越界，向右移动1，保持不动
                transform.position += new Vector3(1, 0, 0);
        }

        //向右移动
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {        
            transform.position += new Vector3(1, 0, 0);
            if (IsValidGridPos())
                UpdateGrid();
            else
                transform.position += new Vector3(-1, 0, 0);
        }

        //按下上键，旋转
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            transform.Rotate(0, 0, -90);
            if (IsValidGridPos())
                UpdateGrid();
            else
                transform.Rotate(0, 0, 90);
        }

        // 向下移动或者每秒自动向下移动
        else if (Input.GetKeyDown(KeyCode.DownArrow) ||Time.time - lastFall >= 1)
        {
            transform.position += new Vector3(0, -1, 0);

            //如果没有越界或者Grid为空，则向下移动一个单位，否则
            if (IsValidGridPos())
            {
                UpdateGrid();
            }
            else
            {
                transform.position += new Vector3(0, 1, 0);

                // 删除被填满行的方块
                Grid.DeleteFullRows();

                //产生下一个方块
                FindObjectOfType<Spawner>().SpanNext();

                // 关闭脚本
                enabled = false;
            }

            lastFall = Time.time;
        }
        else if(Input.GetKeyDown(KeyCode.Space))
        {
            if (Time.timeScale == 1)
            {
                Time.timeScale = 0;
            }       
            else
            {
                Time.timeScale = 1;
            }
        }
    }

    bool IsValidGridPos()
    {
        foreach (Transform child in transform)
        {
            Vector2 v = Grid.RoundVec2(child.position);

            // 判断子方块是否在边界内
            if (!Grid.InsideBorder(v))
                return false;

            // 判断子方块是否属于同一组
            if (Grid.grid[(int)v.x, (int)v.y] != null &&Grid.grid[(int)v.x, (int)v.y].parent != transform)
                return false;
        }
        return true;
    }

    void UpdateGrid()
    {
        //移除旧的方块
        for (int y = 0; y < Grid.h; y++)
            for (int x = 0; x < Grid.w; x++)
                if (Grid.grid[x, y] != null)
                    if (Grid.grid[x, y].parent == transform)
                        Grid.grid[x, y] = null;

        // 添加新的方块
        foreach (Transform child in transform)
        {
            Vector2 v = Grid.RoundVec2(child.position);
            Grid.grid[(int)v.x, (int)v.y] = child;
        }
    }
}
