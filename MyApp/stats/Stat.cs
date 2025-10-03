namespace MyApp;

using System;

public abstract class Stat
{
    public abstract int Base { get; set; }
    public abstract float Multiplier { get; set; }
    public int Value { get{ return (int)(Base * Multiplier); } }
}