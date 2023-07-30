using System;

public class InputSchemeChangedEventArgs : EventArgs
{
    public InputScheme NewInputScheme { get; }

    public InputSchemeChangedEventArgs(InputScheme newInputScheme)
    {
        NewInputScheme = newInputScheme;
    }

}