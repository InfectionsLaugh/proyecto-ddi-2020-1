using System;

public class Token
{
    public int type;
    public string name;

    public Token(string tokenName, int tokenType)
    {
        type = tokenType;
        name = tokenName;
    }

    public override string ToString()
    {
        return "<" + name + ", " + type + ">";
    }
}