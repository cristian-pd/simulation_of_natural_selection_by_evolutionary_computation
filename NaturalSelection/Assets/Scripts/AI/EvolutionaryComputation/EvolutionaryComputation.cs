// Copyright Cristian Pagán Díaz. All Rights Reserved.

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(SimulationObjectPool))]
public abstract class EvolutionaryComputation : MonoBehaviour
{
    public const float k_FloorHeight = 0.05f;

    [SerializeField] private GameObject m_FloorPrefab;

    [HideInInspector] [SerializeField] private int m_Generation;

    private float m_LastTimeScale;

    private LinkedList<float[]> m_FitnessStats;
    private LinkedList<int[,]> m_RepresentationStats;
    private LinkedList<float[]> m_FitnessStatsMeans;
    private LinkedList<float[]> m_RepresentationStatsMeans;

    private void Awake()
    {
        m_Generation = 1;

        m_FitnessStats = new LinkedList<float[]>();
        m_RepresentationStats = new LinkedList<int[,]>();
        m_FitnessStatsMeans = new LinkedList<float[]>();
        m_RepresentationStatsMeans = new LinkedList<float[]>();

        OnAwake();
    }

    private void Start()
    {
        m_LastTimeScale = 1.0f;
        Time.timeScale = m_LastTimeScale;

        OnStart();
    }

    protected int GetGeneration()
    {
        return m_Generation;
    }

    protected void IncrementGeneration()
    {
        m_Generation++;
    }

    private void SpawnFloor()
    {
        GameObject floor = Instantiate(m_FloorPrefab);

        floor.transform.position = transform.position;
        float margin = 1.0f;
        float diameter = (Configuration.Singleton.FloorRadius + margin) * 2.0f;
        floor.transform.localScale = new Vector3(diameter, k_FloorHeight, diameter);
        floor.transform.SetParent(transform);

        floor.name = "Floor";
    }

    public void IncrementSimulationSpeed()
    {
        StopAllCoroutines();
        StartCoroutine(IncrementTimeScale(2.5f));
    }

    public void DecrementSimulationSpeed()
    {
        StopAllCoroutines();
        StartCoroutine(IncrementTimeScale(-2.5f));
    }

    public void PauseSimulation()
    {
        StopAllCoroutines();
        StartCoroutine(PauseTimeScale());
    }

    public void UnpauseSimulation()
    {
        StopAllCoroutines();
        StartCoroutine(UnpauseTimeScale());
    }

    private IEnumerator IncrementTimeScale(float value)
    {
        yield return new WaitForEndOfFrame();

        if (Time.timeScale == 1.0f)
        {
            if (value > 0.0f)
                Time.timeScale = value;
        }
        else
        {
            if (Time.timeScale + value < 1.0f)
            {
                Time.timeScale = 1.0f;
            }
            else
            {
                if (Time.timeScale + value <= 20.0f)
                    Time.timeScale += value;
            }
        }

        yield return null;
    }

    private IEnumerator PauseTimeScale()
    {
        yield return new WaitForEndOfFrame();

        m_LastTimeScale = Time.timeScale;
        Time.timeScale = 0.0f;

        yield return null;
    }

    private IEnumerator UnpauseTimeScale()
    {
        yield return new WaitForEndOfFrame();

        Time.timeScale = m_LastTimeScale;

        yield return null;
    }

    protected void SaveStats(Individual[] individuals)
    {
        float[] fitnessMeans = new float[5];

        fitnessMeans[0] = individuals[0].Fitness();
        for (int i = 0; i < individuals.Length; i++)
        {
            fitnessMeans[4] += individuals[i].Fitness() / individuals.Length;
            fitnessMeans[0] = individuals[i].Fitness() > fitnessMeans[0] ? individuals[i].Fitness() : fitnessMeans[0];
        }

        float[] fitness = new float[individuals.Length];
        for (int i = 0; i < fitness.Length; i++)
            fitness[i] = individuals[i].Fitness();
        
        Array.Sort(fitness);
        Array.Reverse(fitness);

        for (int i = 0; i < 10; i++)
            fitnessMeans[3] += fitness[i] / 10;

        for (int i = 0; i < 5; i++)
            fitnessMeans[2] += fitness[i] / 5;

        for (int i = 0; i < 3; i++)
            fitnessMeans[1] += fitness[i] / 3;

        Debug.Log(string.Format("{0} => Top1:{1} Top3;{2} Top5:{3} Top10:{4} Mean:{5}", GetGeneration(), fitnessMeans[0], fitnessMeans[1], fitnessMeans[2], fitnessMeans[3], fitnessMeans[4]));

        float[] individualsFitness = new float[individuals.Length];
        int[,] individualsRepresentations = new int[individuals.Length, IndividualRepresentation.k_RepresentationLength];
        float[] representationMeans = new float[IndividualRepresentation.k_RepresentationLength];

        for (int i = 0; i < individuals.Length; i++)
        {
            individualsFitness[i] = individuals[i].Fitness();

            int[] representation = individuals[i].Representation.GetRepresentation();
            for (int j = 0; j < representation.Length; j++)
            {
                individualsRepresentations[i, j] = representation[j];

                representationMeans[j] += (float)representation[j] / individuals.Length;
            }
        }

        m_FitnessStats.AddLast(individualsFitness);
        m_RepresentationStats.AddLast(individualsRepresentations);
        m_FitnessStatsMeans.AddLast(fitnessMeans);
        m_RepresentationStatsMeans.AddLast(representationMeans);
    }

