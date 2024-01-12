using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] GameObject item;
    private void Awake()
    {

    }
    public void CreatItem(Vector3 _creatPos)
    {
        Instantiate(item,_creatPos,Quaternion.identity);
    }
}
