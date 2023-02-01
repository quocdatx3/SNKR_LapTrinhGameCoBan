using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassCounterManager : MonoBehaviour
{
    public static ClassCounterManager instance;
    private void Awake() { instance = this; }

    ////////////////////////////////////////////////////////////////////////////
    private List<CounterClass> counterList = new List<CounterClass>();
    
    public void AddToCounterList(HeroClass.Classes _class, ClassCounter data)
    {
        CounterClass counterClass = new CounterClass { _class = _class }; counterClass.counters.Add(data);

        if(counterList.Count == 0) { counterList.Add(counterClass); }
        else
        {
            for (int i = 0; i < counterList.Count; i++)
            {
                if(counterList[i]._class == _class) { counterList[i].counters.Add(data); return; }
            }

            counterList.Add(counterClass);
        }
    }
    public void UpdateCounter(HeroClass.Classes updatedClass)
    {
        int i, j;
        //remove empty
        for (i = 0; i < counterList.Count; i++)
        {
            j = 0;
            while (j < counterList[i].counters.Count)
            {
                if (counterList[i].counters[j] == null) { counterList[i].counters.RemoveAt(j); continue; }
                j++;
            }
        }
        //find correct class to update
        for (i = 0; i < counterList.Count; i++)
        {
            if (counterList[i]._class == updatedClass)
            {
                for (j = 0; j < counterList[i].counters.Count; j++) { counterList[i].counters[j].UpdateCount(); }
                break;
            }
        }
    }
}

////////////////////////////////////////////////////////////////////////////
[System.Serializable]
public class CounterClass
{
    public HeroClass.Classes _class;
    public List<ClassCounter> counters = new List<ClassCounter>();
}
