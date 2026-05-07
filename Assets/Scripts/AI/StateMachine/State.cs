// Copyright Cristian Pagán Díaz. All Rights Reserved.

public abstract class State
{
    public virtual void FixedUpdate() { }

    public abstract State Update();
}
