// Copyright Cristian Pagán Díaz. All Rights Reserved.

using UnityEngine;

public class SmellingHearing : Senses
{
    protected override void OnSenseStart()
    {
        SenseOnEnable();
    }

    protected override void OnSenseEnter(Individual other)
    {
        Individuals.AddLast(other);
    }

    protected override void OnSenseExit(Individual other)
    {
        Individuals.Remove(other);
    }

    protected override void OnSenseEnter(Reward other)
    {
        Rewards.AddLast(other);
    }

    protected override void OnSenseExit(Reward other)
    {
        Rewards.Remove(other);
    }

    protected override void SenseOnEnable()
    {
        m_SphereCollider.radius = m_Individual.Representation.SmellingHearingRadius;
    }

    private void OnDrawGizmos()
    {
        Color color = Gizmos.color;

        if (m_Individual.Representation.SeeingPriorityPct < (100 - m_Individual.Representation.SeeingPriorityPct))
            Gizmos.color = Color.blue;
        else
            Gizmos.color = Color.red;

        Vector3 src = Vector3.zero;
        src.x = m_Individual.Representation.SmellingHearingRadius * Mathf.Cos(0.0f);
        src.z = m_Individual.Representation.SmellingHearingRadius * Mathf.Sin(0.0f);

        for (float angle = 1.0f; angle <= 360.0; angle += 1.0f)
        {
            float radians = angle * Mathf.Deg2Rad;

            Vector3 dst = Vector3.zero;
            dst.x = m_Individual.Representation.SmellingHearingRadius * Mathf.Cos(radians);
            dst.z = m_Individual.Representation.SmellingHearingRadius * Mathf.Sin(radians);

            Gizmos.DrawLine(transform.position + src, transform.position + dst);

            src = dst;
        }

        Gizmos.color = color;
    }
}
