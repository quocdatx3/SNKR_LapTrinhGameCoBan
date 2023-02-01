using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public static SceneChanger instance;
    private Animator transitionAnim = null;
    private void Awake() { if (instance == null) { instance = this; } }
    private void Start()
    {
        transitionAnim = GameManager.instance.GetTransitionAnim();
    }

    //swap between scene using built-int scene management;
    public void LoadThisScene(int BuildIndex)
    {
        StartCoroutine(ChangeScene(BuildIndex));
    }

    private IEnumerator ChangeScene(int index)
    {
        transitionAnim.SetTrigger("Expand");

        yield return new WaitForSeconds(1f);
        SceneManager.LoadSceneAsync(index);

        transitionAnim.SetTrigger("Shrink");
    }
    //activate to quit app
    public void Quit(){ Application.Quit(); }
}