    public void ExportToCSV()
    {
        int individuals = Configuration.Singleton.IndividualsSpawnCount;

        string header = "";
        for (int i = 0; i < individuals - 1; i++)
            header += "individual_" + i + ";";
        header += "individual_" + (individuals - 1) + "\n";

        string fitnessStats = "";
        foreach (var item in m_FitnessStats)
        {
            for (int i = 0; i < item.Length - 1; i++)
                fitnessStats += item[i] + ";";
            fitnessStats += item[item.Length - 1] + "\n";
        }

        string[] valNames = {
            "AggressivenessPct",
            "ScaryPct",
            "Speed",
            "MaxVitality",
            "Strength",
            "MaxEnergy",
            "SightDistance",
            "FieldOfViewDregrees",
            "SmellingHearingRadius",
            "SeeingPriorityPct",
        };

        string[] valStats = new string[valNames.Length];
        
        for (int j = 0; j < valNames.Length; j++)
        {
            foreach (var item in m_RepresentationStats)
            {
                for (int i = 0; i < individuals - 1; i++)
                    valStats[j] += item[i, j] + ";";
                valStats[j] += item[individuals - 1, j] + "\n";
            }
        }

        string fitnessMeans = "Best;Top3;Top5;Top10;Population\n";
        foreach (var item in m_FitnessStatsMeans)
        {
            for (int i = 0; i < item.Length - 1; i++)
                fitnessMeans += item[i] + ";";
            fitnessMeans += item[item.Length - 1] + "\n";
        }

        string repMeans = "";
        for (int i = 0; i < valNames.Length - 1; i++)
            repMeans += valNames[i] + ";";
        repMeans += valNames[valNames.Length - 1] + "\n";

        foreach (var item in m_RepresentationStatsMeans)
        {
            for (int i = 0; i < item.Length - 1; i++)
                repMeans += item[i] + ";";
            repMeans += item[item.Length - 1] + "\n";
        }

#if UNITY_EDITOR
        var folder = Application.streamingAssetsPath;

        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        var filesPath = Path.Combine(folder, Configuration.Singleton.name);
        var resultsPath = Path.Combine(filesPath, "results");
        var summaryPath = Path.Combine(filesPath, "summary");

        if (!Directory.Exists(filesPath))
        {
            Directory.CreateDirectory(filesPath);

            Directory.CreateDirectory(resultsPath);
            Directory.CreateDirectory(summaryPath);
        }
#endif
        filesPath = Path.Combine(resultsPath, "fitness.csv");
        using (var writer = new StreamWriter(filesPath, false))
        {
            writer.Write(header + fitnessStats);
        }

        for (int i = 0; i < valNames.Length; i++)
        {
            filesPath = Path.Combine(resultsPath, valNames[i] + ".csv");
            using (var writer = new StreamWriter(filesPath, false))
            {
                writer.Write(header + valStats[i]);
            }
        }

        filesPath = Path.Combine(summaryPath, "fitness_mean.csv");
        using (var writer = new StreamWriter(filesPath, false))
        {
            writer.Write(fitnessMeans);
        }

        filesPath = Path.Combine(summaryPath, "gen_rep_mean.csv");
        using (var writer = new StreamWriter(filesPath, false))
        {
            writer.Write(repMeans);
        }

#if UNITY_EDITOR
        AssetDatabase.Refresh();
#endif
    }

    protected virtual void OnAwake() { }

    protected virtual void OnStart()
    {
        SpawnFloor();
    }

    protected abstract void FirstSelection(Individual[] individuals);
    protected abstract void Cross(Individual[] individuals, Representation[] generation);
    protected abstract void Mute(Individual[] individuals, Representation[] generation);
    protected abstract void Combination(Individual[] individuals);
}
