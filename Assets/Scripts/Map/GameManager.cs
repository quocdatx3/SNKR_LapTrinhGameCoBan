using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //static variables for accessibility
    public static GameManager instance;
    public static bool waveStart = false;
    public static bool inShop = true;

    private void Awake()
    {
        //remove any other instance except the first one appear on scene
        if (instance == null) { instance = this; DontDestroyOnLoad(this); }
        else                  { Destroy(gameObject); }

        anim = GetComponentInChildren<Animator>();
    }

    [Header("Team Related")]
    [SerializeField] private int maxMember = 8;
    private List<GameObject> memberGOs = new List<GameObject>();

    [Header("Levels/Waves")]
    private GameObject shopPanel;
    private GameObject gamePlayPanel;
    private GameObject startRoundPanel;
    private GameObject endRoundPanel;
    private Text roundText;
    private GameObject congratText;
    private GameObject loseText;
    private GameObject endGamePanel;

    [Header("Shop")]
    [SerializeField] private int startGold = 1000;
    [SerializeField] private GameObject coinPrefab = null;
    private List<GameObject> coinGameObject = new List<GameObject>();
    private int extraCoins = 0;
    private int goldsOwned;
    private int curRound;

    //misc
    private Animator anim;

    //hero class upgrade
    private float healThreshold;
    private float teamDefense;
    private float enemyDefense;
    private float critChance;
    private float summonerDmgIncre;
    private float contactDmgIncre;

    ////////////////////////////////////////////////////////////////////////////
    public void SetHealThreshold(float res) { healThreshold = res; }
    public void SetTeamDefense(float res) { teamDefense = res; }
    public void SetEnemyDefense(float res) { enemyDefense = res; }
    public void SetCritChance(float res) { critChance = res; }
    public void SetSummonerDmgIncre(float res) { summonerDmgIncre = res; }
    public void SetContactDmgIncre(float res) { contactDmgIncre = res; }
    public float GetHealThreshold() { return healThreshold; }
    public float GetTeamDefense() { return teamDefense; }
    public float GetEnemyDefense() { return enemyDefense; }
    public float GetCritChance() { return critChance; }
    public float GetSummonerDmgIncre() { return summonerDmgIncre; }
    public float GetContactDmgIncre() { return contactDmgIncre; }

    ////////////////////////////////////////////////////////////////////////////
    private void Start()
    {
        SetTransitionColor(PlayerPrefs.GetInt("transit"));

        startRoundPanel = anim.transform.GetChild(0).gameObject;
        endRoundPanel = anim.transform.GetChild(1).gameObject;

        roundText = startRoundPanel.GetComponentInChildren<Text>();
        
        SetupStartingVariable();
    }
    public void SetupStartingVariable()
    {
        goldsOwned = startGold;
        waveStart = false;
        inShop = true;
        curRound = 0;

        healThreshold = 50f;
        teamDefense = 0f;
        enemyDefense = 0f;
        critChance = 0;
        summonerDmgIncre = 1;
        contactDmgIncre = 1;
    }
    ////////////////////////////////////////////////////////////////////////////
    //function for spawning class counter as a physical gameobject
    public static GameObject SpawnCounterClassHolder(HeroClass _class, Transform root, bool updateFirst = false)
    {
        //get static variable for easy use
        ShopClassManager classManager = ShopClassManager.instance;

        //create physical gameobject in scene -> change sprite to correct class icon
        GameObject classHolder = Instantiate(classManager.GetPrefab(0), root);
        Transform trans = classHolder.transform;
        trans.GetChild(0).GetChild(0).GetComponent<Image>().sprite = _class.classImage;

        //create counter for class
        //get spawn root
        Transform numberHolder = trans.GetChild(1);
        //change depend on class passive requirement
        numberHolder.GetComponent<GridLayoutGroup>().constraintCount = _class.minNumberNeededForPassive;
        //get script in counter holder -> change variable to correct class
        ClassCounter _counter = numberHolder.GetComponent<ClassCounter>();
        _counter.ChangeCounterClass(_class);

        //spawn individual counter gameobject -> add to a list for later use
        for (int i = 0; i < _class.maxNumberNeededForPassive; i++)
        {
            GameObject imgGo = Instantiate(classManager.GetPrefab(1), numberHolder);
            _counter.AddToCounterImages(imgGo);
        }
        
        //for update counter if needed
        if (updateFirst) { _counter.UpdateCount(); }
        //add the counter to a list for easy management
        ClassCounterManager.instance.AddToCounterList(_class.chosenClass, _counter);

        return classHolder;
    }

    ////////////////////////////////////////////////////////////////////////////
    //start and finished variables
    public void WaveEnable()
    {
        StartCoroutine(StartWave());
    }
    private IEnumerator StartWave()
    {
        curRound++;
        roundText.text = "Level: " + curRound + "/" + WaveSpawner.instance.GetMaxRound();

        ////////////////////////////////////////////////////////////////////////////
        UIManipulation.instance.OpenUI(startRoundPanel);
        anim.SetTrigger("Expand");              //start anim
        yield return new WaitForSeconds(1f);    //wait for anim to fully expand

        //close shop and open stage
        UIManipulation.instance.OpenUI(gamePlayPanel);
        UIManipulation.instance.CloseUI(shopPanel);
        inShop = false;
        yield return new WaitForSeconds(1.5f);

        anim.SetTrigger("Shrink");              //close anim
        yield return new WaitForSeconds(.8f);   //wait for anim to fully shrink
        UIManipulation.instance.CloseUI(startRoundPanel);

        ////////////////////////////////////////////////////////////////////////////
        /* Add hero to scene */
        List<Member> teamMemberList = TeamManager.instance.GetTeamMember();
        memberGOs.Clear();
        foreach (Member member in teamMemberList) { memberGOs.Add(member.GetHeroGO()); }
        SnakeManager.instance.AddToHeroList(memberGOs);

        waveStart = true;
        //wait till finish spawn all hero to turn on wave spawner
        yield return new WaitUntil(()=> {
            if (SnakeManager.instance.GetHeroListCount() == 0) { return true; }
            return false;
        });

        ////////////////////////////////////////////////////////////////////////////
        yield return new WaitForSeconds(.5f);
        WaveSpawner.instance.StartSpawning(curRound);
    }
    public IEnumerator FinishWave(int goldRoundGet)
    {
        UIManipulation.instance.OpenUI(congratText);
        yield return new WaitForSeconds(1f);

        // destroy existing snake
        yield return SnakeManager.instance.ClearSnakeBodies();
        waveStart = false;

        ////////////////////////////////////////////////////////////////////////////
        /* Change money Texts */
        int interest = Mathf.Min(5, goldsOwned / 5);
        int totalGet = goldRoundGet + extraCoins + interest;

        endRoundPanel.transform.GetChild(1).GetComponent<Text>().text = goldRoundGet + " + " + extraCoins;
        endRoundPanel.transform.GetChild(2).GetComponent<Text>().text = interest.ToString();
        endRoundPanel.transform.GetChild(3).GetComponent<Text>().text = totalGet.ToString();
        
        UIManipulation.instance.OpenUI(endRoundPanel);

        /* Change money owned */
        goldsOwned += totalGet;

        ////////////////////////////////////////////////////////////////////////////
        anim.SetTrigger("Expand");              //start anim
        yield return new WaitForSeconds(1f);    //wait for anim to fully expand

        // open shop and close stage
        RemoveAllCoinFromList();
        UIManipulation.instance.CloseUI(congratText);
        UIManipulation.instance.CloseUI(gamePlayPanel);
        UIManipulation.instance.OpenUI(shopPanel);
        inShop = true;
        yield return new WaitForSeconds(2.5f);

        anim.SetTrigger("Shrink");              //close anim
        yield return new WaitForSeconds(.8f);   //wait for anim to fully shrink
        UIManipulation.instance.CloseUI(endRoundPanel);
    }

    public IEnumerator ClearGame()
    {
        waveStart = false;

        UIManipulation.instance.OpenUI(congratText);
        AudioManager.instance.Play("win");
        ObjectsPooling.instance.PlaceParticle("win", new Vector3( 5.56f, -5.2f, 0));
        ObjectsPooling.instance.PlaceParticle("win", new Vector3(-5.56f, -5.2f, 0));
        ObjectsPooling.instance.PlaceParticle("win", new Vector3( 11.1f, 0.9f, 0), new Vector3(-48.793f, 90, -90));
        ObjectsPooling.instance.PlaceParticle("win", new Vector3(-11.1f, 0.9f, 0), new Vector3(-131.462f, 90, 90));
        yield return new WaitForSeconds(1f);

        // destroy existing snake
        yield return SnakeManager.instance.ClearSnakeBodies();

        UIManipulation.instance.OpenUI(endGamePanel);
        SetupStartingVariable();
    }

    public IEnumerator LoseGame()
    {
        waveStart = false;

        UIManipulation.instance.OpenUI(loseText);
        AudioManager.instance.Play("lose");
        ObjectsPooling.instance.PlaceParticle("lose", new Vector3( 3.4f, 5.2f, 0));
        ObjectsPooling.instance.PlaceParticle("lose", new Vector3(-3.4f, 5.2f, 0));
        yield return new WaitForSeconds(1f);

        UIManipulation.instance.OpenUI(endGamePanel);
        SetupStartingVariable();
    }

    ////////////////////////////////////////////////////////////////////////////
    //UI
    public Animator GetTransitionAnim() { return anim; }
    public void SetTransitionColor(int state) {
        switch (state) {
            case 0:
                GetComponentInChildren<Image>().color = Color.white;
                break;
            case 1:
                GetComponentInChildren<Image>().color = Color.gray;
                break;
            case 2:
                GetComponentInChildren<Image>().color = Color.black;
                break;
            case 3:
                GetComponentInChildren<Image>().color = new Color(.89f, .73f, .08f);
                break;
        }
    }
    public void SpawnCoin(Vector3 pos)
    {
        GameObject coin = Instantiate(coinPrefab, pos, Quaternion.identity);
        coinGameObject.Add(coin);
    }
    public void RemoveCoinFromList(GameObject coin)
    {
        coinGameObject.Remove(coin);
        Destroy(coin);
    }
    private void RemoveAllCoinFromList()
    {
        while(coinGameObject.Count > 0)
        {
            Destroy(coinGameObject[0]);
            coinGameObject.Remove(coinGameObject[0]);
        }
        extraCoins = 0;
    }
    public void ExtraPickupCoin() { extraCoins++; }
    public void AddCoinValue(int value) { goldsOwned += value; }
    public int GetCoinValue() { return goldsOwned; }
    public int GetCurRound() { return curRound + 1; }
    public void SetGameObjectPanel(int index, GameObject obj)
    {
        if (index == 0)
        {
            gamePlayPanel = obj;
            
            congratText = gamePlayPanel.transform.GetChild(1).gameObject;
            loseText = gamePlayPanel.transform.GetChild(2).gameObject;
            endGamePanel = gamePlayPanel.transform.GetChild(3).gameObject;

            Button[] buttons = endGamePanel.GetComponentsInChildren<Button>();
            buttons[0].onClick.AddListener(() => {
                SetupStartingVariable();
                SceneChanger.instance.LoadThisScene(1);
            });
            buttons[1].onClick.AddListener(() => {
                SetupStartingVariable();
                SceneChanger.instance.LoadThisScene(0);
            });
        }
        else { shopPanel = obj; }
    }

    ////////////////////////////////////////////////////////////////////////////
    //team
    public void SetMaxMember(int num) { maxMember = num; }
    public int GetMaxMember() { return maxMember; }

    ////////////////////////////////////////////////////////////////////////////
}