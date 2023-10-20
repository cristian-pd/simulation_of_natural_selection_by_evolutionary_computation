// Copyright Cristian Pagán Díaz. All Rights Reserved.

using UnityEngine;

public class IndividualRepresentation : Representation
{
    public const int k_RepresentationLength = 10;
    public const float k_SpeedIncrement = 2.0f;

    // Porcentajes independientes (Valores del 0 al 100)
    public int AggressivenessPct { get { return m_Representation[0]; } private set { m_Representation[0] = value; } }
    public int ScaryPct { get { return m_Representation[1]; } private set { m_Representation[1] = value; } }

    // Valores reales
    private int m_Speed { get { return m_Representation[2]; } set { m_Representation[2] = value; } }
    public int MaxVitality { get { return m_Representation[3]; } private set { m_Representation[3] = value; } }
    public int Strength { get { return m_Representation[4]; } private set { m_Representation[4] = value; } }
    public int MaxEnergy { get { return m_Representation[5]; } private set { m_Representation[5] = value; } }
    public int SightDistance { get { return m_Representation[6]; } private set { m_Representation[6] = value; } }
    public int FieldOfViewDregrees { get { return m_Representation[7]; } private set { m_Representation[7] = value; } }
    public int SmellingHearingRadius { get { return m_Representation[8]; } private set { m_Representation[8] = value; } }

    // Distribuci�n de probabilidad (Valores del 0 al 100)
    public int SeeingPriorityPct { get { return m_Representation[9]; } private set { m_Representation[9] = value; } }
    //public int SmellingHearingPriorityPct { get { return m_Representation[10]; } private set { m_Representation[10] = value; } } // No es necesaria a no se que se a�adan m�s sentidos a los individuos

    private int[] m_Representation;

    public IndividualRepresentation()
    {
        m_Representation = new int[k_RepresentationLength];

        for (int i = 0; i < m_Representation.Length; i++)
            InitRepresentationValue(i);
    }

    private void InitRepresentationValue(int index)
    {
        switch (index)
        {
            case 0:
                AggressivenessPct = ReproducibleRandom.Range(0, 100 + 1);
                break;
            case 1:
                ScaryPct = ReproducibleRandom.Range(0, 100 + 1);
                break;
            case 2:
                m_Speed = ReproducibleRandom.Range(Configuration.Singleton.SpeedRange.x, Configuration.Singleton.SpeedRange.y + 1);
                break;
            case 3:
                MaxVitality = ReproducibleRandom.Range(Configuration.Singleton.VitalityRange.x, Configuration.Singleton.VitalityRange.y + 1);
                break;
            case 4:
                Strength = ReproducibleRandom.Range(Configuration.Singleton.StrengthRange.x, Configuration.Singleton.StrengthRange.y + 1);
                break;
            case 5:
                MaxEnergy = ReproducibleRandom.Range(Configuration.Singleton.EnergyRange.x, Configuration.Singleton.EnergyRange.y + 1);
                break;
            case 6:
                SightDistance = ReproducibleRandom.Range(Configuration.Singleton.SightDistanceRange.x, Configuration.Singleton.SightDistanceRange.y + 1);
                break;
            case 7:
                FieldOfViewDregrees = ReproducibleRandom.Range(Configuration.Singleton.FieldOfViewRange.x, Configuration.Singleton.FieldOfViewRange.y + 1);
                break;
            case 8:
                SmellingHearingRadius = ReproducibleRandom.Range(Configuration.Singleton.SmellingHearingRange.x, Configuration.Singleton.SmellingHearingRange.y + 1);
                break;
            case 9:
                SeeingPriorityPct = ReproducibleRandom.Range(0, 100 + 1);
                break;
            default:
                break;
        }
    }

    private IndividualRepresentation(int[] representation)
    {
        m_Representation = representation;
    }

    public float GetRealSpeed()
    {
        float maxVitalityPct = MaxVitality - Configuration.Singleton.VitalityRange.x;
        maxVitalityPct /= Configuration.Singleton.VitalityRange.y - Configuration.Singleton.VitalityRange.x;

        return Mathf.Max(m_Speed * (1.0f - maxVitalityPct), Configuration.Singleton.SpeedRange.x);
    }

