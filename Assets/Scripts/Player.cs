using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.ReorderableList;
using UnityEngine;

public class Player : MonoBehaviour
{
    public enum hitType
    {
        WallCheck,
        HitCheck,
    }
    [Header("플레이어 데이터")]
    [SerializeField] private float MoveSpeed = 5;
    [SerializeField] private float gravity = 9.81f;
    [SerializeField] private float JumpForce = 5;
    [SerializeField] private Transform layerDynamic;
    [SerializeField] private Sprite sprHurt;
    [SerializeField] private float PlayerMaxHp = 3;
    [SerializeField] private float PlayerCurHp;
    private SpriteRenderer sr;
    private Sprite sprDefault;
    BoxCollider2D boxCollider2D;
    private Camera maincam;
    private Animator anim;
    Rigidbody2D rigid;
    Vector3 moveDir;
    [SerializeField]private bool isGround = false;
    private float verticalVelocity;
    private bool isJump = false;

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
    [SerializeField] private float BulletDamage = 0.0f;
    [SerializeField, Range(0.1f, 1.0f)] private float TimeShoot = 0.5f;

    [Header("프리팹")]
    [SerializeField] GameObject objBullet;
    private void OnDrawGizmos()
    {
        if (boxCollider2D != null)
        {
            Gizmos.color = Color.red;
            Vector3 pos = boxCollider2D.bounds.center - new Vector3(0, 0.1f, 0);
            Gizmos.DrawWireCube(pos, boxCollider2D.bounds.size);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (dash == true) return;
        if(collision.gameObject.tag == "Enemy")
        {
            Hit(1.0f);
        }
    }

    public void Hit(float _damage)
    {
        PlayerCurHp -= _damage;
        if (PlayerCurHp <= 0)
        {
            Destroy(gameObject);
            dashEffect.enabled = false;
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
    }
    void Start()
    {
        maincam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
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
        if(moveDir.x < 0 && transform.localScale.x != -1)
        {
            Vector3 scale = transform.localScale;
            scale.x = -1;
            transform.localScale = scale;
        }
        else if(moveDir.x > 0 && transform.localScale.x != 1)
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
       if(isGround == false)
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
        if(dash == true) return;
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
            if(verticalVelocity < -10.0f)
            {
                verticalVelocity = -10f;
            }
        }
        else
        {
            verticalVelocity = 0f;
        }

        if(isJump == true)
        {
            isJump = false;
            verticalVelocity = JumpForce;
        }
        rigid.velocity = new Vector2(rigid.velocity.x, verticalVelocity);
    }

    private void checkDoStepWallTimer()
    {
        if(doWallStepTimer == true)
        {
            wallStepTimer += Time.deltaTime;
            if(wallStepTimer >= wallStepTime)
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
            if(transform.localScale.x == 1)
            {
                rigid.velocity = new Vector2(10.0f, 0f);
                dashEffect.enabled = true;
            }
            else if(transform.localScale.x == -1)
            {
                rigid.velocity = new Vector2(-10.0f, 0f);
                dashEffect.enabled = true;
            }
        }
        else if(dash == true)
        {
            dashTime += Time.deltaTime;
            if(dashTime >= dashTimer)
            {
                dashTime = 0.0f;
                dashEffect.enabled = false;
                dashEffect.Clear();
                dash = false;
            }
        }
    }

    private void checkShoot()
    {
        if(Input.GetKeyDown(KeyCode.LeftControl))
        {
            shootBullet();
        }
    }

    private void shootBullet()
    {
        createBullet(trsShootpos.position,Vector3.zero);
    }

    private void createBullet(Vector3 _pos, Vector3 _rot)
    {
        GameObject obj = Instantiate(objBullet, _pos, Quaternion.Euler(_rot), layerDynamic);
        Bullet objsc = obj.GetComponent<Bullet>();
        bool isRight = true;
        if (transform.localScale.x == -1)
        {
            isRight = false;
        }
        objsc.SetDamege(true, BulletDamage, isRight);
    }



    public void TriggerEnter(hitType _Type, Collider2D _collision)
    {
        if(_Type == hitType.WallCheck && _collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            wallStep = true;
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
