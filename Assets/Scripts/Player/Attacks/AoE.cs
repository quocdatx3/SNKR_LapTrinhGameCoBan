using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AoE : MonoBehaviour
{
    [SerializeField] private float lifeTime = 0.1f;
    private float _lifeTime;
    private float damage;
    private string specialFX = "";
    private bool isEnemy = false;

    private void OnEnable()
    {
        _lifeTime = lifeTime;
    }
    public void SetVariables(float dm, Color color, float aoeSize, string fx = "") 
    {
        isEnemy = false;

        //raw variables
        damage = dm;
        transform.localScale = new Vector3(aoeSize, aoeSize, 1);

        //color
        color.a = .5f;
        GetComponentInChildren<SpriteRenderer>().color = color;
        
        //special fx
        specialFX = fx;
    }
    public void SetIsEnemy() { isEnemy = true; }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.tag)
        {
            case "Hero":
                if (!isEnemy) // if hero is using thi aoe
                {
                    if (specialFX.Equals("heal_allies"))
                    {
                        collision.GetComponent<Hero>().TakeDamage(-damage / 2);
                    }
                }
                else         // if an enemy is using this aoe
                {
                    collision.GetComponent<Hero>().TakeDamage(damage / 2);
                }
                break;
            case "Enemy":
                collision.GetComponent<Enemy>().TakeDamage(damage);
                break;
        }
    }

    private void Update()
    {
        if (GameManager.waveStart)
        {
            if (_lifeTime <= 0) { gameObject.SetActive(false); }
            else                { _lifeTime -= Time.deltaTime; }
        }
        else if (GameManager.inShop) { gameObject.SetActive(false); }
    }
}
