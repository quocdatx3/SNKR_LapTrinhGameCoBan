using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float lifeTime = 5f;
    [SerializeField] private float speed = 10f;
    private float _lifeTime;
    private float damage;
    private string specialFX = "";
    private Color color;
    private float aoeSize;
    private bool destroyOnContact;
    private bool isEnemy = false;

    private void OnEnable()
    {
        _lifeTime = lifeTime;
    }

    public void SetVariables(float dm, Color color, bool destroyOnContact = true, string fx = "", float aoeSize = 0)
    {
        isEnemy = false;

        //raw variables
        damage = dm;
        this.aoeSize = aoeSize;
        this.destroyOnContact = destroyOnContact;

        //color
        this.color = color;
        GetComponent<SpriteRenderer>().color = color;

        //special fx
        specialFX = fx;
    }
    public void SetIsEnemy() { isEnemy = true; } 

    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.tag)
        {
            case "Hero":
                if (isEnemy)
                {
                    Hero hero = collision.gameObject.GetComponent<Hero>();
                    hero.TakeDamage(damage);

                    if (specialFX.Equals("aoe_on_contact"))
                    {
                        GameObject aoe = ObjectsPooling.instance.FromAtkPool("aoe");
                        aoe.transform.position = transform.position;
                        aoe.transform.rotation = transform.rotation;

                        AoE _aoe = aoe.GetComponent<AoE>();
                        _aoe.SetVariables(damage, color, aoeSize);
                        _aoe.SetIsEnemy();

                        aoe.SetActive(true);
                    }

                    gameObject.SetActive(false);
                }
                break;
            case "Enemy":
                Enemy enemy = collision.gameObject.GetComponent<Enemy>();
                enemy.TakeDamage(damage);

                if (specialFX.Equals("aoe_on_contact"))
                {
                    GameObject aoe = ObjectsPooling.instance.FromAtkPool("aoe");
                    aoe.transform.position = transform.position;
                    aoe.transform.rotation = transform.rotation;

                    aoe.GetComponent<AoE>().SetVariables(damage, color, aoeSize);
                    aoe.SetActive(true);
                }

                if (destroyOnContact) { gameObject.SetActive(false); }
                break;
            case "WallT":
            case "WallB":
            case "WallL":
            case "WallR":
                gameObject.SetActive(false);
                break;
        }
    }
    
    private void Update()
    {
        if (GameManager.waveStart)
        {
            transform.position += transform.right * speed * Time.deltaTime;

            if(_lifeTime <= 0) { gameObject.SetActive(false); }
            else               { _lifeTime -= Time.deltaTime; }
        }
        else if (GameManager.inShop) { gameObject.SetActive(false); }
    }
}
