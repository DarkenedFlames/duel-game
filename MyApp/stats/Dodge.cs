namespace MyApp;

using System;

public class Dodge : Stat
{
    public override int Base { get; set; }
    public override float Multiplier { get; set; }
    public Dodge()
    {
        Base = 0;
        Multiplier = 1.0f;
    }
}