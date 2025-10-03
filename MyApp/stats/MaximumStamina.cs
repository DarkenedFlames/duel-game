namespace MyApp;

using System;

public class MaximumStamina : Stat
{
    public override int Base { get; set; }
    public override float Multiplier { get; set; }
    public MaximumStamina()
    {
        Base = 100;
        Multiplier = 1.0f;
    }
}