using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopClassManager : MonoBehaviour
{
    //static variable for accessibility
    public static ShopClassManager instance;
    private void Awake() { if (instance == null) { instance = this; } }

    ////////////////////////////////////////////////////////////////////////////
    //Main-GameObject-Variables to create a class counter 
    [SerializeField] private GameObject classHolderPrefab = null;
    [SerializeField] private GameObject classCounterPrefab = null;
    public GameObject GetPrefab(int index) { return index == 0 ? classHolderPrefab : classCounterPrefab; }

    ////////////////////////////////////////////////////////////////////////////
    //list holding class in team
    private List<classesData> classesInTeam = new List<classesData>();
    //list holding gameObject of spawned class in team
    struct ClassCounterGO { public GameObject GO; public HeroClass.Classes type; }
    private List<ClassCounterGO> classCounterGO = new List<ClassCounterGO>();

    //for outside of script access
    public void ModifyClassesInTeam(List<HeroClass> heroClasses, bool add)
    {
        //loop through each class in list of hero class
        foreach (HeroClass _class in heroClasses)
        {
            //add or remove depended
            if (add) { AddToClassesInTeam(_class); }
            else     { RemoveClassesInTeam(_class); }
        }
    }
    private void AddToClassesInTeam(HeroClass _class)
    {
        //init needed start variable
        classesData data = new classesData(_class.chosenClass, 1);
        //loop through all classes
        for (int i = 0; i < classesInTeam.Count; i++)
        {
            //find class in list
            if (classesInTeam[i].heroClass == data.heroClass)
            {
                //add number of existing
                classesInTeam[i].AddNumber();
                return;
            }
        }

        //if cant find an existing class in list -> add new
        classesInTeam.Add(data);
        //create physical gameObject -> add to list
        GameObject CounterGO = GameManager.SpawnCounterClassHolder(_class, transform);
        ClassCounterGO counterObj = new ClassCounterGO
        {
            GO = CounterGO,
            type = _class.chosenClass
        };
        CounterGO.GetComponent<ShowDescription>().SetHeroClassScript(_class);
        classCounterGO.Add(counterObj);
    }
    private void RemoveClassesInTeam(HeroClass _class)
    {
        //init needed start variable
        classesData data = new classesData(_class.chosenClass, 1);
        //loop through all classes
        for (int i = 0; i < classesInTeam.Count; i++)
        {
            //find class in list
            if (classesInTeam[i].heroClass == data.heroClass)
            {
                //decrease number of a existing class
                classesInTeam[i].DecrNumber();
                //if delete all member of a class
                if(classesInTeam[i].GetNumber() == 0) {
                    //loop through List hold class+counter
                    for (int j = 0; j < classCounterGO.Count; j++)
                    {
                        //if find empty class counter -> remove
                        if(classCounterGO[j].type == classesInTeam[i].heroClass)
                        {
                            //destroy physical gameobject + remove in list
                            Destroy(classCounterGO[j].GO);
                            classCounterGO.RemoveAt(j); 
                            classesInTeam.RemoveAt(i);
                            return;
                        }
                    }
                }

                return;
            }
        }
    }
}

////////////////////////////////////////////////////////////////////////////
[System.Serializable] public class classesData
{
    //main variables;
    public HeroClass.Classes heroClass;
    private int number;

    //init function
    public classesData(HeroClass.Classes _class, int num) { heroClass = _class; number = num; }
   
    //functions for accessibility
    public void AddNumber() { number++; }
    public void DecrNumber() { number--; }
    public int GetNumber() { return number; }
}