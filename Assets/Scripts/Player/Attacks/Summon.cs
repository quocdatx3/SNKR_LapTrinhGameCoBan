using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Summon : MonoBehaviour
{
    [SerializeField] private float lifeTime = 10f;
    private float _lifeTime;
    private float atkCD;
    private float _atkCD;
    private float damage;
    private float speed;
    private Color color;
    private string specialFX = "";

    private void OnEnable()
    {
        _lifeTime = lifeTime;
    }
    public void SetVariables(float dmg, Color color, float spd, float atkCD, string fx = "")
    {
        //raw variables
        damage = dmg * GameManager.instance.GetSummonerDmgIncre();
        speed = spd;
        this.atkCD = atkCD;

        //color
        this.color = color;
        GetComponent<SpriteRenderer>().color = color;

        //special fx
        specialFX = fx;
    }

    private void Update()
    {
        if (GameManager.waveStart)
        {
            transform.position += transform.right * speed * Time.deltaTime;

            if (_lifeTime <= 0) { gameObject.SetActive(false); }
            else                { _lifeTime -= Time.deltaTime; }

            if(_atkCD < 0) { Attack(); _atkCD = atkCD; }
            else           { _atkCD -= Time.deltaTime; }
        }
        else if (GameManager.inShop) { gameObject.SetActive(false); }
    }

    private void Attack()
    {
        switch (specialFX)
        {
            case "fire_bullet":
                for(int i = -1; i < 2; i++)
                {
                    GameObject bullet = ObjectsPooling.instance.FromAtkPool("bullet");
                    bullet.transform.position = transform.position;
                    bullet.transform.rotation = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z + i * 10f); ;
                    bullet.GetComponent<Bullet>().SetVariables(damage, color);
                    bullet.SetActive(true);
                }
                break;
        }
    }
}
