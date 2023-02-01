using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectsPooling : MonoBehaviour
{
    public static ObjectsPooling instance;
    private void Awake()
    {
        if(instance == null) { instance = this; DontDestroyOnLoad(this); }
        else                 { Destroy(gameObject); }
    }

    private GameObject obj;

    [Header("Attack Pool")]
    [SerializeField] private GameObject bullet = null;
    [SerializeField] private GameObject aoe = null;
    [SerializeField] private GameObject bomb = null;
    [SerializeField] private GameObject summon = null;
    //////////////////////////////////////////
    private List<GameObject> bulletPool;
    private List<GameObject> aoePool;
    private List<GameObject> bombPool;
    private List<GameObject> summonPool;

    [Header("Particle Pool")]
    [SerializeField] private GameObject heroDeath = null;
    [SerializeField] private GameObject moneyDrop = null;
    [SerializeField] private GameObject enemySpawn = null;
    [SerializeField] private GameObject win = null;
    [SerializeField] private GameObject lose = null;
    //////////////////////////////////////////
    private List<GameObject> heroDeathParticles;
    private List<GameObject> moneyDropParticles;
    private List<GameObject> enemySpawnParticles;
    private List<GameObject> winParticles;
    private List<GameObject> loseParticles;

    private void Start()
    {
        //open pool
        bulletPool = new List<GameObject>();
        aoePool = new List<GameObject>();
        bombPool = new List<GameObject>();
        summonPool = new List<GameObject>();

        heroDeathParticles = new List<GameObject>();
        moneyDropParticles = new List<GameObject>();
        enemySpawnParticles = new List<GameObject>();
        winParticles = new List<GameObject>();
        loseParticles = new List<GameObject>();
    }

    public GameObject FromAtkPool(string innerPool)
    {
        switch (innerPool)
        {
            case "bullet":
                for (int i = 0; i < bulletPool.Count; i++)
                {
                    if (!bulletPool[i].activeInHierarchy) { return bulletPool[i]; }
                }
                obj = Instantiate(bullet, transform.GetChild(0));
                bulletPool.Add(obj);
                return obj;
            case "aoe":
                for (int i = 0; i < aoePool.Count; i++)
                {
                    if (!aoePool[i].activeInHierarchy) { return aoePool[i]; }
                }
                obj = Instantiate(aoe, transform.GetChild(1));
                aoePool.Add(obj);
                return obj;
            case "bomb":
                for (int i = 0; i < bombPool.Count; i++)
                {
                    if (!bombPool[i].activeInHierarchy) { return bombPool[i]; }
                }
                obj = Instantiate(bomb, transform.GetChild(2));
                bombPool.Add(obj);
                return obj;
            case "summon":
                for (int i = 0; i < summonPool.Count; i++)
                {
                    if (!summonPool[i].activeInHierarchy) { return summonPool[i]; }
                }
                obj = Instantiate(summon, transform.GetChild(3));
                summonPool.Add(obj);
                return obj;
            default:
                return null;
        }
    }

    private GameObject FromParticlePool(string innerPool)
    {
        GameObject obj;
        switch (innerPool)
        {
            case "heroDeath":
                for (int i = 0; i < heroDeathParticles.Count; i++)
                {
                    if (!heroDeathParticles[i].activeInHierarchy) { return heroDeathParticles[i]; }
                }
                obj = Instantiate(heroDeath, transform.GetChild(4));
                heroDeathParticles.Add(obj);
                return obj;
            case "moneyDrop":
                for (int i = 0; i < moneyDropParticles.Count; i++)
                {
                    if (!moneyDropParticles[i].activeInHierarchy) { return moneyDropParticles[i]; }
                }
                obj = Instantiate(moneyDrop, transform.GetChild(4));
                moneyDropParticles.Add(obj);
                return obj;
            case "enemySpawn":
                for (int i = 0; i < enemySpawnParticles.Count; i++)
                {
                    if (!enemySpawnParticles[i].activeInHierarchy) { return enemySpawnParticles[i]; }
                }
                obj = Instantiate(enemySpawn, transform.GetChild(4));
                enemySpawnParticles.Add(obj);
                return obj;
            case "win":
                for (int i = 0; i < winParticles.Count; i++)
                {
                    if (!winParticles[i].activeInHierarchy) { return winParticles[i]; }
                }
                obj = Instantiate(win, transform.GetChild(4));
                winParticles.Add(obj);
                return obj;
            case "lose":
                for (int i = 0; i < loseParticles.Count; i++)
                {
                    if (!loseParticles[i].activeInHierarchy) { return loseParticles[i]; }
                }
                obj = Instantiate(lose, transform.GetChild(4));
                loseParticles.Add(obj);
                return obj;
            default:
                return null;
        }
    }
    public void PlaceParticle(string innerPool, Vector3 position, Vector3 rotation = default)
    {
        GameObject particle = FromParticlePool(innerPool);
        particle.transform.position = position;
        particle.transform.rotation = Quaternion.Euler(rotation);
        UIManipulation.instance.OpenUI(particle);
    }
}
