namespace MyApp;

using System;

public class Stat
{
    public int Base { get; set; }
    public float Multiplier { get; set; }
    public int Value { get { return (int)(Base * Multiplier); } }
    public Stat(int baseValue = 0, float multiplier = 1.0f)
    {
        Base = baseValue;
        Multiplier = multiplier;
    }
}