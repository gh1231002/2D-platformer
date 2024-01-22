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
    [SerializeField] Button Key;
    [SerializeField] Button KeyExit;
    [SerializeField] GameObject KeyGuide;
    [SerializeField] TMP_Text keyText;
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
        keyText.text = "���� : space \n ���� : LeftCtrl \n ������ : A or D �Ǵ� ���� ȭ��ǥ or ������ ȭ��ǥ";
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
