using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.ReorderableList;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public enum hitType
    {
        WallCheck,
        HitCheck,
        WakeEnemy,
    }
    [Header("플레이어 데이터")]
    [SerializeField] private float MoveSpeed = 5;
    [SerializeField] private float gravity = 9.81f;
    [SerializeField] private float JumpForce = 5;
    [SerializeField] private Transform layerDynamic;
    [SerializeField] private Sprite sprHurt;
    [SerializeField] private float PlayerMaxHp = 3;
    [SerializeField] private float PlayerCurHp;
    [SerializeField] Button BtnHpUp;
    [SerializeField] Button BtnHpDown;
    [SerializeField] Button BtnDamageUp;
    [SerializeField] Button BtnDamageDown;
    private SpriteRenderer sr;
    private Sprite sprDefault;
    BoxCollider2D boxCollider2D;
    private Camera maincam;
    private Animator anim;
    Rigidbody2D rigid;
    Vector3 moveDir;
    [SerializeField] private bool isGround = false;
    private float verticalVelocity;
    private bool isJump = false;
    PlayerHp playerHp;

    Gamemanager gamemanager;

    [Header("벽점프")]
    private bool wallStep = false;
    private bool doWallStep = false;
    private bool doWallStepTimer = false;
    private float wallStepTimer = 0.0f;
    [SerializeField] private float wallStepTime = 0.5f;

    [Header("Dash")]
    [SerializeField] private float dashTimer = 0.3f;
    private bool dash = false;
    private float dashTime = 0.0f;
    private TrailRenderer dashEffect;

    [Header("총알세팅")]
    private Transform trsShootpos;
    private float timer = 0.01f;
    [SerializeField] private float BulletDamage = 0;
    [SerializeField, Range(0.1f, 1.0f)] private float TimeShoot = 0.5f;
    [SerializeField, Range(1f, 3f)] private int BulletLevel = 1;

    [Header("프리팹")]
    [SerializeField] GameObject objBullet1;
    [SerializeField] GameObject objBullet2;
    private int Count = 0;
    ItemUi itemUi;
    private void OnDrawGizmos()
    {
        if (boxCollider2D != null)
        {
            Gizmos.color = Color.red;
            Vector3 pos = boxCollider2D.bounds.center - new Vector3(0, 0.1f, 0);
            Gizmos.DrawWireCube(pos, boxCollider2D.bounds.size);
        }
    }

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (dash == true) return;
    //    if(collision.gameObject.tag == "Enemy")
    //    {
    //        Enemy obj = collision.gameObject.GetComponent<Enemy>();
    //        obj.Hit(1,true);
    //    }
    //    else if (collision.gameObject.tag == "Item")
    //    {
    //        Destroy(collision.gameObject);
    //        Count += 1;
    //        itemUi.SetItemGet(Count);
    //    }
    //}
    public void SetItemGet(ItemUi _value)
    {
        itemUi = _value;
        itemUi.SetItemGet(Count);
    }

    public void Hit(float _damage)
    {
        PlayerCurHp -= _damage;
        playerHp.SetPlayerHp(PlayerCurHp);
        if (PlayerCurHp == 0)
        {
            Destroy(gameObject);
            dashEffect.enabled = false;
            gamemanager.GameOver();
        }
    }
    
    private void Awake()
    {
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        dashEffect = GetComponent<TrailRenderer>();
        sr = GetComponent<SpriteRenderer>();
        sprDefault = sr.sprite;
        trsShootpos = transform.Find("ShotPos");
        dashEffect.enabled = false;
        PlayerCurHp = PlayerMaxHp;
        transform.position = new Vector3(-25,3,0);
    }
    void Start()
    {
        maincam = Camera.main;
        gamemanager = Gamemanager.instance;
        btnCheck();
    }

    void Update()
    {
        returnPlayerStat();
        moving();
        doAnimation();
        turning();
        checkGround();
        Jumping();
        checkGravity();
        checkDoStepWallTimer();
        checkDash();
        checkShoot();
    }

    private void returnPlayerStat()
    {
        gamemanager.checkPlayerStat(PlayerMaxHp, PlayerCurHp, BulletLevel);
        playerHp.SetPlayerHp(PlayerCurHp);
    }

    private void btnCheck()
    {
        BtnHpUp.onClick.AddListener(() =>
        {
            if (Count == 0) return;
            if (PlayerMaxHp == 10) return;
            Count -= 1;
            itemUi.SetItemGet(Count);
            PlayerMaxHp++;
            PlayerCurHp = PlayerMaxHp;
            if (PlayerMaxHp >= 10)
            {
                PlayerMaxHp = 10;
                PlayerCurHp = PlayerMaxHp;
            }
        });
        BtnHpDown.onClick.AddListener(() =>
        {
            if (Count == 0) return;
            if (PlayerMaxHp == 3) return;
            Count += 1;
            itemUi.SetItemGet(Count);
            PlayerMaxHp--;
            PlayerCurHp = PlayerMaxHp;
            if (PlayerMaxHp <= 3)
            {
                PlayerMaxHp = 3;
                PlayerCurHp = PlayerMaxHp;
            }
        });
        BtnDamageUp.onClick.AddListener(() =>
        {
            if (Count == 0) return;
            if (BulletLevel == 2) return;
            Count -= 1;
            itemUi.SetItemGet(Count);
            BulletLevel += 1;
            if (BulletLevel >= 2)
            {
                BulletLevel = 2;
            }
        });
        BtnDamageDown.onClick.AddListener(() =>
        {
            if (Count == 0) return;
            if (BulletLevel == 1) return;
            Count += 1;
            itemUi.SetItemGet(Count);
            BulletLevel -= 1;
            if (BulletLevel <= 1)
            {
                BulletLevel = 1;
            }
        });
        playerHp.SetPlayerHp(PlayerCurHp);
    }

    private void moving()
    {
        if (dash == true || doWallStepTimer == true) return;
        moveDir.x = Input.GetAxisRaw("Horizontal") * MoveSpeed;
        moveDir.y = rigid.velocity.y;
        rigid.velocity = moveDir;
    }

    private void doAnimation()
    {
        anim.SetInteger("Horizontal", (int)moveDir.x);
        anim.SetBool("isGround", isGround);
    }

    private void turning()
    {
        if (moveDir.x < 0 && transform.localScale.x != -1)
        {
            Vector3 scale = transform.localScale;
            scale.x = -1;
            transform.localScale = scale;
        }
        else if (moveDir.x > 0 && transform.localScale.x != 1)
        {
            Vector3 scale = transform.localScale;
            scale.x = 1;
            transform.localScale = scale;
        }
    }

    private void checkGround()
    {
        isGround = false;
        if (verticalVelocity > 0)
        {
            return;
        }
        RaycastHit2D hit = Physics2D.BoxCast(boxCollider2D.bounds.center, boxCollider2D.bounds.size, 0f, Vector2.down, 0.1f, LayerMask.GetMask("Ground"));
        if (hit.transform != null && hit.transform.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            isGround = true;
        }
    }

    private void Jumping()
    {
        if (dash == true) return;
        if (isGround == false)
        {
            if (Input.GetKeyDown(KeyCode.Space) && wallStep == true && moveDir.x != 0)
            {
                doWallStep = true;
            }
        }

        else if (Input.GetKeyDown(KeyCode.Space) && isGround == true)
        {
            isJump = true;
        }
    }

    private void checkGravity()
    {
        if (dash == true) return;
        if (doWallStep == true)
        {
            Vector2 dir = rigid.velocity;
            dir.x *= -1;
            rigid.velocity = dir;
            verticalVelocity = JumpForce;
            doWallStepTimer = true;
            doWallStep = false;
        }
        if (isGround == false)
        {
            verticalVelocity -= gravity * Time.deltaTime;
            if (verticalVelocity < -10.0f)
            {
                verticalVelocity = -10f;
            }
        }
        else
        {
            verticalVelocity = 0f;
        }

        if (isJump == true)
        {
            isJump = false;
            verticalVelocity = JumpForce;
        }
        rigid.velocity = new Vector2(rigid.velocity.x, verticalVelocity);
    }

    private void checkDoStepWallTimer()
    {
        if (doWallStepTimer == true)
        {
            wallStepTimer += Time.deltaTime;
            if (wallStepTimer >= wallStepTime)
            {
                wallStepTimer = 0.0f;
                doWallStepTimer = false;
            }
        }
    }

    private void checkDash()
    {
        if (isGround == false) return;
        if (Input.GetKeyDown(KeyCode.LeftShift) && dash == false)
        {
            dash = true;
            verticalVelocity = 0f;
            if (transform.localScale.x == 1)
            {
                rigid.velocity = new Vector2(10.0f, 0f);
                dashEffect.enabled = true;
            }
            else if (transform.localScale.x == -1)
            {
                rigid.velocity = new Vector2(-10.0f, 0f);
                dashEffect.enabled = true;
            }
            gameObject.layer = LayerMask.NameToLayer("Dash");
        }
        else if (dash == true)
        {
            dashTime += Time.deltaTime;
            if (dashTime >= dashTimer)
            {
                dashTime = 0.0f;
                dashEffect.enabled = false;
                dashEffect.Clear();
                dash = false;
                gameObject.layer = LayerMask.NameToLayer("Player");
            }
        }
    }

    private void checkShoot()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            shootBullet();
        }
    }

    private void shootBullet()
    {
        createBullet(trsShootpos.position, Vector3.zero);
    }

    private void createBullet(Vector3 _pos, Vector3 _rot)
    {
        bool isRight = true;
        if (transform.localScale.x == -1)
        {
            isRight = false;
        }
        switch (BulletLevel)
        {
            case 1:
                {
                    GameObject obj = Instantiate(objBullet1, _pos, Quaternion.Euler(_rot), layerDynamic);
                    Bullet objsc = obj.GetComponent<Bullet>();
                    objsc.SetDamege(true, BulletDamage,isRight,1);
                }
                break;
            case 2:
                {
                    BulletDamage = 2;
                    GameObject obj = Instantiate(objBullet2, _pos, Quaternion.Euler(_rot), layerDynamic);
                    Bullet objsc = obj.GetComponent<Bullet>();
                    objsc.SetDamege(true,BulletDamage,isRight,2);
                }
                break;
        }
    }

    public void SetPlayerHp(PlayerHp _value)
    {
        playerHp = _value;
        playerHp.SetPlayerHp(PlayerCurHp);
    }
    public void TriggerEnter(hitType _Type, Collider2D _collision)
    {
        if (_Type == hitType.WallCheck && _collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            wallStep = true;
        }
        else if (_Type == hitType.HitCheck)//triggerenter 함수로 하게될경우 히트박스 스크립트에서 물체를 확인해서 값을 가져오는게 아닌 플레이어 본인이 체크하게 되기 때문에 생각한대로 기능이 작동하지 않는다.
        {
            if (_collision.gameObject.tag == "Enemy")//맞닿은 태그가 적일경우
            {
                if (dash == true) return;
                Enemy obj = _collision.gameObject.GetComponent<Enemy>();
                obj.Hit(1, true);
            }
            else if (_collision.gameObject.tag == "Item")//맞닿은 태그가 아이템일 경우
            {
                Destroy(_collision.gameObject);
                Count += 1;
                itemUi.SetItemGet(Count);
            }
        }
    }

    public void TriggerExit(hitType _Type, Collider2D _collision)
    {
        if (_Type == hitType.WallCheck && _collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            wallStep = false;
        }
    }
}
