/*
 * All Rights Reserved
 * 
 * Copyright (c) 2023 Cristian Pagán Díaz
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GeneticAlgorithm))]
public class GeneticAlgorithmEditor : Editor
{
    private bool m_Pause;
    private EvolutionaryComputation m_Target;
    SerializedProperty m_Generation;

    private void OnEnable()
    {
        m_Target = (EvolutionaryComputation)target;
        m_Generation = serializedObject.FindProperty("m_Generation");
        m_Pause = false;
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.LabelField("Simulation setup", EditorStyles.boldLabel);

        base.OnInspectorGUI();

        EditorGUILayout.LabelField("Simulation control", EditorStyles.boldLabel);

        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.PropertyField(m_Generation);
        EditorGUI.EndDisabledGroup();

        if (GUILayout.Button("Increment Speed"))
            m_Target.IncrementSimulationSpeed();

        if (GUILayout.Button("Decrement Speed"))
            m_Target.DecrementSimulationSpeed();

        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.FloatField("Simulation speed", Time.timeScale);
        EditorGUI.EndDisabledGroup();

        if (!m_Pause)
        {
            if (GUILayout.Button("Pause"))
            {
                m_Pause = true;
                m_Target.PauseSimulation();
            }
        }
        else
        {
            if (GUILayout.Button("Unpause"))
            {
                m_Pause = false;
                m_Target.UnpauseSimulation();
            }
        }

        if (GUILayout.Button("Export to CSV"))
            m_Target.ExportToCSV();
    }
}
