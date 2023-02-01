using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhiteBoss : Enemy
{
    [Header("Boss Specific")]
    [SerializeField] private float SpeedIncrease = 500;
    [SerializeField] private float Duration = 2f;
    [SerializeField] private bool IsDurationStackable = false;
    protected override void Update()
    {
        base.Update();      //need
    }
    // Start is called before the first frame update
    protected override void Attack()
    {
        if (!gameObject.GetComponent<TimedSpeedBuff>())
        {
            TimedSpeedBuff buff = gameObject.AddComponent<TimedSpeedBuff>();
            buff.SetBuff(IsDurationStackable, Duration, SpeedIncrease);
        }
        gameObject.GetComponent<TimedSpeedBuff>().Activate();
    }
    protected override void OnDestroy()
    {
    }
}
