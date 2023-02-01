using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaveSpawner : MonoBehaviour
{
    public static WaveSpawner instance;
    public static bool start = false;
    private void Awake() { if (instance == null) { instance = this; } }

    [System.Serializable] private struct Spawnpoints
    {
        public string name;
        public List<Transform> points;

    }
    [System.Serializable] private struct Waves
    {
        [Range(0, 30)] public int specialSpawnChance;
        [Range (0, 90)] public int advSpawnChance;
    }
    [System.Serializable] private struct Rounds
    {
        public string name;
        public List<Waves> waves;
        public float timeBtwSpawn;
        public int goldGet;
    }

    [Header("For Other")]
    [SerializeField] private List<GameObject> allEnemies;
    [SerializeField] private List<GameObject> allBosses;

    [Header("Setup")]
    [SerializeField] private List<Spawnpoints> spawnpoints_basic = new List<Spawnpoints>();
    [SerializeField] private List<Spawnpoints> spawnpoints_adv = new List<Spawnpoints>();
    [SerializeField] private List<Rounds> wavePreset = new List<Rounds>(); 
    private readonly int starting_enemyCount = 9;
    private int curRound = 0;
    private int curWave = 0;
    private bool isSpawning = false;
    private bool eliteSpawn = false;
    private Spawnpoints spawnpointChoose;
    private GameObject enemyToSpawn;

    [Header("UI")]
    [SerializeField] private Text waveText;
    [SerializeField] private Slider bossSlider;
    private Animator[] anims;
    private Text countdownTimerText;

    private void Start()
    {
        anims = GetComponentsInChildren<Animator>();
        countdownTimerText = GetComponentInChildren<Text>();
    }

    public void StartSpawning(int curRound)
    {
        curWave = 0; 
        this.curRound = curRound - 1;
        StartTimer();
    }

    private void Update()
    {
        if (start && GameManager.waveStart)
        {
            if(EnemiesManager.enemies.Count == 0 && !isSpawning)
            {
                //final round
                if (curRound + 1 == wavePreset.Count)
                {
                    if (!eliteSpawn) 
                    {
                        waveText.text = "Kill the Boss! ";
                        //spawn elite
                        eliteSpawn = true;
                        StartCoroutine(SpawnElite());
                        UIManipulation.instance.OpenUI(bossSlider.gameObject);
                    }
                    else if (EnemiesManager.eliteEnemy == null)
                    {
                        StartCoroutine(GameManager.instance.ClearGame());
                    }
                    else
                    {
                        StartCoroutine(SpawnWave());
                    }
                    return;
                }

                //other rounds check specific round wave
                if (curWave < wavePreset[curRound].waves.Count)
                {
                    //spawn enemies
                    StartCoroutine(SpawnWave());
                }
                else
                {
                    start = false;
                    waveText.text = "";
                    StartCoroutine(GameManager.instance.FinishWave(wavePreset[curRound].goldGet));
                }
            }
        }
    }
    private IEnumerator SpawnElite()
    {
        isSpawning = true;
        //prep location to spawn
        spawnpoints_basic[0].points[0].rotation = Quaternion.Euler(0, 0, Random.Range(0, 270));
        spawnpoints_basic[0].points[0].GetComponent<Animator>().SetTrigger("start");
        yield return new WaitForSeconds(1.3f);

        //play particle
        ObjectsPooling.instance.PlaceParticle("enemySpawn", spawnpointChoose.points[0].position);

        //Spawning
        enemyToSpawn = allBosses[Random.Range(0, allBosses.Count)];
        GameObject boss = Instantiate(enemyToSpawn, spawnpoints_basic[0].points[0].position, Quaternion.identity, transform);
        boss.GetComponent<Enemy>().SetBossSlider(bossSlider);
        
        isSpawning = false;
    }
    private IEnumerator SpawnWave()
    {
        isSpawning = true;
        Waves thisWave = wavePreset[curRound].waves[curWave];

        //choose and prep location to spawn
        if (Random.Range(0, 100) < thisWave.advSpawnChance) { spawnpointChoose = spawnpoints_adv[Random.Range(0, spawnpoints_adv.Count)]; }
        else                                                { spawnpointChoose = spawnpoints_basic[Random.Range(0, spawnpoints_basic.Count)]; }
        
        for (int i = 0; i < spawnpointChoose.points.Count; i++)
        {
            spawnpointChoose.points[i].rotation = Quaternion.Euler(0, 0, Random.Range(0, 270));
            spawnpointChoose.points[i].GetComponent<Animator>().SetTrigger("start");
        }
        yield return new WaitForSeconds(1.3f);
        //check if elite still alive
        if(eliteSpawn && EnemiesManager.eliteEnemy == null) { isSpawning = false; yield break; }

        //play particle once
        for (int i = 0; i < spawnpointChoose.points.Count; i++)
        {
            ObjectsPooling.instance.PlaceParticle("enemySpawn", spawnpointChoose.points[i].position);
        }

        //Spawning
        int numOfEnemyToSpawn = starting_enemyCount + Random.Range(curWave, curWave + Random.Range(0, Mathf.FloorToInt((float)curRound / 8)));
        while (numOfEnemyToSpawn > 0)
        {
            for (int i = 0; i < spawnpointChoose.points.Count; i++)
            {
                yield return new WaitUntil(() => { return GameManager.waveStart; });

                //choose enemy
                if (Random.Range(0, 100) < thisWave.specialSpawnChance) { enemyToSpawn = allEnemies[Random.Range(1, allEnemies.Count)]; }
                else                                                    { enemyToSpawn = allEnemies[0]; }

                //choose position and spawn
                Vector3 position = spawnpointChoose.points[i].position + new Vector3(Random.Range(-.4f, .4f), Random.Range(-.4f, .4f));
                Instantiate(enemyToSpawn, position, Quaternion.identity, transform);

                //stop condition
                numOfEnemyToSpawn--;
                if (numOfEnemyToSpawn == 0) { break; }
            }

            yield return new WaitForSeconds(wavePreset[curRound].timeBtwSpawn);
        }

        //have finite wave if not boss wave
        if (!eliteSpawn)
        {
            curWave++;
            waveText.text = "Wave " + curWave + "/" + wavePreset[curRound].waves.Count + " ";
        }
        isSpawning = false;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //Timer
    private void StartTimer()
    {
        anims[0].SetTrigger("start");
        SetTimerText("3");
    }
    public void SetTimerText(string txt)
    {
        countdownTimerText.text = txt;
    }
    public void EndTimer()
    {
        // use in animation event
        start = true;
    }
    public void TimerManip(bool pause)
    {
        foreach(Animator anim in anims)
        {
            if (pause) { anim.speed = 0; }
            else       { anim.speed = 1; }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public float GetMaxRound() { return wavePreset.Count; }
}