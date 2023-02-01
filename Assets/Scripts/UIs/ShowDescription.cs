using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShowDescription : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private Hero heroScript = null;
    private HeroClass heroClassScript = null;
    public void SetHeroScript(Hero hero) { heroScript = hero; }
    public void SetHeroClassScript(HeroClass heroClass) { heroClassScript = heroClass; }

    ////////////////////////////////////////////////////////////////////////////
    private void Start()
    {
        Member member = GetComponentInParent<Member>();
        if (member != null) { heroScript = member.GetHero(); }
    }

    ////////////////////////////////////////////////////////////////////////////
    public void OnPointerDown(PointerEventData eventData)
    {
        if (heroScript != null) 
        { 
            TeamManager.instance.ShowDesc(heroScript.name, heroScript.GetDescription()); 
        }
        else if(heroClassScript != null)
        {
            TeamManager.instance.ShowDesc(heroClassScript.chosenClass.ToString(), heroClassScript.classDescription);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        TeamManager.instance.closeDesc();
    }
}
