using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SellTeamMember : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler

{
    private TeamManager container;
    private Text txt;
    [SerializeField] private float ghostTime = .2f;
    private float _ghostTime;
    private bool startGhostTime = false;
    ////////////////////////////////////////////////////////////////////////////
    private void Start()
    {
        container = TeamManager.instance;
        txt = GetComponentInChildren<Text>();
    }
    private void Update()
    {
        if (startGhostTime)
        {
            if (_ghostTime < 0) { txt.text = "-- Party --"; }
            else                { _ghostTime -= Time.deltaTime; }
        }
    }
    ////////////////////////////////////////////////////////////////////////////
    public void OnPointerEnter(PointerEventData eventData)
    {
        GameObject objectBeingDragged = container.objectBeingDragged;
        if (objectBeingDragged != null)
        {
            startGhostTime = false;
            Member member = objectBeingDragged.GetComponent<Member>();
            txt.text = "-- Sell: " + member.GetHero().GetSellPrice() + " --";
            member.SetSell(true);
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        startGhostTime = true;
        _ghostTime = ghostTime;

        GameObject objectBeingDragged = container.objectBeingDragged;
        if(objectBeingDragged != null)
        { 
            objectBeingDragged.GetComponent<Member>().SetSell(false);
        }
    }
}
