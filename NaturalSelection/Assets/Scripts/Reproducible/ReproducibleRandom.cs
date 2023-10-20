// Copyright Cristian Pagán Díaz. All Rights Reserved.

using System;

/* Al ser una aplicaci�n dependiente del tiempo de frame,
 * y de el orden, en el que unity detecta los eventos,
 * cosa que no podemos controlar, no es posible hacer los 
 * experimentos reproducibles, ni siquiera, fijando el orden 
 * de ejecuci�n de los scripts, ni creando nuestra propia
 * clase para la generaci�n de n�meros aleatorios, 
 * ni evitando eventos de frame con ciclos, como el FixedUpdate */

/* Aunque por supuesto el problema de la reproducibilidad tiene 
 * solución, tan solo hay que eliminar la multiplicación por 
 * Time.deltaTime en todos los scripts del proyecto, esto provocaría
 * que la simulación fuese más difícil de visualizar (cosa que no 
 * importa mucho, ya que lo importante son los resultados), además 
 * ahora la simulación aprovecharía al máximo el hardware por lo que 
 * sería más rápida */

public static class ReproducibleRandom
{
    private static Random m_Random;

    public static void InitState(int seed)
    {
        m_Random = new Random(seed);
    }

    public static float Range(float minInc, float maxInc)
    {
        return (float)m_Random.NextDouble() * (maxInc - minInc) + minInc;
    }

    public static int Range(int minInc, int maxExc)
    {
        return m_Random.Next(minInc, maxExc);
    }
}
