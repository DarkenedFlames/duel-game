namespace MyApp;

using System;

public class Shield : Stat
{
    public override int Base { get; set; }
    public override float Multiplier { get; set; }
    public Shield()
    {
        Base = 50;
        Multiplier = 1.0f;
    }
}