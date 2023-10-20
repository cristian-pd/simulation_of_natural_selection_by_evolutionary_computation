// Copyright Cristian Pagán Díaz. All Rights Reserved.

public abstract class IndividualState : State
{
    private Individual m_Individual;

    public IndividualState(Individual individual)
    {
        m_Individual = individual;
    }

    protected Individual GetIndividual()
    {
        return m_Individual;
    }
}
