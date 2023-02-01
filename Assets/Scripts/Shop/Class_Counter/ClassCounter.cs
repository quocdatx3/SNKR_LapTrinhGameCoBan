using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClassCounter : MonoBehaviour
{
    private HeroClass counterClass = null;
    public void ChangeCounterClass(HeroClass newClass) { counterClass = newClass; }

    ////////////////////////////////////////////////////////////////////////////
    private List<Image> counterImages = new List<Image>();
    public void AddToCounterImages(GameObject imgGO) { counterImages.Add(imgGO.GetComponent<Image>()); }

    ////////////////////////////////////////////////////////////////////////////
    private Image symbol;
    private void Awake(){ symbol = transform.parent.GetChild(0).GetChild(0).GetComponent<Image>(); }

    ////////////////////////////////////////////////////////////////////////////
    public void UpdateCount()
    {
        //get number of hero that have counterClass in teamMembers
        int have = TeamManager.instance.GetClassInTeamCount(counterClass.chosenClass);
        //change color
        for (int i = 0; i < counterImages.Count; i++)
        {
            if(i < have) { counterImages[i].color = counterClass.heroColor; }
            else         { counterImages[i].color = Color.white; }
        }
        //change main color if reach minimum member
        if(have >= counterClass.minNumberNeededForPassive) { symbol.color = counterClass.heroColor; }
        else                                               { symbol.color = Color.white; }

        if(have % counterClass.minNumberNeededForPassive == 0)
        {
            int classStage = have / counterClass.minNumberNeededForPassive;
            switch(counterClass.chosenClass)
            {
                case HeroClass.Classes.Healer:
                    if (classStage == 1) { GameManager.instance.SetHealThreshold(65); }
                    if (classStage == 2) { GameManager.instance.SetHealThreshold(80); }
                    break;
                case HeroClass.Classes.Warrior:
                    if (classStage == 1) { GameManager.instance.SetTeamDefense(15); }
                    if (classStage == 2) { GameManager.instance.SetTeamDefense(30); }
                    break;
                case HeroClass.Classes.Mage:
                    if (classStage == 1) { GameManager.instance.SetEnemyDefense(-15); }
                    if (classStage == 2) { GameManager.instance.SetEnemyDefense(-30); }
                    if (classStage == 3) { GameManager.instance.SetEnemyDefense(-40); }
                    break;
                case HeroClass.Classes.Rogue:
                    if (classStage == 1) { GameManager.instance.SetCritChance(15); }
                    if (classStage == 2) { GameManager.instance.SetCritChance(30); }
                    break;
                case HeroClass.Classes.Summoner:
                    if (classStage == 1) { GameManager.instance.SetSummonerDmgIncre(15); }
                    if (classStage == 2) { GameManager.instance.SetSummonerDmgIncre(30); }
                    break;
                case HeroClass.Classes.Enforcer:
                    if (classStage == 1) { GameManager.instance.SetContactDmgIncre(30); }
                    if (classStage == 2) { GameManager.instance.SetContactDmgIncre(60); }
                    break;
            }
        }
    }
}
