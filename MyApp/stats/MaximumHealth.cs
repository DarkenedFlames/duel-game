namespace MyApp;

using System;

public class MaximumHealth : Stat
{
    public override int Base { get; set; }
    public override float Multiplier { get; set; }
    public MaximumHealth()
    {
        Base = 100;
        Multiplier = 1.0f;
    }
}