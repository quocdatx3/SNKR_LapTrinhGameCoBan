using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeamManager : MonoBehaviour
{
    //static variable for easy accessability
    public static TeamManager instance;
    private void Awake() { if (instance == null) { instance = this; } }
    //variable to show which object is being grab and drag
    public GameObject objectBeingDragged { get; set; } = null;

    ////////////////////////////////////////////////////////////////////////////
    //Show team member Description
    [SerializeField] private Transform memberDescUI = null;
    public void ShowDesc(string name, string desc, TextAnchor alignment = 0)
    {
        //change name text
        memberDescUI.GetChild(0).GetComponent<Text>().text = name;
        //change Description text
        Text descriptionText = memberDescUI.GetChild(1).GetComponent<Text>();
        descriptionText.text = desc;
        descriptionText.alignment = alignment;
        //enable the UI
        UIManipulation.instance.OpenUI(memberDescUI.gameObject);
    }
    public void closeDesc()
    {
        //disable the UI
        UIManipulation.instance.CloseUI(memberDescUI.gameObject);
    }

    ////////////////////////////////////////////////////////////////////////////
    //variable list needed to manage team
    private List<Member> teamMembers = new List<Member>();
    private List<classesData> teamClasses = new List<classesData>();
    [System.Serializable] public struct TeamData
    {
        public Hero hero;
        public int totalMember;
        public int res;
    }
    public TeamData AddToTeam(Hero hero)
    {
        //init varaibles
        TeamData data = new TeamData { hero = hero, res = 0 };
        List<HeroClass> curHeroClasses = hero.GetClasses();
        //upgrade existing memeber
        for (int i = 0; i < teamMembers.Count; i++)
        {
            //find correct hero
            Hero curHero = teamMembers[i].GetHero();
            if (curHero.name == hero.name)
            {
                //if team not max member
                if (teamMembers[i].GetNumber() < 9)
                {
                    //add new unit to member
                    teamMembers[i].AddNumber();
                    //refresh and add new number
                    int teamMemberNewNumber = teamMembers[i].GetNumber();
                    data.totalMember = teamMemberNewNumber;
                    //save and send data
                    if (teamMembers[i].GetNumber() == 9) { teamMembers[i].SetHeroMaxLvl(3); }
                    else if (teamMembers[i].GetNumber() >= 3) { teamMembers[i].SetHeroMaxLvl(2); }
                    else { teamMembers[i].SetHeroMaxLvl(1); }
                    curHero.SetHeroSellPrice(teamMemberNewNumber);
                    data.res = 1; return data;
                }
                else
                {
                    ShowDesc("WARNING !!!", "This hero is max lvl.", TextAnchor.UpperCenter);
                    data.res = 3; return data;
                }
            }
        }

        if (teamMembers.Count < GameManager.instance.GetMaxMember())
        {
            //add new Member if member is less than max member 
            //add new hero class to list
            ShopClassManager.instance.ModifyClassesInTeam(curHeroClasses, true);
            //if have class in list
            if (teamClasses.Count > 0)
            {
                //loop through all teamClasses
                for (int i = 0; i < curHeroClasses.Count; i++)
                {
                    bool isInTeam = false;
                    for (int j = 0; j < teamClasses.Count; j++)
                    {
                        //if class in class list add -> add unit
                        if (teamClasses[j].heroClass == curHeroClasses[i].chosenClass) { teamClasses[j].AddNumber(); isInTeam = true; }
                    }
                    //if class not in class list -> create new class
                    if (!isInTeam) { teamClasses.Add(new classesData(curHeroClasses[i].chosenClass, 1)); }
                }
            }
            //first class in list
            else { foreach (HeroClass _class in curHeroClasses) { teamClasses.Add(new classesData(_class.chosenClass, 1)); } }

            data.res = 2; return data;
        }
        else
        {
            ShowDesc("WARNING !!!", "Team have maximum number of heros.", TextAnchor.UpperCenter);
            data.res = 3; return data;
        }
    }
    public void RemoveFromTeam(Member member)
    {
        //init variables
        Hero hero = member.GetHero();
        List<HeroClass> curHeroClasses = hero.GetClasses();
        //remove hero class from list
        ShopClassManager.instance.ModifyClassesInTeam(curHeroClasses, false);

        //loop through all teamClasses
        if (teamClasses.Count > 0)
        {
            //loop through heroclasses and classes in list
            for (int i = 0; i < curHeroClasses.Count; i++)
            {
                for (int j = 0; j < teamClasses.Count; j++)
                {
                    //find correct class
                    if (teamClasses[j].heroClass == curHeroClasses[i].chosenClass)
                    { 
                        //remove unit -> if remove all unit, then remove from list
                        teamClasses[j].DecrNumber(); 
                        if (teamClasses[j].GetNumber() == 0) { teamClasses.RemoveAt(j); } 
                    }
                }
            }
        }
        //remove member from member list
        teamMembers.Remove(member);
    }
    ////////////////////////////////////////////////////////////////////////////
    public int GetTeamMemberCount()            { return teamMembers.Count; }
    public List<Member> GetTeamMember()        { return teamMembers; }
    public void AddMemberToList(Member member) { teamMembers.Add(member); }
    public int GetClassInTeamCount(HeroClass.Classes _class)
    {
        foreach (classesData data in teamClasses) 
        { 
            if (data.heroClass == _class) { return data.GetNumber(); } 
        }
        return -1;
    }
    public void ReArrangeTeamMembers() {
        teamMembers.Clear();
        for (int i = 0; i < transform.childCount; i++)
        {
            teamMembers.Add(transform.GetChild(i).GetComponent<Member>());
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
    }
}
