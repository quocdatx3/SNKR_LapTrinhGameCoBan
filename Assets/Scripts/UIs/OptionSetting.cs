using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OptionSetting : MonoBehaviour
{
    private AudioManager audioM;

    [Header("Sliders")]
    [SerializeField] private Slider music = null;
	[SerializeField] private Slider sound = null;

    [Header("General Buttons")]
	[SerializeField] private Button resume = null;
    [SerializeField] private Button transition = null;
    [SerializeField] private Button control = null;
    [SerializeField] private Button arwOnSnk = null;
    [SerializeField] private Button cdOnSnk = null;

    [Header("Gameplay-Only Buttons")]
    [SerializeField] private Button mainMenu = null;
    [SerializeField] private Button restart = null;

    private void Start()
    {
        audioM = AudioManager.instance;

        UpdateDarkTransition(PlayerPrefs.GetInt("transit"));
        UpdateControl(PlayerPrefs.GetInt("control"));
        UpdateArrow(PlayerPrefs.GetInt("ARROW"));
        UpdateCD(PlayerPrefs.GetInt("CD"));

        SetSliderValue(Sound.MixerGroup.music);
        SetSliderValue(Sound.MixerGroup.sound);

        music.onValueChanged.AddListener((float value) => { ChangeMixerVolume(Sound.MixerGroup.music, value); });
        sound.onValueChanged.AddListener((float value) => { ChangeMixerVolume(Sound.MixerGroup.sound, value); });
        
        resume.onClick.AddListener(() => { Resume(); });
        transition.onClick.AddListener(() => { ToggleTransition(); });
        control.onClick.AddListener(() => { ToggleControl(); });
        arwOnSnk.onClick.AddListener(() => { ToggleArrow(); });
        cdOnSnk.onClick.AddListener(() => { ToggleCD(); });

        if (mainMenu != null)
        {
            mainMenu.onClick.AddListener(() => {
                GameManager.instance.SetupStartingVariable();
                SceneChanger.instance.LoadThisScene(0);
            });
        }
        if (restart != null)
        {
            restart.onClick.AddListener(() => {
                GameManager.instance.SetupStartingVariable();
                SceneChanger.instance.LoadThisScene(1);
            });
        }
    }

    public void Resume()
    {
        UIManipulation.instance.CloseUI(gameObject);
        if (!GameManager.inShop)
        {
            GameManager.waveStart = true; 
            WaveSpawner.instance.TimerManip(false);
        }
    }

    private void ChangeMixerVolume(Sound.MixerGroup group, float value)
    {
        AudioMixerGroup mixer = audioM.GetMixerGroup(group);
        if (value != 0)
        {
            float volume = Mathf.Log10(value) * 20;
            mixer.audioMixer.SetFloat("Volume", volume);

            string prefKey = "Volume_" + group.ToString();
            PlayerPrefs.SetFloat(prefKey, volume);
        }
    }
    private void SetSliderValue(Sound.MixerGroup group)
    {
        string prefKey = "Volume_" + group.ToString();
        float volume = PlayerPrefs.GetFloat(prefKey);

        if(group == Sound.MixerGroup.music)
        {
            music.value = Mathf.Pow(Mathf.Pow(10, volume), 1f / 20f);
        }
        else
        {
            sound.value = Mathf.Pow(Mathf.Pow(10, volume), 1f / 20f);
        }
    }

    public void ToggleTransition()
    {
        int state = PlayerPrefs.GetInt("transit");

        state++;
        if(state == 4) { state = 0; } //0 1 2 OK

        PlayerPrefs.SetInt("transit", state);

        UpdateDarkTransition(state);
        GameManager.instance.SetTransitionColor(state);
    }
    private void UpdateDarkTransition(int state)
    {
        switch (state)
        {
            case 0:
                transition.GetComponentInChildren<Text>().text = "Transition: LIGHT";
                break;
            case 1:
                transition.GetComponentInChildren<Text>().text = "Transition: GRAY";
                break;
            case 2:
                transition.GetComponentInChildren<Text>().text = "Transition: DARK";
                break;
            case 3:
                transition.GetComponentInChildren<Text>().text = "Transition: YELLOW";
                break;
        }
    }
   
    public void ToggleControl()
    {
        int state = PlayerPrefs.GetInt("control");

        state++;
        if (state == 2) { state = 0; } //0 1 OK

        PlayerPrefs.SetInt("control", state);

        UpdateControl(state);
        SnakeManager.instance.SetMoveType(state);
    }
    private void UpdateControl(int state)
    {
        switch (state)
        {
            case 0:
                control.GetComponentInChildren<Text>().text = "Control: " + SnakeManager.MoveType.ARROW.ToString();
                break;
            case 1:
                control.GetComponentInChildren<Text>().text = "Control: " + SnakeManager.MoveType.MOUSE.ToString();
                break;
        }
    }

    public void ToggleArrow()
    {
        int state = PlayerPrefs.GetInt("ARROW");

        state++;
        if(state == 2) { state = 0; } //0 1 OK

        UpdateArrow(state);
        PlayerPrefs.SetInt("ARROW", state);

        SnakeManager.instance.SpawnArrowHead();
    }
    private void UpdateArrow(int state)
    {
        switch (state)
        {
            case 0:
                arwOnSnk.GetComponentInChildren<Text>().text = "Arrow on Snake: NO";
                break;
            case 1:
                arwOnSnk.GetComponentInChildren<Text>().text = "Arrow on Snake: YES";
                break;
        }
    }

    public void ToggleCD()
    {
        int state = PlayerPrefs.GetInt("CD");

        state++;
        if (state == 2) { state = 0; } //0 1 OK

        PlayerPrefs.SetInt("CD", state);

        UpdateCD(state);
        List<GameObject> bodies = SnakeManager.instance.GetSnakeBody();

        foreach (GameObject body in bodies)
        {
            body.GetComponent<Hero>().SetEnabledCDSlider(state==1);
        }
    }
    private void UpdateCD(int state)
    {
        switch (state)
        {
            case 0:
                cdOnSnk.GetComponentInChildren<Text>().text = "Cooldown on Snake: NO";
                break;
            case 1:
                cdOnSnk.GetComponentInChildren<Text>().text = "Cooldown on Snake: YES";
                break;
        }
    }
}
