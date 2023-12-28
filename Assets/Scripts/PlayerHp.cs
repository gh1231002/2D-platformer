using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHp : MonoBehaviour
{
    [SerializeField] private Image Heart;
    [SerializeField] private TMP_Text Hp;
    private void Start()
    {
        GameObject objPlayer = GameObject.Find("Player");
        Player player = objPlayer.GetComponent<Player>();
        player.SetPlayerHp(this);
    }
    private void Update()
    {
        
    }
    public void SetPlayerHp(float _curHp)
    {
        string value = $"x {(int)_curHp}";
        Hp.text = value;
    }
}
