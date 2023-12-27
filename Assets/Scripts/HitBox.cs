using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour
{
    [SerializeField] private Player.hitType hitType;
    Player player;
    Enemy enemy;
    void Start()
    {
        player = GetComponentInParent<Player>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {

        player.TriggerEnter(hitType, collision);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {

        player.TriggerExit(hitType, collision);  
    }
}
