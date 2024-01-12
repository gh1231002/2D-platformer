using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemUi : MonoBehaviour
{
    [SerializeField] private TMP_Text coinText;
    [SerializeField] private Image item;
    private void Start()
    {
        GameObject objPlayer = GameObject.Find("Player");
        Player player = objPlayer.GetComponent<Player>();
        player.SetItemGet(this);
    }
    public void SetItemGet(int _Num)
    {
        string value = $"X {_Num}";
        coinText.text = value;
    }
}
