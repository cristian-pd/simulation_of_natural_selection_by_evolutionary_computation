// Copyright Cristian Pagán Díaz. All Rights Reserved.

using UnityEngine;

public class IndividualRunAwayState : IndividualState
{
    private Individual m_Agressor;

    public IndividualRunAwayState(Individual individual, Individual aggresor) : base(individual)
    {
        m_Agressor = aggresor;
    }

    public override void FixedUpdate()
    {
        Vector3 lookAt = -m_Agressor.transform.position;
        lookAt.y = GetIndividual().transform.position.y; // As� no mira ni para arriba ni para abajo

        GetIndividual().transform.LookAt(lookAt, Vector3.up);

        Vector3 direction = GetIndividual().transform.position - m_Agressor.transform.position;
        direction.y = 0.0f;

        if (GetIndividual().transform.position.magnitude >= Configuration.Singleton.FloorRadius)
            return;

        GetIndividual().transform.Translate(direction.normalized * Time.deltaTime * GetIndividual().Representation.GetRealSpeed() * IndividualRepresentation.k_SpeedIncrement, Space.World);
    }

    public override State Update()
    {
        if (GetIndividual().transform.position.magnitude >= Configuration.Singleton.FloorRadius)
            return new IndividualSearchState(GetIndividual());

        if (GetIndividual().CurrentEnergy == 0.0f)
            return new IndividualSearchState(GetIndividual());

        if (!GetIndividual().GetSeeing().Individuals.Contains(m_Agressor) && !GetIndividual().GetSmellingHearing().Individuals.Contains(m_Agressor))
            return new IndividualSearchState(GetIndividual());

        GetIndividual().LooseEnergy();

        return this;
    }
}
