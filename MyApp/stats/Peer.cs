namespace MyApp;

using System;

public class Peer : Stat
{
    public override int Base { get; set; }
    public override float Multiplier { get; set; }
    public Peer()
    {
        Base = 50;
        Multiplier = 1.0f;
    }
}