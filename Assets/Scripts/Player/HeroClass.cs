using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new Class", menuName = "Classes")]
public class HeroClass : ScriptableObject
{
    public enum Classes { Mage, Warrior, Healer, Rogue, Summoner, Enforcer }
    public Classes chosenClass;
    public Color heroColor;
    public Sprite classImage;
    public int minNumberNeededForPassive = 3;
    public int maxNumberNeededForPassive = 6;
    public string classDescription = "Empty";

    public float lvl_mutiplier_def = 2f;
    public float lvl_mutiplier_dmg = 1.5f;
    public float lvl_mutiplier_range = 1.2f;
    public float lvl_mutiplier_cooldown = 0.9f;
}
