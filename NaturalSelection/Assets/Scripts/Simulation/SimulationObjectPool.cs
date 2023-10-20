// Copyright Cristian Pagán Díaz. All Rights Reserved.

using System.Collections.Generic;
using UnityEngine;

public class SimulationObjectPool : ObjectPool
{
    [SerializeField] private Individual m_IndividualPrefab;
    [SerializeField] private Reward m_RewardPrefab;

    private GameObject m_Individuals;
    private GameObject m_Rewards;

    private Individual[] m_CurrentIndividuals;
    private Reward[] m_CurrentRewards;

    private List<Vector3> m_SpawnPoints;
    private List<bool> m_SpawnPointsOcupation;

    private bool m_Update;
    private bool m_FirstTime;

    protected override void Init(object[] args)
    {
        m_FirstTime = true;

        InitSpawningPoints();

        // TODO: Comprobar que hay suficientes puntos de spawn

        InitIndividuals();
        InitRewards();
    }

    private void LateUpdate()
    {
        if (!m_Update)
            return;

        bool generationEnd = true;
        for (int i = 0; i < m_CurrentIndividuals.Length; i++)
        {
            if (m_CurrentIndividuals[i].gameObject.activeSelf)
            {
                generationEnd = false;
                continue;
            }
        }

        if (generationEnd)
        {
            Despawn();

            if (m_FirstTime)
            {
                SendMessage("FirstSelection", m_CurrentIndividuals);
                m_FirstTime = false;
            }
            else
            {
                SendMessage("Combination", m_CurrentIndividuals);
            }
        }
    }

    private void InitSpawningPoints()
    {
        m_SpawnPoints = new List<Vector3>();
        m_SpawnPointsOcupation = new List<bool>();

        float floorRadius = Configuration.Singleton.FloorRadius;

        for (float z = -floorRadius; z <= floorRadius; z += Individual.k_SpawnRadius)
        {
            for (float x = -floorRadius; x <= floorRadius; x += Individual.k_SpawnRadius)
            {
                Vector3 position = new Vector3(x, 0.0f, z);
                if (position.magnitude <= floorRadius)
                {
                    m_SpawnPoints.Add(position);
                    m_SpawnPointsOcupation.Add(false);
                }
            }
        }
    }

    private void InitIndividuals()
    {
        m_CurrentIndividuals = new Individual[Configuration.Singleton.IndividualsSpawnCount];

        m_Individuals = new GameObject("Individuals");
        m_Individuals.transform.position = transform.position;
        m_Individuals.transform.SetParent(transform);

        for (int i = 0; i < m_CurrentIndividuals.Length; i++)
        {
            Individual individual = Instantiate(m_IndividualPrefab);
            individual.gameObject.SetActive(false);
            m_CurrentIndividuals[i] = individual;

            individual.transform.SetParent(m_Individuals.transform);

            individual.name = "Individual[" + i + "]";
        }
    }

    private void InitRewards()
    {
        m_CurrentRewards = new Reward[Configuration.Singleton.RewardsSpawnCount];

        m_Rewards = new GameObject("Rewards");
        m_Rewards.transform.position = transform.position;
        m_Rewards.transform.SetParent(transform);

        for (int i = 0; i < m_CurrentRewards.Length; i++)
        {
            Reward reward = Instantiate(m_RewardPrefab);
            reward.gameObject.SetActive(false);
            m_CurrentRewards[i] = reward;

            reward.transform.SetParent(m_Rewards.transform);

            reward.name = "Reward[" + i + "]";
        }
    }

    protected override void Spawn()
    {
        SpawnIndividuals();
        SpawnRewards();

        m_Update = true;
    }

    private void SpawnIndividuals()
    {
        for (int i = 0; i < m_CurrentIndividuals.Length; i++)
        {
            Individual individual = m_CurrentIndividuals[i];
            individual.gameObject.SetActive(true);

            Vector3 spawnPoint = NextSpawnPoint();

            float x = spawnPoint.x;
            float y = EvolutionaryComputation.k_FloorHeight / 2.0f + individual.transform.localScale.y / 2.0f;
            float z = spawnPoint.z;

            individual.transform.position = m_Individuals.transform.position + new Vector3(x, y, z);
        }
    }

    private void SpawnRewards()
    {
        for (int i = 0; i < m_CurrentRewards.Length; i++)
        {
            Reward reward = m_CurrentRewards[i];
            reward.gameObject.SetActive(true);

            Vector3 spawnPoint = NextSpawnPoint();

            float x = spawnPoint.x;
            float y = EvolutionaryComputation.k_FloorHeight / 2.0f + reward.transform.localScale.y / 2.0f;
            float z = spawnPoint.z;

            reward.transform.position = m_Rewards.transform.position + new Vector3(x, y, z);
        }
    }

    // Se puede hacer m�s eficiente a�n
    private Vector3 NextSpawnPoint()
    {
        int spawnPointIndex;

        do
        {
            spawnPointIndex = ReproducibleRandom.Range(0, m_SpawnPoints.Count);
        } while (m_SpawnPointsOcupation[spawnPointIndex]);

        Vector3 spawnPoint = m_SpawnPoints[spawnPointIndex];
        m_SpawnPointsOcupation[spawnPointIndex] = true;

        return spawnPoint;
    }

    protected override void Despawn()
    {
        m_Update = false;

        for (int i = 0; i < m_CurrentIndividuals.Length; i++)
            m_CurrentIndividuals[i].gameObject.SetActive(false);

        for (int i = 0; i < m_CurrentRewards.Length; i++)
            m_CurrentRewards[i].gameObject.SetActive(false);

        for (int i = 0; i < m_SpawnPointsOcupation.Count; i++)
            m_SpawnPointsOcupation[i] = false;
    }
}
