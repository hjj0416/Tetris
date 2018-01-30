using UnityEngine;

public class DelayActive : MonoBehaviour
{
    public float Delay = 0f;
    public GameObject[] Objects;

    void Start()
    {
        Invoke("Active", Delay);
    }

    void Active()
    {
        if (Objects != null && Objects.Length > 0)
        {
            for (int i = 0; i < Objects.Length; i++)
            {
                Objects[i].SetActive(true);
            }
        }
    }

}
