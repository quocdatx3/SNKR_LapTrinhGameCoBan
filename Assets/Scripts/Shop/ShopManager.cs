using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    //static variable for easy accessibility
    public static ShopManager instance;
    private void Awake() { if (instance == null) { instance = this; } }

    ////////////////////////////////////////////////////////////////////////////
    [SerializeField] private List<GameObject> allHeros = null;

    [Header("Buying Hero")]
    [SerializeField] private GameObject shopHeroPrefab = null;
    [SerializeField] private Transform shopBuyRoot = null;
    [SerializeField] private int rerollStartCost = 2;
    [SerializeField] private int numberOfShopItems = 3;
    private HorizontalLayoutGroup buyRootLayoutGroup = null;
    private bool enableLock = false;
    private int rerollCost;

    [Header("Team/Party")]
    [SerializeField] private GameObject teamMemberHolderPrefab = null;
    [SerializeField] private GameObject teamMemberPrefab = null;
    [SerializeField] private Transform teamRoot = null;

    [Header("Misc")]
    [SerializeField] private Button[] settingButtons = null;
    [SerializeField] private GameObject optionPanel = null;
    [SerializeField] private Text coinCounter = null;
    [SerializeField] private Button rerollButton = null;
    [SerializeField] private Button lockButton = null;
    [SerializeField] private Button startWaveButton = null;
    [SerializeField] private GameObject gamePlayPanel = null;
    [SerializeField] private Text roundCounter = null;
    private Text rerollText = null;
    private List<GameObject> shopItems = new List<GameObject>();
    private bool firstOpen = true;
    private GameManager gm = null;

    ////////////////////////////////////////////////////////////////////////////
    private void Start()
    {
        //init variables
        gm = GameManager.instance;
        gm.SetGameObjectPanel(0, gamePlayPanel);
        gm.SetGameObjectPanel(1, gameObject);
        rerollText = rerollButton.GetComponentInChildren<Text>();
        buyRootLayoutGroup = shopBuyRoot.GetComponent<HorizontalLayoutGroup>();
        firstOpen = false;
        rerollCost = rerollStartCost;

        //add event to buttons
        foreach (Button settingButton in settingButtons)
        {
            settingButton.onClick.AddListener(() => { 
                UIManipulation.instance.OpenUI(optionPanel);
                if (!WaveSpawner.start) { WaveSpawner.instance.TimerManip(true); }
                GameManager.waveStart = false;
            });
        }
        rerollButton.onClick.AddListener(() => { Reroll(); });
        lockButton.onClick.AddListener(() => { ToggleLock(); });
        startWaveButton.onClick.AddListener(() => { StartWave(); });
        //update texts
        rerollText.text = "REROLL: " + rerollCost;
        UpdateCoinCounter();
        StockHero();
    }
    private void OnEnable()
    {
        if (!firstOpen)
        {
            //open shop
            startWaveButton.enabled = true;
            UpdateCoinCounter();
            if (!enableLock) { StockHero(); }
            rerollText.text = "REROLL: " + rerollCost;
            roundCounter.text = "LVL : " + gm.GetCurRound();
        }
    }

    ////////////////////////////////////////////////////////////////////////////
    public void UpdateCoinCounter(int value = -10) 
    {
        if (value != -10) { gm.AddCoinValue(value); }
        coinCounter.text = "SHOP - Gold: " + GameManager.instance.GetCoinValue(); 
    }

    ////////////////////////////////////////////////////////////////////////////
    public void RemoveStockHero()
    {
        //remove all hero in stock
        while (shopItems.Count > 0)
        {
            if (shopItems[0] != null) { Destroy(shopItems[0].gameObject); }
            shopItems.RemoveAt(0);
        }
    }
    public void RemoveStockHero(int index)
    {
        //remove specific hero in stock
        Destroy(shopItems[index].gameObject);
        shopItems[index] = null;
    }

    public void StockHero()
    {
        //select random heros -> add to list
        List<int> RandomHero = new List<int>();
        for (int i = 0; i < numberOfShopItems; i++)
        {
            int Rand = Random.Range(0, allHeros.Count);
            RandomHero.Add(Rand);
        }

        //add hero to shopBuyRoot
        //init variables
        GameObject shopItem; Transform trans, root;
        buyRootLayoutGroup.enabled = true;
        //remove existing heros before adding new one
        RemoveStockHero();

        //loop through number of items
        for (int i = 0; i < numberOfShopItems; i++)
        {
            //create physical gameobject
            shopItem = Instantiate(shopHeroPrefab, shopBuyRoot);    
            shopItems.Add(shopItem);
            //init needed variables
            trans = shopItem.transform.GetChild(0); 
            GameObject curHeroGO = allHeros[RandomHero[i]];
            Hero curHero = curHeroGO.GetComponent<Hero>();
            List<HeroClass> curHeroClasses = curHero.GetClasses();

            //top-half
            //change template to correct color, hero cost, hero name
            trans.GetChild(0).GetComponent<Image>().color = curHeroClasses[0].heroColor;
            trans.GetChild(0).GetComponentInChildren<Text>().text = curHero.GetHeroCost().ToString();
            trans.GetChild(1).GetComponent<Text>().text = curHero.name;
            //add listener to top-half, needed for buying hero
            int itemIndex = i; //needed
            trans.GetComponent<Button>().onClick.AddListener(() => { BuyFunction(curHeroGO, itemIndex); });
            trans.GetComponent<ShowDescription>().SetHeroScript(curHero);

            //lower-half
            //init needed variable 
            root = shopItem.transform.GetChild(1);
            //loop through all classes that hero have
            foreach (HeroClass _class in curHeroClasses)
            {
                //spawn physical template gameobject
                GameObject CounterGO = GameManager.SpawnCounterClassHolder(_class, root, true);
                CounterGO.GetComponent<ShowDescription>().SetHeroClassScript(_class);
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////
    public void Reroll()
    {
        //if player have more money than reroll cost
        if (gm.GetCoinValue() >= rerollCost)
        {
            UpdateCoinCounter(-rerollCost);
            StockHero();

            rerollCost += 2;
            rerollText.text = "REROLL: " + rerollCost;
        }
        else { TeamManager.instance.ShowDesc("WARNING!!!", "Don't have enough money to reroll.", TextAnchor.UpperCenter); }
    }
    public void ToggleLock() {
        //toggle enableLock boolean
        enableLock = !enableLock; 
        //change button inner text to reflect state
        if(enableLock) { lockButton.GetComponentInChildren<Text>().text = "UNLOCK"; } 
        else           { lockButton.GetComponentInChildren<Text>().text = "LOCK"; }
    }
    public void StartWave()
    {
        //only start wave when there are member in team
        if (TeamManager.instance.GetTeamMemberCount() == 0)
        {
            TeamManager.instance.ShowDesc("WARNING!!!", "Team need at least 1 Hero to continue.", TextAnchor.UpperCenter);
            return;
        }

        //if condition met
        rerollCost = rerollStartCost;
        startWaveButton.enabled = false;
        gm.WaveEnable();
    }
    private void BuyFunction(GameObject heroGO, int index)
    {
        //init needed variables
        Hero hero = heroGO.GetComponent<Hero>();
        buyRootLayoutGroup.enabled = false;

        //if team have less member than max number + player have more coin than hero cost
        if (gm.GetCoinValue() >= hero.GetHeroCost())
        {
            //if member is possible to add to team
            TeamManager.TeamData result = TeamManager.instance.AddToTeam(hero);
            //from result send back, add member to the team
            if (AddMemberToTeam(result, heroGO))
            {
                //shop related
                UpdateCoinCounter(-hero.GetHeroCost());
                RemoveStockHero(index);
                //get all classes that hero have -> update counter of said class
                List<HeroClass> curHeroClasses = result.hero.GetClasses();
                for (int i = 0; i < curHeroClasses.Count; i++) { ClassCounterManager.instance.UpdateCounter(curHeroClasses[i].chosenClass); }

                AudioManager.instance.Play("buy");
            }
        }
        else { TeamManager.instance.ShowDesc("WARNING !!!", "Don't have enough money to buy this hero.", TextAnchor.UpperCenter); }
    }
    public void SellFunction(Member member)
    {
        //remove member from team manager teamMembers list
        TeamManager.instance.RemoveFromTeam(member);
        UpdateCoinCounter(member.GetHero().GetSellPrice());
        //get all classes that hero have -> update counter of said class
        List<HeroClass> curHeroClasses = member.GetHero().GetClasses();
        for (int i = 0; i < curHeroClasses.Count; i++) { ClassCounterManager.instance.UpdateCounter(curHeroClasses[i].chosenClass); }
        
        //destroy physical gameobject in scene after removing it from lists
        Destroy(member.gameObject);

        AudioManager.instance.Play("sell");
    }
    ////////////////////////////////////////////////////////////////////////////
    private bool AddMemberToTeam(TeamManager.TeamData data, GameObject heroGO)
    {
        //init needed variables
        GameObject holder, member; Transform root;
        Member _member;
        List<Member> teamList = TeamManager.instance.GetTeamMember();
        List<HeroClass> curHeroClasses = data.hero.GetClasses();

        //output depend on data result send back from TeamManager
        switch (data.res)
        {
            case 1:     // add more member to existing 
                //loop through all members in team
                int length = TeamManager.instance.GetTeamMemberCount();
                for (int i = 0; i < length; i++)
                {
                    //find same hero
                    if(teamList[i].GetHero() == data.hero)
                    {
                        root = teamRoot.GetChild(i).transform;
                        //create physical gameobject -> change sprite to correct color
                        member = Instantiate(teamMemberPrefab, root);
                        member.GetComponent<Image>().color = curHeroClasses[0].heroColor;

                        //if member reach upgradable number (3/6/9)
                        if (data.totalMember % 3 == 0 )
                        {
                            if(data.totalMember < 9)
                            {
                                //get index of obj that need to be remove
                                int index = (data.totalMember / 3 - 1);
                                //remove unnecessary member
                                Destroy(root.GetChild(index + 1).gameObject);
                                Destroy(root.GetChild(index).gameObject);
                                //change member gameobject to correct level
                                member.GetComponentInChildren<Text>().text = "2";
                            }
                            else
                            {
                                //destroy the all existing members
                                for(int j = 0; j < 4; j++) { Destroy(root.GetChild(j).gameObject); }
                                //change member gameobject to correct level
                                member.GetComponentInChildren<Text>().text = "3";
                            }
                        }
                    }
                }
                return true;
            case 2:     // create new member
                //create physicall gameobject -> change sprite to correct color
                holder = Instantiate(teamMemberHolderPrefab, teamRoot);
                member = Instantiate(teamMemberPrefab, holder.transform);
                member.GetComponent<Image>().color = curHeroClasses[0].heroColor;

                //init new member variables -> add to TeamManager list
                _member = holder.GetComponent<Member>();
                _member.SetHero(data.hero);
                _member.SetHeroGO(heroGO);
                _member.GetHero().SetHeroSellPrice(1);
                _member.AddNumber();
                TeamManager.instance.AddMemberToList(_member);
                return true;
            default:
                return false;
        }
    }
}
