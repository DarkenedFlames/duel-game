namespace MyApp;

using System;

public class Armor : Stat
{
    public override int Base { get; set; }
    public override float Multiplier { get; set; }
    public Armor()
    {
        Base = 50;
        Multiplier = 1.0f;
    }
}