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
    [SerializeField] Button Key;
    [SerializeField] Button KeyExit;
    [SerializeField] GameObject KeyGuide;
    [SerializeField] TMP_Text keyText;
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
        KeyGuide.SetActive(false);
        GameObject obj = GameObject.Find("Player");
        player = obj.GetComponent<Player>();
        helpGuide();
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

    private void helpGuide()
    {
        Key.onClick.AddListener(() =>
        {
            KeyGuide.SetActive(true);
            Time.timeScale = 0.0f;
        });
        KeyExit.onClick.AddListener(() =>
        {
            KeyGuide.SetActive(false);
            Time.timeScale = 1.0f;
        });
        keyText.text = "점프 : space \n 공격 : LeftCtrl \n 움직임 : A or D 또는 왼쪽 화살표 or 오른쪽 화살표";
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
