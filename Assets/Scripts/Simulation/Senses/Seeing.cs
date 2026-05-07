// Copyright Cristian Pagán Díaz. All Rights Reserved.

using System.Collections.Generic;
using UnityEngine;

public class Seeing : Senses
{
    private LinkedList<Individual> m_Individuals;
    private LinkedList<Reward> m_Rewards;

    protected override void OnSenseStart()
    {
        SenseOnEnable();

        StartSense();
    }

    protected override void OnSenseProcessing()
    {
        RemoveInactive(m_Individuals);
        RemoveInactive(m_Rewards);

        base.OnSenseProcessing();

        ProcessSeeing(Individuals, m_Individuals);
        ProcessSeeing(Rewards, m_Rewards);
    }

    private void ProcessSeeing<T>(LinkedList<T> sensorList, LinkedList<T> triggerList) where T : MonoBehaviour
    {
        foreach (T element in triggerList)
        {
            Vector3 otherPos = element.transform.position;
            otherPos.y = 0.0f;
            otherPos = otherPos.normalized;

            float angle = Vector3.Angle(transform.forward, otherPos);

            if (angle <= m_Individual.Representation.FieldOfViewDregrees / 2.0f)
            {
                if (!sensorList.Contains(element))
                    sensorList.AddLast(element);
            }
            else
            {
                sensorList.Remove(element);
            }
        }
    }

    protected override void OnSenseEnter(Individual other)
    {
        m_Individuals.AddFirst(other);
    }

    protected override void OnSenseExit(Individual other)
    {
        m_Individuals.Remove(other);
        Individuals.Remove(other);
    }

    protected override void OnSenseEnter(Reward other)
    {
        m_Rewards.AddLast(other);
    }

    protected override void OnSenseExit(Reward other)
    {
        m_Rewards.Remove(other);
        Rewards.Remove(other);
    }

    public override void ResetSense()
    {
        StartSense();
        base.ResetSense();
    }

    private void StartSense()
    {
        m_Individuals = new LinkedList<Individual>();
        m_Rewards = new LinkedList<Reward>();
    }

    protected override void SenseOnEnable()
    {
        m_SphereCollider.radius = m_Individual.Representation.SightDistance;
    }

    private void OnDrawGizmos()
    {
        Color color = Gizmos.color;

        if (m_Individual.Representation.SeeingPriorityPct >= (100 - m_Individual.Representation.SeeingPriorityPct))
            Gizmos.color = Color.blue;
        else
            Gizmos.color = Color.red;

        float alpha = 90.0f * Mathf.Deg2Rad;
        float halfFov = m_Individual.Representation.FieldOfViewDregrees / 2.0f;
        float theta = halfFov * Mathf.Deg2Rad;

        Vector3 src = Vector3.zero;
        src.x = m_Individual.Representation.SightDistance * Mathf.Cos(alpha - theta);
        src.z = m_Individual.Representation.SightDistance * Mathf.Sin(alpha - theta);

        for (float angle = 90.0f - halfFov + 1; angle <= 90.0f + halfFov; angle += 1.0f)
        {
            float radians = angle * Mathf.Deg2Rad;

            Vector3 dst = Vector3.zero;
            dst.x = m_Individual.Representation.SightDistance * Mathf.Cos(radians);
            dst.z = m_Individual.Representation.SightDistance * Mathf.Sin(radians);

            Gizmos.DrawLine(transform.position + transform.rotation * src, transform.position + transform.rotation * dst);

            src = dst;
        }

        Vector3 v1 = Vector3.zero;
        v1.x = m_Individual.Representation.SightDistance * Mathf.Cos(alpha + theta);
        v1.z = m_Individual.Representation.SightDistance * Mathf.Sin(alpha + theta);

        Vector3 v2 = Vector3.zero;
        v2.x = m_Individual.Representation.SightDistance * Mathf.Cos(alpha - theta);
        v2.z = m_Individual.Representation.SightDistance * Mathf.Sin(alpha - theta);

        Gizmos.DrawLine(transform.position, transform.position + transform.rotation * v1);
        Gizmos.DrawLine(transform.position, transform.position + transform.rotation * v2);

        Gizmos.color = color;
    }
}
