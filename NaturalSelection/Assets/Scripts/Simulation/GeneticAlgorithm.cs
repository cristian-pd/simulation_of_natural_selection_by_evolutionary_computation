// Copyright Cristian Pagán Díaz. All Rights Reserved.

public class GeneticAlgorithm : EvolutionaryComputation
{
    private class GenerationElement : System.IComparable
    {
        public IndividualRepresentation IndividualRepresentation;
        public float Fitness;

        public int CompareTo(object obj)
        {
            GenerationElement other = (GenerationElement)obj;

            float a = Fitness;
            float b = other.Fitness;

            if (a < b)
                return -1;
            else if (a > b)
                return 1;
            else
                return 0;
        }
    }

    private GenerationElement[] m_LastGeneration;

    protected override void OnStart()
    {
        base.OnStart();

        object[] args = { };
        SendMessage("Init", args);

        SendMessage("Spawn");
    }

    protected override void FirstSelection(Individual[] individuals)
    {
        GenerationElement[] generation = new GenerationElement[individuals.Length];
        for (int i = 0; i < generation.Length; i++)
        {
            GenerationElement generationElement = new GenerationElement();
            generationElement.IndividualRepresentation = (IndividualRepresentation)individuals[i].Representation.Clone();
            generationElement.Fitness = individuals[i].Fitness();

            generation[i] = generationElement;
        }

        System.Array.Sort(generation, (a, b) => {
            return a.CompareTo(b);
        });
        System.Array.Reverse(generation);

        Selection(individuals, generation);
    }

    private void Selection(Individual[] individuals, GenerationElement[] generation)
    {
        SaveStats(individuals);

        IncrementGeneration();
        

        int n = individuals.Length;

        // Distribuci�n de probabilidad
        float[] ps = new float[n];
        for (int i = 0; i < n; i++)
            ps[i] = 2.0f * (n - i) / (n * n + n);

        // Porcentajes acumulados
        float[] p = new float[n];
        p[0] = ps[0];
        for (int i = 1; i < n; i++)
            p[i] = p[i - 1] + ps[i];

        m_LastGeneration = new GenerationElement[n];
        for (int i = 0; i < n; i++)
        {
            GenerationElement generationElement = new GenerationElement();
            generationElement.IndividualRepresentation = (IndividualRepresentation)generation[i].IndividualRepresentation.Clone();
            generationElement.Fitness = generation[i].Fitness;

            m_LastGeneration[i] = generationElement;
        }

        // Seleccionamos utilizando la distrubuci�n de probabilidad
        IndividualRepresentation[] newGeneration = new IndividualRepresentation[n];
        for (int i = 0; i < n; i++)
        {
            float u = ReproducibleRandom.Range(0.0f, 1.0f);
            for (int j = 0; j < n; j++)
            {
                if (u <= p[j])
                {
                    newGeneration[i] = (IndividualRepresentation)generation[j].IndividualRepresentation.Clone();
                    break;
                }
            }
        }

        Cross(individuals, newGeneration);
    }

    protected override void Cross(Individual[] individuals, Representation[] generation)
    {
        // Posibilidad de cruze del 100% por tanto se omite la generaci�n de n�meros aleatorios

        int n = generation.Length;
        if (generation.Length % 2 != 0)
            n--;

        for (int i = 0; i < n; i += 2)
            generation[i].Cross(generation[i+1]);

        Mute(individuals, generation);
    }

    protected override void Mute(Individual[] individuals, Representation[] generation)
    {
        for (int i = 0; i < generation.Length; i++)
            generation[i].Mute();

        for (int i = 0; i < generation.Length; i++)
            individuals[i].Representation = (IndividualRepresentation)generation[i];

        SendMessage("Spawn");
    }

    protected override void Combination(Individual[] individuals)
    {
        // Por truncamiento
        GenerationElement[] generationUnion = new GenerationElement[individuals.Length * 2];

        for (int i = 0; i < individuals.Length; i++)
        {
            GenerationElement generationElement = new GenerationElement();
            generationElement.IndividualRepresentation = (IndividualRepresentation)m_LastGeneration[i].IndividualRepresentation.Clone();
            generationElement.Fitness = m_LastGeneration[i].Fitness;

            generationUnion[i] = generationElement;
        }

        for (int i = 0; i < individuals.Length; i++)
        {
            GenerationElement generationElement = new GenerationElement();
            generationElement.IndividualRepresentation = (IndividualRepresentation)individuals[i].Representation.Clone();
            generationElement.Fitness = individuals[i].Fitness();

            generationUnion[individuals.Length + i] = generationElement;
        }

        System.Array.Sort(generationUnion, (a, b) => {
            return a.CompareTo(b);
        });
        System.Array.Reverse(generationUnion);

        GenerationElement[] generation = new GenerationElement[individuals.Length];
        for (int i = 0; i < individuals.Length; i++)
            generation[i] = generationUnion[i];

        Selection(individuals, generation);
    }
}
