using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelRotation : MonoBehaviour
{
    private Transform pivot;
    private Hero hero;

    private void Start()
    {
        hero = GetComponent<Hero>();

        //create pivot automatically 
        GameObject obj = new GameObject();
        pivot = obj.transform;
        pivot.SetParent(transform);
        pivot.localPosition = Vector3.zero;
        hero.SetPivot(pivot);
    }
    private void Update()
    {
        List<GameObject> targets = hero.GetCurrentTargets();
        if (targets.Count > 0 && targets[0] != null)
        {
            Vector2 relative = targets[0].transform.position - pivot.transform.position;
            float angle = Mathf.Atan2(relative.y, relative.x) * Mathf.Rad2Deg;
            Vector3 newRotation = new Vector3(0, 0, angle);
            pivot.rotation = Quaternion.Euler(newRotation);
        }
    }
}
