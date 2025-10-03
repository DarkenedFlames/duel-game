namespace MyApp;

using System;

public class Critical : Stat
{
    public override int Base { get; set; }
    public override float Multiplier { get; set; }
    public Critical()
    {
        Base = 0;
        Multiplier = 1.0f;
    }
}