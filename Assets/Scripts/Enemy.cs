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
    private Animator anim;

    [Header("보스패턴")]
    [SerializeField] private bool isBoss;
    private float pattern1Reload = 1f;
    private int pattern1Count = 1;
    private bool pattern1 = false;

    private bool bossMove = false;
    private float moveTime = 0.0f;
    [SerializeField]private int bossPattern = 1; //현재패턴
    private bool patternChange = false; //패턴을 바꿔야하는지
    private int patternConunt = 0; //몇번 패턴을 했는지
    private float patternTimer = 0.0f; //패턴 변경 시간
    

    private void Awake()
    {
        CurHp = MaxHp;
        rigid = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        sprdefault = sr.sprite;
    }
    
    void Update()
    {
        moving();
        bossAnimation();
        //checnkBossPattern();
    }

    private void moving()
    {
        if(isBoss == false)
        {
            rigid.velocity = new Vector2(moveSpeed, rigid.velocity.y);
        }
       else if(isBoss == true)
        {
            bossMove = true;
            rigid.velocity = new Vector2(-moveSpeed, rigid.velocity.y);
            moveTime += Time.deltaTime;
            if(moveTime >= 5.0f)
            {
                turn();
                moveTime = 0.0f;
            }
        }
    }

    private void bossAnimation()
    {
        anim.SetBool("BossMove", bossMove);
        anim.SetBool("Pattern1", pattern1);
    }

    //private void checnkBossPattern()
    //{
    //    if (isBoss == false) return;
    //    patternTimer += Time.deltaTime;
    //    if (patternChange == true)
    //    {
    //        if(patternTimer >= 2.0f)
    //        {
    //            patternTimer = 0.0f;
    //            patternChange = false;
    //        }
    //        return;
    //    }
        
    //    switch(bossPattern)
    //    {
    //        case 1:
    //            {
    //                if(patternTimer >= pattern1Reload)
    //                {
    //                    patternTimer = 0.0f;
    //                    patternConunt++;
    //                    pattern1 = true;
    //                    if(patternConunt >= pattern1Count)
    //                    {
    //                        bossPattern++;
    //                        patternChange = true;
    //                        patternConunt = 0;
    //                    }
    //                    pattern1 = false;
    //                }
    //            }
    //            break;
    //        case 2:
    //            {
    //                if (patternTimer >= pattern2Reload)
    //                {
    //                    patternTimer = 0.0f;
    //                    patternConunt++;
    //                    pattern2 = true;
    //                    if (patternConunt >= pattern2Count)
    //                    {
    //                        bossPattern = 1;
    //                        patternChange = true;
    //                        patternConunt = 0;
    //                    }
    //                    pattern2 = false;
    //                }
    //            }
    //            break;
    //    }
    //}

    private void FixedUpdate()
    {
        if (trigger.IsTouchingLayers(layer)==false && isBoss == false)
        {
            turn();
        }
        
        else if (trigger.IsTouchingLayers(layerEnemy)==true && isBoss == false)
        {
            turn();
        }

        else if (trigger.IsTouchingLayers(layer) == true && isBoss == true)
        {
            bossMove = false;
            pattern1 = true;
        }
        else if (trigger.IsTouchingLayers(layer) == false && isBoss == true)
        {
            bossMove = true;
            pattern1 = false;
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
        if(CurHp <=0 && isBoss == false)
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
