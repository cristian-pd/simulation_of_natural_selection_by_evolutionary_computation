// Copyright Cristian Pagán Díaz. All Rights Reserved.

using System;

public abstract class Representation : ICloneable
{
    public abstract void Cross(Representation representation);
    public abstract void Mute();
    public abstract object Clone();
}
