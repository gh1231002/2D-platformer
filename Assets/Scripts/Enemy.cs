using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.ReorderableList;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] GameObject objExplosion;
    [SerializeField] Collider2D trigger;
    [SerializeField] LayerMask layer;
    [SerializeField] LayerMask layerEnemy;
    [SerializeField] private float moveSpeed = 2;
    [SerializeField]private float MaxHp = 3;
    [SerializeField] Transform trsLayer;
    private Sprite sprdefault;
    private SpriteRenderer sr;
    private float CurHp;
    Rigidbody2D rigid;

    [Header("보스패턴")]
    [SerializeField] private bool isBoss;
    [SerializeField] private int bossPattern1Count;
    [SerializeField] private int bossPattern2Count;
    private int bossPattern = 1; //현재패턴
    private bool patternChange = false; //패턴을 바꿔야하는지
    private int patternConunt = 0; //몇번 패턴을 했는지
    private float patternTimer = 0.0f; //패턴 변경 시간

    private void Awake()
    {
        CurHp = MaxHp;
        rigid = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        sprdefault = sr.sprite;
    }
    
    void Update()
    {
        moving();
        checnkBossPattern();
    }

    private void moving()
    {
        if(isBoss == false)
        { 
            rigid.velocity = new Vector2(moveSpeed, rigid.velocity.y);
        }
        else
        {
            bossStartMoving();
        }
    }

    private void checnkBossPattern()
    {
        patternTimer += Time.deltaTime;
        if(patternChange == true)
        {
            if(patternTimer >= 2.0f)
            {
                patternTimer = 0.0f;
                patternChange = false;
            }
            return;
        }
    }

    private void bossStartMoving()
    {
        rigid.velocity = new Vector2(-moveSpeed, rigid.velocity.y);
    }

    private void FixedUpdate()
    {
        if (trigger.IsTouchingLayers(layer)==false)
        {
            turn();
        }
        if(trigger.IsTouchingLayers(layerEnemy)==true)
        {
            turn();
        }
    }

    private void turn()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;

        moveSpeed *= -1;
    }

    public void Hit(float _damage, bool _bodySlam = false)
    {
        CurHp -= _damage;
        if(CurHp <=0)
        {
            Destroy(gameObject);
            Instantiate(objExplosion, transform.position, Quaternion.identity, trsLayer);
        }
        else if(_bodySlam == true && isBoss == false)
        {
            GameObject obj = GameObject.Find("Player");
            Player player = obj.GetComponent<Player>();
            player.Hit(1);
            Destroy(gameObject);
            GameObject objEx =  Instantiate(objExplosion, transform.position, Quaternion.identity, trsLayer);
            Explosion explosion = objEx.GetComponent<Explosion>();
            float sizeWidth = sr.sprite.rect.width;
            explosion.AnimationSize(sizeWidth);
        }
        else if(_bodySlam == true && isBoss == true)
        {
            GameObject obj = GameObject.Find("Player");
            Player player = obj.GetComponent<Player>();
            player.Hit(1);
        }
        else
        {
            sr.color = new Color(1f, 0f, 0f, 1f);
            Invoke("SetSpriteDefault", 0.1f);
        }
    }

    private void SetSpriteDefault()
    {
        sr.color = Color.white;
    }
}
