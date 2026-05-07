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

public class IndividualMoveState : IndividualState
{
    private float m_Timer;
    private Vector3 m_Dst;

    public IndividualMoveState(Individual individual) : base(individual)
    {
        m_Timer = ReproducibleRandom.Range(Configuration.Singleton.MoveTimeRange.x, Configuration.Singleton.MoveTimeRange.y);

        float angle = ReproducibleRandom.Range(0.0f, 360.0f);
        float distance = ReproducibleRandom.Range(0.0f, Configuration.Singleton.FloorRadius);

        m_Dst = new Vector3(distance * Mathf.Cos(angle), 0.0f, distance * Mathf.Sin(angle));

        Vector3 lookAt = m_Dst;
        lookAt.y = GetIndividual().transform.position.y; // As� no mira ni para arriba ni para abajo

        GetIndividual().transform.LookAt(lookAt, Vector3.up);
    }

    public override void FixedUpdate()
    {
        Vector3 dir = m_Dst - GetIndividual().transform.position;
        dir.y = 0.0f;

        GetIndividual().transform.Translate(dir.normalized * Time.deltaTime * GetIndividual().Representation.GetRealSpeed(), Space.World);
    }

    public override State Update()
    {
        if (m_Timer <= 0.0f)
            return new IndividualSearchState(GetIndividual());

        if (Vector3.Distance(GetIndividual().transform.position, m_Dst) <= 1.0f)
            return new IndividualSearchState(GetIndividual());

        GetIndividual().RestoreEnergy();

        m_Timer -= Time.deltaTime;

        return this;
    }
}
