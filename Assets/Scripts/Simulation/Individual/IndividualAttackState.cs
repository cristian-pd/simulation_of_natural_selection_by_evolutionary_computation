// Copyright Cristian Pagán Díaz. All Rights Reserved.

using UnityEngine;

public class IndividualAttackState : IndividualState
{
    private float m_Timer;
    private Individual m_Target;

    public IndividualAttackState(Individual individual, Individual target) : base(individual)
    {
        m_Timer = 0.0f;
        m_Target = target;
    }

    public override void FixedUpdate()
    {
        Vector3 lookAt = m_Target.transform.position;
        lookAt.y = GetIndividual().transform.position.y; // As� no mira ni para arriba ni para abajo

        GetIndividual().transform.LookAt(lookAt, Vector3.up);

        if (GetIndividual().transform.position.magnitude >= Configuration.Singleton.FloorRadius)
            return;

        Vector3 direction = m_Target.transform.position - GetIndividual().transform.position;
        direction.y = 0.0f;

        GetIndividual().transform.Translate(direction.normalized * Time.deltaTime * GetIndividual().Representation.GetRealSpeed() * IndividualRepresentation.k_SpeedIncrement, Space.World);
    }

    public override State Update()
    {
        if (GetIndividual().transform.position.magnitude >= Configuration.Singleton.FloorRadius)
            return new IndividualSearchState(GetIndividual());

        if (GetIndividual().CurrentEnergy == 0.0f)
            return new IndividualSearchState(GetIndividual());

        if (!GetIndividual().GetSeeing().Individuals.Contains(m_Target) && !GetIndividual().GetSmellingHearing().Individuals.Contains(m_Target))
            return new IndividualSearchState(GetIndividual());

        if (Vector3.Distance(GetIndividual().transform.position, m_Target.transform.position) <= Individual.k_AttackRange)
        {
            if (m_Timer <= 0.0f)
            {
                m_Timer = Individual.k_AttackTime;
                m_Target.Attack(GetIndividual(), GetIndividual().Representation.Strength);
            }
        }

        GetIndividual().LooseEnergy();

        m_Timer -= Time.deltaTime;

        return this;
    }
}