    public Color CalculateColor()
    {
        Color color1;

        float aggressivenessPct = AggressivenessPct / 100.0f;
        float scaryPct = ScaryPct / 100.0f;
        float runAwayPct = (1.0f - aggressivenessPct) * scaryPct; // La formula es as� porque se decide despues de agredir

        if (aggressivenessPct >= runAwayPct)
            color1 = Color.red;
        else
            color1 = Color.blue;

        float maxStats = 0.0f;
        maxStats += Configuration.Singleton.SpeedRange.y - Configuration.Singleton.SpeedRange.x;
        maxStats += Configuration.Singleton.VitalityRange.y - Configuration.Singleton.VitalityRange.x;
        maxStats += Configuration.Singleton.StrengthRange.y - Configuration.Singleton.StrengthRange.x;
        maxStats += Configuration.Singleton.EnergyRange.y - Configuration.Singleton.EnergyRange.x;

        float statsPct = 0.0f;
        statsPct += m_Speed - Configuration.Singleton.SpeedRange.x;
        statsPct += MaxVitality - Configuration.Singleton.VitalityRange.x;
        statsPct += Strength - Configuration.Singleton.StrengthRange.x;
        statsPct += MaxEnergy - Configuration.Singleton.EnergyRange.x;
        statsPct /= maxStats;

        float h, s, v;
        Color.RGBToHSV(color1, out h, out s, out v);
        s = statsPct;
        color1 = Color.HSVToRGB(h, s, v);

        float mainSensePct;
        if (SeeingPriorityPct >= 100 - SeeingPriorityPct)
        {
            mainSensePct = SeeingPriorityPct / 100.0f;

            float sightDistancePct = SightDistance - Configuration.Singleton.SightDistanceRange.x;
            sightDistancePct /= Configuration.Singleton.SightDistanceRange.y - Configuration.Singleton.SightDistanceRange.x;

            float fovPct = FieldOfViewDregrees - Configuration.Singleton.FieldOfViewRange.x;
            fovPct /= Configuration.Singleton.FieldOfViewRange.y - Configuration.Singleton.FieldOfViewRange.x;

            mainSensePct *= sightDistancePct * fovPct;
        }
        else
        {
            mainSensePct = (100 - SeeingPriorityPct) / 100.0f;
            mainSensePct *= SmellingHearingRadius - Configuration.Singleton.SmellingHearingRange.x;
            mainSensePct /= Configuration.Singleton.SmellingHearingRange.y - Configuration.Singleton.SmellingHearingRange.x;
        }

        Color color2 = Color.green;
        Color.RGBToHSV(color2, out h, out s, out v);
        s = mainSensePct;
        color2 = Color.HSVToRGB(h, s, v);

        return color1 + color2;
    }

    public override void Cross(Representation representation)
    {
        //// Codificaci�n entera
        //IndividualRepresentation other = (IndividualRepresentation)representation;

        //for (int i = 0; i < m_Representation.Length; i++)
        //{
        //    bool v = ReproducibleRandom.Range(0, 1 + 1) == 0 ? false : true;

        //    if (v)
        //    {
        //        int aux = m_Representation[i];
        //        m_Representation[i] = other.m_Representation[i];
        //        other.m_Representation[i] = aux;
        //    }
        //}

        // Aunque sean enteros lo hacemos como codificaci�n real
        IndividualRepresentation parent1 = this;
        IndividualRepresentation parent2 = (IndividualRepresentation)representation;

        for (int i = 0; i < m_Representation.Length; i++)
        {
            // Mascara en lugar de elegir punto de corte
            if (ReproducibleRandom.Range(0, 1 + 1) > 0)
            {
                float alpha = ReproducibleRandom.Range(0.0f, 1.0f);

                float child1 = alpha * parent1.m_Representation[i] + (1.0f - alpha) * parent2.m_Representation[i];
                float child2 = alpha * parent2.m_Representation[i] + (1.0f - alpha) * parent1.m_Representation[i];

                parent1.m_Representation[i] = Mathf.RoundToInt(child1);
                parent2.m_Representation[i] = Mathf.RoundToInt(child2);
            }
        }
    }

    public override void Mute()
    {
        float mutationProbabilty = Configuration.Singleton.MutationProbability;
        float maxMutationPct = Configuration.Singleton.MaxMutationPct;

        if (ReproducibleRandom.Range(0.0f, 1.0f) <= mutationProbabilty)
        {
            float mutationPct = ReproducibleRandom.Range(0.0f, maxMutationPct);
            int n = (int)(m_Representation.Length * mutationPct);
            for (int i = 0; i < n; i++)
            {
                int index = ReproducibleRandom.Range(0, m_Representation.Length);
                InitRepresentationValue(index);
            }
        }
    }

    public override object Clone()
    {
        return new IndividualRepresentation((int[])m_Representation.Clone());
    }

    public int[] GetRepresentation()
    {
        int[] representation = new int[m_Representation.Length];
        for (int i = 0; i < representation.Length; i++)
            representation[i] = m_Representation[i];

        return representation;
    }
}
