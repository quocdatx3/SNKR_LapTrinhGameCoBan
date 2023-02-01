using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SetupMainMenuButton : MonoBehaviour
{
    [SerializeField] private Button playButton = null;
    [SerializeField] private Button optionButton = null;
    [SerializeField] private GameObject optionPanel = null;
    [SerializeField] private Button quitButton = null;

    private void Start()
    {
        playButton.onClick.AddListener(() => { SceneChanger.instance.LoadThisScene(1); });
        optionButton.onClick.AddListener(() => { UIManipulation.instance.OpenUI(optionPanel); });
        quitButton.onClick.AddListener(() => { SceneChanger.instance.Quit(); });
    }

    public void OnPointerEnter(Animator btn)
    {
        btn.SetBool("Enter", true);
    }
    public void OnPointerExit(Animator btn)
    {
        btn.SetBool("Enter", false);
    }
}
