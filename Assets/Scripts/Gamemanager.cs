using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gamemanager : MonoBehaviour
{
    public static Gamemanager instance;
    private Camera maincam;
    [Header("상점기능")]
    [SerializeField] Button shop;
    [SerializeField] GameObject shoppanel;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
    void Start()
    {
        maincam = Camera.main;
        shoppanel.SetActive(false);
    }
    void Update()
    {
        openShop();
    }

    private void openShop()
    {
        if (shoppanel.activeSelf == true)
        {
            Time.timeScale = 0.0f;
        }
        else if (shoppanel.activeSelf == false)
        {
            Time.timeScale = 1.0f;
        }
    }
}
