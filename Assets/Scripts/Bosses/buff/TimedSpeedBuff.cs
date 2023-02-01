public class TimedSpeedBuff : TimedBuff
{
    private float SpeedIncrease;

    public void SetBuff(bool isDurationStackable, float duration, float spdIncre)
    {
        SetBuff(isDurationStackable, duration);
        SpeedIncrease = spdIncre;
    }

    protected override void ApplyEffect()
    {
        float speed = GetComponent<Enemy>().GetSpeed() + SpeedIncrease;
        GetComponent<Enemy>().SetSpeed(speed);
        ApplyEffect();
    }
    protected override void End()
    {
        float speed = GetComponent<Enemy>().GetSpeed() - SpeedIncrease;
        GetComponent<Enemy>().SetSpeed(speed);
        End();
    }
}
