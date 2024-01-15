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
    [SerializeField] private int pattern1Count = 8;
    [SerializeField] private float pattern1Reload = 1f;
    [SerializeField] GameObject obj2;
    private bool attack1 = false;
    private bool attack2 = false;
    private bool death = false;

    private bool bossMove = false;
    private float moveTime = 0.0f;
    [SerializeField]private int bossPattern = 0;//현재패턴
    private float shootTimer = 0.0f;
    private int patternCount = 0;
    private bool patternChange = false;
    private float timer = 0.0f;


    bool meetPlayer = false;
    public bool GetisBoss()
    {
        return isBoss;
    }

    public void meetSomething(bool _something)
    {
        meetPlayer = _something;
    }
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
        randomAttack();
    }

    private void moving()
    {
        if (death == true) return;
        if (attack1 == true) return;
        if (attack2 == true) return;
        if (isBoss == false)
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
        anim.SetBool("Attack1", attack1);
        anim.SetBool("Attack2", attack2);
        anim.SetBool("Death", death);
    }


    //private void OnBecameVisible()
    //{
    //    meetPlayer = true;
    //}

    private void randomAttack()
    {
        timer += Time.deltaTime;
        if(timer >= 5.0f)
        {
            bossPattern = Random.Range(0, 3);
            timer = 0.0f;
        }
        shootTimer += Time.deltaTime;
        if(patternChange == true)
        {
            if(shootTimer >= 3.0f)
            {
                shootTimer = 0.0f;
                patternChange = false;
            }
            return;
        }
        if (meetPlayer == true)
        {
            switch (bossPattern)
            {
                case 0:
                    {
                        if (trigger.IsTouchingLayers(layer) == true && isBoss == true)
                        {
                            attack1 = true;
                            rigid.velocity = Vector2.zero;
                        }
                        else if (trigger.IsTouchingLayers(layer) == false && isBoss == true)
                        {
                            attack1 = false;
                        }
                    }
                    break;
                case 1:
                    {
                        if (trigger.IsTouchingLayers(layer) == true && isBoss == true)
                        {
                            attack2 = true;
                            rigid.velocity = Vector2.zero;
                        }
                        else if (trigger.IsTouchingLayers(layer) == false && isBoss == true)
                        {
                            attack2 = false;
                        }
                    }
                    break;
                case 2:
                    {
                        if (shootTimer >= pattern1Reload)
                        {
                            shootTimer = 0.0f;
                            posShoot();
                            if (patternCount >= pattern1Count)
                            {
                                bossPattern = 0;
                                patternChange = true;
                            }
                        }
                    }
                    break;
            }
        }
    }

    private void creatBullet(GameObject _obj, Vector3 _pos, Vector3 _rot, float _speed)
    {
        GameObject obj = Instantiate(_obj, _pos, Quaternion.Euler(_rot), trsLayer);
        Bullet objSc = obj.GetComponent<Bullet>();
        objSc.SetDamege(false, 1, true, _speed);
    }
    private void posShoot()
    {
        GameObject objPlayer = GameObject.Find("Player");
        Player player = objPlayer.GetComponent<Player>();
        Vector3 playerPos = player.transform.position;
        creatBullet(obj2, playerPos + new Vector3(0,5,0), new Vector3(0, 0, -90), 10);
        patternCount++;
    }
    private void FixedUpdate()
    {
        if(death == true) return;
        if (trigger.IsTouchingLayers(layer)== false && isBoss == false)
        {
            turn();
        }
        
        else if (trigger.IsTouchingLayers(layerEnemy)== true && isBoss == false)
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
        if(CurHp <=0 && isBoss == false && _bodySlam == false)
        {
            Destroy(gameObject);
            Instantiate(objExplosion, transform.position, Quaternion.identity, trsLayer);
            GameObject obj = GameObject.Find("GameManager");
            Item item = obj.GetComponent<Item>();
            item.CreatItem(transform.position);
        }
        else if (CurHp <=0 && isBoss == true)
        {
            rigid.velocity = Vector2.zero;
            death = true;
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
