using System;

namespace MyApp
{
    public enum StackingType
    {
        RefreshOnly, // Refresh duration only
        AddStack,    // Add a stack and refresh duration
        Ignore       // Do nothing if effect already exists
    }
}