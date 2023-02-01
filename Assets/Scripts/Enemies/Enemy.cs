using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    private Rigidbody2D rb;

    [Header("Enemy Basic Stats")]
    [SerializeField] [Range(0, 50)] private float defense = 10;
    [SerializeField] private float curHp = 20;
    [SerializeField] private float speed = 250;
    [SerializeField] private int damage = 0;
    [SerializeField] private float atkRange = 0f;
    [SerializeField] private float atkCD = 1f;

    [Header("Enemy Extra Stats")]
    [SerializeField] private Color color = Color.red;
    [SerializeField] private bool isBoss = false;
    [SerializeField] [Range(0, 100)] private int dropMoneyChance = 10;

    ///////////////////////////////////////////////////////////////////////////
    private float _atkCD;
    private bool inAttackrange = false;
    private bool isKnockback = false;
    private float knockbackForce = 1f;
    private GameObject target;
    private Transform UIHolder;
    private Slider slider;
    private Slider bossSlider;

    ///////////////////////////////////////////////////////////////////////////
    public GameObject GetCurrentTarget() { return target; }
    public float GetDmg() { return damage; }
    public float GetSpeed() { return speed; }
    public void SetSpeed(float spd) { speed = spd; }
    public Color GetColor() { return color; }
    public float GetKnockbackForce() { return knockbackForce; }
    public bool GetIsKnockback() { return isKnockback; }
    public void SetBossSlider(Slider hpSlider) { 
        bossSlider = hpSlider;
        bossSlider.maxValue = curHp;
        bossSlider.value = curHp;
    }

    ///////////////////////////////////////////////////////////////////////////
    private void Awake()
    {
        GetComponent<SpriteRenderer>().color = color;

        UIHolder = transform.GetChild(0);
        slider = UIHolder.GetComponentInChildren<Slider>();
        if (!isBoss)
        {
            EnemiesManager.enemies.Add(gameObject);
            slider.maxValue = curHp;
            slider.value = curHp;
            UIManipulation.instance.CloseUI(slider.gameObject);
        }
        else
        {
            EnemiesManager.eliteEnemy = gameObject;
            slider.maxValue = atkCD;
            slider.value = _atkCD;
        }

        rb = GetComponent<Rigidbody2D>();
    }
   
    private void RotaionToTarget()
    {
        Vector2 relative = target.transform.position - transform.position;
        float angle = Mathf.Atan2(relative.y, relative.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90f));
        UIHolder.localRotation = Quaternion.Euler(new Vector3(0, 0, -angle + 90f));
    }

    ///////////////////////////////////////////////////////////////////////////
    //Overridable functions
    protected virtual void Attack() { }
    protected virtual void OnDestroy() { }
    protected virtual void Update()
    {
        UpdateNearestHero();
        if (GameManager.waveStart && target != null)
        {
            RotaionToTarget();
            if (inAttackrange)
            {
                if (_atkCD <= 0) {  Attack(); _atkCD = atkCD; }
                else             { _atkCD -= Time.deltaTime;  }

                if(isBoss)       { slider.value = _atkCD; }
            }
            if (!isKnockback) { rb.velocity = transform.up * speed * Time.deltaTime; }
        }
        else if (rb.velocity != Vector2.zero) { rb.velocity = Vector2.zero; }
    }

    ///////////////////////////////////////////////////////////////////////////
    public void UpdateNearestHero()
    {
        GameObject currentNearest = null;
        float distance = Mathf.Infinity;

        List<GameObject> snakeBodies = SnakeManager.instance.GetSnakeBody();
        foreach (GameObject hero in snakeBodies)
        {
            if (hero != null)
            {
                float _distance = (transform.position - hero.transform.position).magnitude;
                if (_distance < distance)
                {
                    distance = _distance;
                    currentNearest = hero;
                }
            }
        }
        target = currentNearest;
        if (distance < atkRange) { inAttackrange = true; }
        else                     { inAttackrange = false; }
    }
    ///////////////////////////////////////////////////////////////////////////
    public void TakeDamage(float dmg)
    {
        float sumOfDefense = GameManager.instance.GetEnemyDefense() + defense;
        float dmgTakeMultiplier = (100 - sumOfDefense) / 100;
        curHp -= dmg * dmgTakeMultiplier;

        if (!slider.gameObject.activeInHierarchy)
        {
            UIManipulation.instance.OpenUI(slider.gameObject);
            StartCoroutine(HideHealthSlider());
        }

        if (!isBoss) { slider.value = curHp; }
        else         { bossSlider.value = curHp; }

        if (curHp <= 0)
        {
            if (!isBoss) { EnemiesManager.enemies.Remove(gameObject); }
            else         { EnemiesManager.eliteEnemy = null; }

            if (Random.Range(0, 100) < dropMoneyChance) //spawn money
            {
                ObjectsPooling.instance.PlaceParticle("moneyDrop", transform.position);
                GameManager.instance.SpawnCoin(transform.position);
            }

            Destroy(gameObject);
        }
    }
    private IEnumerator HideHealthSlider()
    {
        yield return new WaitForSeconds(.75f);
        UIManipulation.instance.CloseUI(slider.gameObject);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Hero"))
        {
            Hero hero = collision.gameObject.GetComponent<Hero>();
            hero.TakeDamage(damage);
            TakeDamage(hero.GetContactDmg());

            StartCoroutine(Knockback(collision.transform.parent, hero.GetKnockbackDmg()));
        }
        else if(isKnockback)
        {
            switch (collision.gameObject.tag)
            {
                case "WallT":
                case "WallB":
                    StartCoroutine(Knockback(collision.transform.parent, knockbackForce, Vector3.up));
                    break;
                case "WallL":
                case "WallR":
                    StartCoroutine(Knockback(collision.transform.parent, knockbackForce, Vector3.right));
                    break;
            }

            TakeDamage(knockbackForce);
        }
    }
    public IEnumerator Knockback(Transform obj, float force, Vector3 normalBounce = default)
    {
        isKnockback = true;
        knockbackForce = force;

        Vector2 direction;
        if (normalBounce != default) { direction = Vector3.Reflect(transform.up, normalBounce).normalized;  }
        else                         { direction = (transform.position - obj.position).normalized; }
        rb.AddForce(direction * force, ForceMode2D.Impulse);

        yield return new WaitUntil(() => {
            if (rb.velocity.magnitude <= 2.5f) { return true; }
            else                               { return false; }
        });

        knockbackForce = 1f;
        isKnockback = false;
    }

    ///////////////////////////////////////////////////////////////////////////
    //Gizmos
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = color;
        Gizmos.DrawWireSphere(transform.position, atkRange);
    }
}