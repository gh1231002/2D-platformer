using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("ÃÑ¾Ë¼¼ÆÃ")]
    [SerializeField] private float BulletSpeed;
    [SerializeField] private float TimeDestroy = 0.5f;
    private bool PlayerBullet = false;
    private float BulletDamage = 1.0f;
    private bool isRight;

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(PlayerBullet == true && collision.gameObject.tag=="Enemy")
        {
            Enemy enemySc = collision.gameObject.GetComponent<Enemy>();
            enemySc.Hit(BulletDamage);
            Destroy(gameObject );
        }
    }
    void Update()
    {
        int diretion = 1;
        if (isRight == false)
        {
            diretion = -1;
        }
        transform.position += transform.right * diretion * Time.deltaTime * BulletSpeed;
    }

    public void SetDamege(bool _isPlayer, float _damege, bool _isRight, float _speed = -1)
    {
        PlayerBullet = _isPlayer;
        BulletDamage = _damege;
        if (_speed != -1)
        {
            BulletSpeed = _speed;
        }
        isRight = _isRight;
        if (isRight == false)
        {
            Vector3 scale = transform.localScale;
            scale.x = -1;
            transform.localScale = scale;
        }
    }
}
