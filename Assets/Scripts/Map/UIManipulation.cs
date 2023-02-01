using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManipulation : MonoBehaviour
{
    //static variale for accessibility
    public static UIManipulation instance;
    private void Awake() { instance = this; }

    ////////////////////////////////////////////////////////////////////////////
    //functions for operating UI
    public void OpenUI(GameObject UI) { UI.SetActive(true); }
    public void CloseUI(GameObject UI) { UI.SetActive(false); }
}
