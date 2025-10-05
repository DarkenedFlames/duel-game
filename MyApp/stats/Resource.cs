namespace MyApp;

using System;

public class Resource
{
    public int Value { get; set; }
    public Stat MaximumStat { get; set; }
    public float RestorationMultiplier { get; set; }
    public float ExpenditureMultiplier { get; set; }

    public Resource(Stat MaxStat)
    {
        Value = MaxStat.Value;
        MaximumStat = MaxStat;
        RestorationMultiplier = 1.0f;
        ExpenditureMultiplier = 1.0f;
    }
    public void Change(int delta)
    {
        float multiplier = delta >= 0 ? RestorationMultiplier : ExpenditureMultiplier;
        int adjusted = (int)(delta * multiplier);
        Value += adjusted;
        Value = Math.Clamp(Value, 0, MaximumStat.Value);
    }
}