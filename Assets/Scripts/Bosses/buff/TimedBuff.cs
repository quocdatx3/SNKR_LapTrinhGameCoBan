using UnityEngine;

public class TimedBuff : MonoBehaviour
{
    private float duration = 0;
    private bool isDurationStackable = false;
    private bool isFinished = false;
    private float _duration = 0;

    private void Update()
    {
        if (!isFinished)
        {
            _duration -= Time.deltaTime;
            if (_duration <= 0) { End(); }
        }
    }
    public void SetBuff(bool isDurationStackable, float duration)
    {
        this.isDurationStackable = isDurationStackable;
        this.duration = duration;
    }
    public void Activate()
    {        
        if (isDurationStackable || _duration <= 0)
        {
            _duration += duration;
        }
    }
    protected virtual void ApplyEffect()
    {
        isFinished = false;
    }
    protected virtual void End()
    {
        isFinished = true;
    }
}
