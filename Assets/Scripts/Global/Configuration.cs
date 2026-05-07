// Copyright Cristian Pagán Díaz. All Rights Reserved.

using UnityEngine;

// Este script se ha fijado como el primero en ejecutarse de todos para evitar problemas de dependencia
public class Configuration : MonoBehaviour
{
    public static Configuration Singleton { get; private set; }

    [SerializeField] private int m_RandomSeed = 0;

    [SerializeField] private float m_MutationProbability = 0.02f;
    [SerializeField] private float m_MaxMutationPct = 0.3f;
    [Min(10)] [SerializeField] private int m_IndividualsSpawnCount = 10;
    [SerializeField] private int m_RewardsSpawnCount = 40;
    [SerializeField] private float m_FloorRadius = 25.0f;
    [SerializeField] private float m_MaxLifeTime = 2.5f;
    [SerializeField] private float m_LifeTimeRegeneration = 0.5f;
    [SerializeField] private float m_KillReward = 5.0f;
    [SerializeField] private Vector2Int m_VitalityRange = new Vector2Int(50, 150);
    [SerializeField] private Vector2Int m_SpeedRange = new Vector2Int(10, 30);
    [SerializeField] private Vector2Int m_StrengthRange = new Vector2Int(5, 35);
    [SerializeField] private Vector2Int m_EnergyRange = new Vector2Int(1, 2);
    [SerializeField] private Vector2Int m_SightDistanceRange = new Vector2Int(2, 15);
    [SerializeField] private Vector2Int m_FieldOfViewRange = new Vector2Int(45, 90);
    [SerializeField] private Vector2Int m_SmellingHearingRange = new Vector2Int(2, 4);
    [SerializeField] private Vector2 m_MoveTimeRange = new Vector2(2.0f, 5.0f);

    /* TODO: En los sitios en los que se usen estos valores como parte de una operaci�n
     * aritmetica, se deber�a de realizar una comprobaci�n, de si se desbordan los 
     * bit y en caso afirmativo, asignar el maximo o mininmo valor valor */
    public float MutationProbability { get => m_MutationProbability; private set => m_MutationProbability = value; }
    public float MaxMutationPct { get => m_MaxMutationPct; private set => m_MaxMutationPct = value; }
    public int IndividualsSpawnCount { get => m_IndividualsSpawnCount; private set => m_IndividualsSpawnCount = value; }
    public int RewardsSpawnCount { get => m_RewardsSpawnCount; private set => m_RewardsSpawnCount = value; }
    public float FloorRadius { get => m_FloorRadius; private set => m_FloorRadius = value; }
    public float MaxLifeTime { get => m_MaxLifeTime; private set => m_MaxLifeTime = value; }
    public float LifeTimeRegeneration { get => m_LifeTimeRegeneration; private set => m_LifeTimeRegeneration = value; }
    public Vector2Int VitalityRange { get => m_VitalityRange; private set => m_VitalityRange = value; }
    public Vector2Int SpeedRange { get => m_SpeedRange; private set => m_SpeedRange = value; }
    public Vector2Int StrengthRange { get => m_StrengthRange; private set => m_StrengthRange = value; }
    public Vector2Int EnergyRange { get => m_EnergyRange; private set => m_EnergyRange = value; }
    public Vector2Int SightDistanceRange { get => m_SightDistanceRange; private set => m_SightDistanceRange = value; }
    public Vector2Int FieldOfViewRange { get => m_FieldOfViewRange; private set => m_FieldOfViewRange = value; }
    public Vector2Int SmellingHearingRange { get => m_SmellingHearingRange; private set => m_SmellingHearingRange = value; }
    public Vector2 MoveTimeRange { get => m_MoveTimeRange; private set => m_MoveTimeRange = value; }
    public float KillReward { get => m_KillReward; private set => m_KillReward = value; }

    public void Awake()
    {
        if (Singleton == null)
        {
            ReproducibleRandom.InitState(m_RandomSeed);

            Singleton = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
