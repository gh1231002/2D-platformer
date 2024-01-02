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
    [SerializeField] private float bossPattern1Reload;
    [SerializeField] GameObject pattern1Bullet;
    [SerializeField] private int bossPattern2Count;
    [SerializeField] private float bossPattern2Reload;
    [SerializeField] GameObject pattern2Bullet;
    [SerializeField] private int bossPattern3Count;
    [SerializeField] private float bossPattern3Reload;
    [SerializeField] GameObject pattern3Bullet;
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
       else if(isBoss == true)
        {
            
        }
    }

    private void checnkBossPattern()
    {
        if (isBoss == false) return;
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
        
        switch(bossPattern)
        {
            case 1:
                {
                    if(patternTimer >= bossPattern1Reload)
                    {
                        patternTimer = 0.0f;
                        shootStraight();
                        if (patternConunt >= bossPattern1Count)
                        {
                            bossPattern++;
                            patternChange = true;
                            patternConunt = 0;
                        }
                    }
                }
                break;
            case 2:
                {
                    if (patternTimer >= bossPattern2Reload)
                    {
                        patternTimer = 0.0f;
                        airShotgun();
                        if (patternConunt >= bossPattern2Count)
                        {
                            bossPattern++;
                            patternChange = true;
                            patternConunt = 0;
                        }
                    }
                }
                break;
            case 3:
                {
                    if (patternTimer >= bossPattern3Reload)
                    {
                        patternTimer = 0.0f;
                        airGatling();
                        if (patternConunt >= bossPattern3Count)
                        {
                            bossPattern++;
                            patternChange = true;
                            patternConunt = 0;
                        }
                    }
                }
                break;
        }
    }

    private void creatBullet(GameObject _obj, Vector3 _pos, Vector3 _rot, float _speed)
    {
        GameObject obj = Instantiate(_obj,_pos,Quaternion.Euler(_rot),trsLayer);
        Bullet objSc = obj.GetComponent<Bullet>();
        bool isRight = false;
        if (transform.localScale.x == -1)
        {
            isRight = true;
        }
        objSc.SetDamege(false,1,isRight,_speed);
    }
    private void shootStraight()
    {
        creatBullet(pattern1Bullet, transform.position, Vector3.zero, 5);
        creatBullet(pattern1Bullet, transform.position + new Vector3(0,1,0), Vector3.zero, 5);
        creatBullet(pattern1Bullet, transform.position + new Vector3(0,2,0), Vector3.zero, 5);
        patternConunt++;
    }
    private void airShotgun()
    {

    }
    private void airGatling()
    {

    }



    private void FixedUpdate()
    {
        if (trigger.IsTouchingLayers(layer)==false && isBoss == false)
        {
            turn();
        }
        if(trigger.IsTouchingLayers(layerEnemy)==true && isBoss == false)
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
