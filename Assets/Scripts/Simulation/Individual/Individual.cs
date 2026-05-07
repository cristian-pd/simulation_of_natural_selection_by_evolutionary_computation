// Copyright Cristian Pagán Díaz. All Rights Reserved.

using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class Individual : MonoBehaviour, IComparable
{
    private class AttackerInfo
    {
        public Individual Individual;
        public float Timer;
    }

    public const float k_AttackTime = 1.5f;
    public const float k_SpawnRadius = 1.25f;
    public const float k_AttackRange = 2.0f;

    [SerializeField] private GameObject m_KilledMarkPrefab;
    [SerializeField] private GameObject m_EndLifeTimeMarkPrefab;
    
    [SerializeField] private Seeing m_Seeing;
    [SerializeField] private SmellingHearing m_SmellingHearing;

    public IndividualRepresentation Representation;

    private Renderer m_Renderer;

    private State m_State;

    public float CurrentEnergy { get; private set; }
    private float m_TotalAliveTime;

    private float m_CurrentLifeTime;
    private float m_CurrentVitality;
    private LinkedList<AttackerInfo> m_Attackers;

    private GameObject m_Mark;

    private void Awake()
    {
        m_Renderer = GetComponent<Renderer>();

        Representation = new IndividualRepresentation();

        ResetIndividual();

        m_Attackers = new LinkedList<AttackerInfo>();
    }

    private void ResetIndividual()
    {
        m_State = new IndividualSearchState(this);
        m_CurrentLifeTime = Configuration.Singleton.MaxLifeTime;
        m_CurrentVitality = Representation.MaxVitality;
        CurrentEnergy = Representation.MaxEnergy;
        m_TotalAliveTime = 0.0f;
        m_Renderer.material.color = Representation.CalculateColor();
    }

    private void FixedUpdate()
    {
        m_State.FixedUpdate();
    }

    private void Update()
    {
        m_State = m_State.Update();

        UpdateAttackers();

        if (m_CurrentLifeTime - Time.deltaTime <= 0.0f)
        {
            m_TotalAliveTime += m_CurrentLifeTime;
            m_CurrentLifeTime -= m_CurrentLifeTime;
        }
        else
        {
            m_CurrentLifeTime -= Time.deltaTime;
            m_TotalAliveTime += Time.deltaTime;
        }

        if (IsDead())
        {
            float killReward = Configuration.Singleton.KillReward / m_Attackers.Count;

            foreach (AttackerInfo attacker in m_Attackers)
                attacker.Individual.RegenerateLifeTime(killReward);

            if (m_Attackers.Count > 0)
                m_Mark = Instantiate(m_KilledMarkPrefab);
            else
                m_Mark = Instantiate(m_EndLifeTimeMarkPrefab);

            m_Mark.transform.position = new Vector3(transform.position.x, EvolutionaryComputation.k_FloorHeight * 1.5f, transform.position.z);
            m_Mark.transform.parent = transform.parent.parent;

            m_Attackers = new LinkedList<AttackerInfo>();

            gameObject.SetActive(false);
        }
    }

    public float Fitness()
    {
        return m_TotalAliveTime;
    }

    private void UpdateAttackers()
    {
        var currentNode = m_Attackers.First;
        while (currentNode != null)
        {
            currentNode.Value.Timer -= Time.deltaTime;

            var nextNode = currentNode.Next;
            if (currentNode.Value.Timer <= 0.0f)
                m_Attackers.Remove(currentNode);
            currentNode = nextNode;
        }
    }

    public Senses GetSeeing()
    {
        return m_Seeing;
    }

    public Senses GetSmellingHearing()
    {
        return m_SmellingHearing;
    }

    private bool IsDead()
    {
        return m_CurrentLifeTime <= 0.0f || m_CurrentVitality <= 0.0f;
    }

    public void Attack(Individual attacker, float damage)
    {      
        AttackerInfo attackerInfo = new AttackerInfo { Individual = attacker, Timer = k_AttackTime };
        m_Attackers.AddLast(attackerInfo);

        float lifeTimePct = m_CurrentLifeTime / (2.0f * Configuration.Singleton.MaxLifeTime);
        m_CurrentVitality -= damage * (1.0f - lifeTimePct);
    }

    public void RegenerateLifeTime(float value)
    {
        m_CurrentLifeTime += value;
    }

    public void LooseEnergy()
    {
        if (CurrentEnergy > 0.0f)
        {
            CurrentEnergy -= Time.deltaTime;

            if (CurrentEnergy < 0.0f)
                CurrentEnergy = 0.0f;
        }
    }

    public void RestoreEnergy()
    {
        if (CurrentEnergy < Representation.MaxEnergy)
        {
            CurrentEnergy += Time.deltaTime;

            if (CurrentEnergy > Representation.MaxEnergy)
                CurrentEnergy = Representation.MaxEnergy;
        }
    }

    private void OnDisable()
    {
        m_Seeing.ResetSense();
        m_SmellingHearing.ResetSense();
    }

    private void OnEnable()
    {
        ResetIndividual();

        if (m_Mark != null)
        {
            Destroy(m_Mark);
            m_Mark = null;
        }
    }

    public int CompareTo(object obj)
    {
        Individual other = (Individual)obj;

        float a = Fitness();
        float b = other.Fitness();

        if (a < b)
            return -1;
        else if (a > b)
            return 1;
        else
            return 0;
    }
}
