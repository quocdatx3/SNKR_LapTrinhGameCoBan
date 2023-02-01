using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField] private float lifeTime = 10f;
    private float _lifeTime;
    private float damage;
    private float aoeSize;
    private Color color;

    private void OnEnable()
    {
        _lifeTime = lifeTime;
    }
    public void SetVariables(float dmg, Color color, float aoeSize)
    {
        //raw variables
        damage = dmg * GameManager.instance.GetSummonerDmgIncre();
        this.aoeSize = aoeSize;

        //color
        this.color = color;
        GetComponent<SpriteRenderer>().color = color;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy")) { Attack(); }
    }

    private void Update()
    {
        if (GameManager.waveStart)
        {
            if (_lifeTime <= 0) { Attack(); }
            else                { _lifeTime -= Time.deltaTime; }
        }
        else if(GameManager.inShop) { Attack(); }
    }

    private void Attack()
    {
        GameObject aoe = ObjectsPooling.instance.FromAtkPool("aoe");
        aoe.transform.position = transform.position;
        aoe.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 90f));
        aoe.GetComponent<AoE>().SetVariables(damage, color, aoeSize);
        aoe.SetActive(true);

        gameObject.SetActive(false);
    }
}
