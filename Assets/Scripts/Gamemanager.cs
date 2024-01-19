using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    [Header("게임오버")]
    [SerializeField] Button btnMainMenu;
    [SerializeField] GameObject gameOver;
    [Header("게임클리어")]
    [SerializeField] GameObject gameClear;
    [SerializeField] Button btnStart;
    Player player;
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
        gameOver.SetActive(false);
        gameClear.SetActive(false);
        GameObject obj = GameObject.Find("Player");
        player = obj.GetComponent<Player>();
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

    public void GameOver()
    {
            gameOver.SetActive(true);
            btnMainMenu.onClick.AddListener(() =>
            {
                SceneManager.LoadSceneAsync(0);
            });
    }

    public void GameClear()
    {
        gameClear.SetActive(true);
        btnStart.onClick.AddListener(() =>
        {
            SceneManager.LoadSceneAsync(0);
        });
    }

    public void checkPlayerStat(float _maxHp, float _curHp, float _damage)
    {
        hptext.text = $"최대체력 = {_maxHp.ToString()} \n 현재체력 = {_curHp.ToString()} \n (업글시 체력회복)";
        damagetext.text = $"공격 Level = {_damage.ToString()}";
    }
}
