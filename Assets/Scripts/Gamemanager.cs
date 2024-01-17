using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Gamemanager : MonoBehaviour
{
    public static Gamemanager instance;
    private Camera maincam;
    [Header("상점기능")]
    [SerializeField] Button shop;
    [SerializeField] GameObject shoppanel;
    [SerializeField] Button shoppanelexit;
    [SerializeField] TMP_Text hptext;
    [SerializeField] TMP_Text damagetext;
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
        openShop();
    }
    void Update()
    {
        
    }

    private void openShop()
    {
        shop.onClick.AddListener(() =>
        {
            shoppanel.SetActive(true);
            Time.timeScale = 0.0f;
        });
        shoppanelexit.onClick.AddListener(() =>
        {
            shoppanel.SetActive(false);
            Time.timeScale = 1.0f;
        });
    }

    public void checkPlayerStat(float _maxHp, float _curHp, float _damage)
    {
        hptext.text = $"최대체력 = {_maxHp.ToString()} \n 현재체력 = {_curHp.ToString()}";
        damagetext.text = $"현재 공격력 = {_damage.ToString()}";
    }
}
