namespace MyApp;

using System;

public class Stat
{
    public int Base { get; set; }
    public float Multiplier { get; set; }
    public int Value { get{ return (int)(Base * Multiplier); } }
}