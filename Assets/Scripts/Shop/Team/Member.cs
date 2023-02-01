using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Member : MonoBehaviour, IPointerEnterHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Hero hero;
    public Hero GetHero() { return hero; }
    public void SetHero(Hero newHero) { hero = newHero; }

    ////////////////////////////////////////////////////////////////////////////
    private GameObject heroGO;
    public GameObject GetHeroGO() { return heroGO; }
    public void SetHeroGO(GameObject newHero) { heroGO = newHero; }

    ////////////////////////////////////////////////////////////////////////////

    private int lvl = 1;
    public int GetHeroLvl() { return lvl; }
    public void SetHeroMaxLvl(int res) { lvl = res; }

    ////////////////////////////////////////////////////////////////////////////
    private int number;
    public void AddNumber() { number++; }
    public void DecrNumber() { number--; }
    public int GetNumber() { return number; }

    ////////////////////////////////////////////////////////////////////////////
    private bool sellThisMember = false;
    public void SetSell(bool res) { sellThisMember = res; }

    ////////////////////////////////////////////////////////////////////////////
    //click and drag 
    private TeamManager container;
    private Image backgroundImg;
    private float deltaX;

    void Start()
    {
        container = TeamManager.instance;
        backgroundImg = GetComponent<Image>();
    }

    ////////////////////////////////////////////////////////////////////////////
    public void OnBeginDrag(PointerEventData eventData)
    {
        deltaX = Camera.main.ScreenToWorldPoint(Input.mousePosition).x - transform.position.x;
        container.objectBeingDragged = gameObject;
    }
    public void OnDrag(PointerEventData eventData)
    {
        backgroundImg.enabled = false;
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector2(mousePosition.x - deltaX, mousePosition.y + .35f);
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        backgroundImg.enabled = true;
        container.ReArrangeTeamMembers();
        if (container.objectBeingDragged == gameObject)
        {
            if (sellThisMember) { ShopManager.instance.SellFunction(this); }
            container.objectBeingDragged = null;
        }
    }

    ////////////////////////////////////////////////////////////////////////////
    public void OnPointerEnter(PointerEventData eventData)
    {
        GameObject objectBeingDragged = container.objectBeingDragged;
        if (objectBeingDragged != null && objectBeingDragged != gameObject)
        {
            objectBeingDragged.transform.SetSiblingIndex(transform.GetSiblingIndex());
        }
    }

    ////////////////////////////////////////////////////////////////////////////
}
