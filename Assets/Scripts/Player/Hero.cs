using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hero : MonoBehaviour
{
    private enum Target { enemy, player };
    [Header("Hero Class")]
    [SerializeField] private List<HeroClass> classes = new List<HeroClass>();
    [SerializeField] private int heroCost = 1;
    [SerializeField] private string heroDescription = "Empty heroDescription";
    private int sellPrice;

    ////////////////////////////////////////////////////////////////////////////
    public void SetHeroSellPrice(int count)
    {
        switch (count)
        {
            case 1: sellPrice = heroCost; break;
            case 2: sellPrice = heroCost * 2; break;
            case 3: sellPrice = heroCost * 2; break;
            case 4: sellPrice = heroCost * 3; break;
            case 5: sellPrice = heroCost * 4; break;
            case 6: sellPrice = heroCost * 4; break;
            case 7: sellPrice = heroCost * 5; break;
            case 8: sellPrice = heroCost * 6; break;
            case 9: sellPrice = heroCost * 6; break;
        }
    }
    public int GetSellPrice() { return sellPrice; }

    ////////////////////////////////////////////////////////////////////////////
    public List<HeroClass> GetClasses() { return classes; }
    public int GetHeroCost() { return heroCost; }
    public string GetDescription() { return heroDescription; }
    public void SetHeroLvl(int lvl) { SetStatToLvl(lvl); }
    public Color GetHeroColor() { return classes[0].heroColor; }

    ///////////////////////////////////////////////////////////////////////////
    [Header("Gameplay Stats")]
    [SerializeField] [Range(0, 50)] private float defense = 10f;
    [SerializeField] private float damage = 5f;
    [SerializeField] private float contactDamage = 5f;
    [SerializeField] private float knockbackDamage = 7f;
    [SerializeField] private float atkRange = 5f;
    [SerializeField] private float atkCD = 1f;
    private float _atkCD;
    private Slider cdSlider;
    private float maxHp = 100f;
    private float curHp;
    private Slider healthSlider;
    private List<GameObject> currentTargets = new List<GameObject>();
    private Transform pivot;

    ///////////////////////////////////////////////////////////////////////////
    public float GetHp() { return curHp; }
    public float GetDmg() { return damage; }
    public float GetContactDmg() { return contactDamage; }
    public float GetKnockbackDmg() { return knockbackDamage; }
    public List<GameObject> GetCurrentTargets() { return currentTargets; }
    public void SetPivot(Transform pi) { pivot = pi; }
    public Transform GetPivot() { return pivot; }

    ///////////////////////////////////////////////////////////////////////////
    //Overridable functions
    protected virtual void Start()
    {
        //Set GameObject env
        healthSlider = transform.GetChild(1).GetComponent<Slider>();
        
        //Set hero variables
        curHp = maxHp;
        _atkCD = atkCD;
        contactDamage *= GameManager.instance.GetContactDmgIncre();

        //Set color
        if (PlayerPrefs.GetInt("CD") == 1 && !SnakeManager.instance.forcedStart)
        {
            SetEnabledCDSlider(true);
        }
        GetComponentInChildren<SpriteRenderer>().color = classes[0].heroColor;  //body
        healthSlider.transform.GetChild(1).GetChild(0).GetComponent<Image>().color = new Color(.32f, .95f, .25f); // change color of health slider
    }
    protected virtual void Update()
    {
        if (GameManager.instance != null && GameManager.waveStart)
        {
            if (_atkCD <= 0 && currentTargets.Count > 0)
            {
                 Attack();
                _atkCD = atkCD;
            }
            else { _atkCD -= Time.deltaTime; }

            if (cdSlider != null) { cdSlider.value = _atkCD; }
        }
    }
    protected virtual void Attack() { currentTargets.Clear(); }
    protected virtual void OnDmgTaken(float dmgTaken) { }
    ///////////////////////////////////////////////////////////////////////////
    public void SpawnPivot() { gameObject.AddComponent<BarrelRotation>(); }
    public void TakeDamage(float dmg)
    {
        bool dealDmg = false;
        if (dmg > 0)
        {
            float sumOfDefense = GameManager.instance.GetTeamDefense() + defense;
            float dmgTakeMultiplier = (100 - sumOfDefense) / 100;
            curHp -= dmg * dmgTakeMultiplier;
            dealDmg = true;

            OnDmgTaken(dmg * dmgTakeMultiplier);
        }
        else if(curHp <= GameManager.instance.GetHealThreshold())
        {
            curHp -= dmg;
            dealDmg = true;
        }
        
        if (!healthSlider.gameObject.activeInHierarchy && dealDmg)
        {
            UIManipulation.instance.OpenUI(healthSlider.gameObject);
            StartCoroutine(HideHealthSlider());
        }

        if (curHp > maxHp) { curHp = maxHp; }
        else if (curHp <= 0)
        {
            ObjectsPooling.instance.PlaceParticle("heroDeath", transform.position);
            AudioManager.instance.Play("hero_death");
            Destroy(gameObject);
        }

        healthSlider.value = curHp;
    }
    private IEnumerator HideHealthSlider()
    {
        yield return new WaitForSeconds(.75f);
        UIManipulation.instance.CloseUI(healthSlider.gameObject);
    }

    private void SetStatToLvl(int lvl)
    {
        if(lvl == 2)
        {
            foreach(HeroClass _class in classes)
            {
                defense *= _class.lvl_mutiplier_def;
                damage *= _class.lvl_mutiplier_dmg;
                atkRange *= _class.lvl_mutiplier_range;
                atkCD *= _class.lvl_mutiplier_cooldown;
            }
        }

        if(lvl == 3)
        {
            foreach (HeroClass _class in classes)
            {
                defense *= _class.lvl_mutiplier_def * _class.lvl_mutiplier_def;
                damage *= _class.lvl_mutiplier_dmg * _class.lvl_mutiplier_dmg;
                atkRange *= _class.lvl_mutiplier_range * _class.lvl_mutiplier_range;
                atkCD *= _class.lvl_mutiplier_cooldown * _class.lvl_mutiplier_cooldown;
            }
        }
    }
    ///////////////////////////////////////////////////////////////////////////
    public void GetNearestEnemy()
    {
        if (EnemiesManager.enemies.Count > 0)
        {
            GameObject currentNearestEnemy = null;
            float distance = Mathf.Infinity;

            List<GameObject> enemies = new List<GameObject>();
            enemies.AddRange(EnemiesManager.enemies);
            if (EnemiesManager.eliteEnemy != null) { enemies.Add(EnemiesManager.eliteEnemy); }

            foreach (GameObject enemy in enemies)
            {
                if (enemy != null)
                {
                    float _distance = (transform.position - enemy.transform.position).magnitude;
                    if (_distance < distance)
                    {
                        distance = _distance;
                        currentNearestEnemy = enemy;
                    }
                }
            }
            if (distance < atkRange)
            {
                if (currentTargets.Count > 0) { currentTargets[0] = currentNearestEnemy; }
                else { currentTargets.Add(currentNearestEnemy); }
            }
        }
    }
    public void GetRandomEnemy()
    {
        if(EnemiesManager.enemies.Count > 0)
        {
            List<GameObject> enemies = new List<GameObject>();
            enemies.AddRange(EnemiesManager.enemies);
            if (EnemiesManager.eliteEnemy != null) { enemies.Add(EnemiesManager.eliteEnemy); }

            int i = 0;
            while(i < EnemiesManager.enemies.Count)
            {
                if(EnemiesManager.enemies[i] == null) { EnemiesManager.enemies.RemoveAt(0); }
                else                                  { i++; }
            }
            int randomIndex = Random.Range(0, enemies.Count);

            if (currentTargets.Count > 0) { currentTargets[0] = EnemiesManager.enemies[randomIndex]; }
            else                          { currentTargets.Add(EnemiesManager.enemies[randomIndex]); }
        }
    }

    public void GetLowestHPHero()
    {
        GameObject currentLowestHpHero = null;

        float health = maxHp;
        List<GameObject> snakeBodies = SnakeManager.instance.GetSnakeBody();
        foreach (GameObject hero in snakeBodies)
        {
            if (hero != null)
            {
                float _health = hero.GetComponent<Hero>().GetHp();
                if (_health < health)
                {
                    health = _health;
                    currentLowestHpHero = hero;
                }
            }
        }
        if (health < GameManager.instance.GetHealThreshold())
        {
            if (currentTargets.Count > 0) { currentTargets[0] = currentLowestHpHero; }
            else                          { currentTargets.Add(currentLowestHpHero); }
        }
    }
    public void GetAllHero()
    {
        List<GameObject> snakeBodies = SnakeManager.instance.GetSnakeBody();
        foreach (GameObject hero in snakeBodies)
        {
            if (hero != null) { currentTargets.Add(hero); }
        }
    }

    ///////////////////////////////////////////////////////////////////////////
    //Gizmos
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = classes[0].heroColor;
        Gizmos.DrawWireSphere(transform.position, atkRange);
    }

    public void SetEnabledCDSlider(bool state)
    {
        if(cdSlider == null)
        {
            cdSlider = transform.GetChild(2).GetComponent<Slider>();
            cdSlider.maxValue = atkCD;  //set max value
            cdSlider.transform.GetChild(0).GetComponent<Image>().color = classes[0].heroColor; //change color of cooldown slider
        }
        if (atkCD != -10) { cdSlider.gameObject.SetActive(state); }
    }
}