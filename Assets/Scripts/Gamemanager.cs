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
    [Header("�������")]
    [SerializeField] Button shop;
    [SerializeField] GameObject shoppanel;
    [SerializeField] Button shoppanelexit;
    [SerializeField] TMP_Text hptext;
    [SerializeField] TMP_Text damagetext;
    [Header("���ӿ���")]
    [SerializeField] Button btnMainMenu;
    [SerializeField] GameObject gameOver;
    [Header("����Ŭ����")]
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
        hptext.text = $"�ִ�ü�� = {_maxHp.ToString()} \n ����ü�� = {_curHp.ToString()} \n (���۽� ü��ȸ��)";
        damagetext.text = $"���� Level = {_damage.ToString()}";
    }
}
