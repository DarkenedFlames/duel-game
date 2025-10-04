namespace MyApp;

using System;

public class Luck : Stat
{
    public override int Base { get; set; }
    public override float Multiplier { get; set; }
    public Luck()
    {
        Base = 50;
        Multiplier = 1.0f;
    }
}