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

public class IndividualGatherState : IndividualState
{
    private Reward m_Reward;

    public IndividualGatherState(Individual individual, Reward reward) : base(individual)
    {
        m_Reward = reward;
    }

    public override void FixedUpdate()
    {
        Vector3 lookAt = m_Reward.transform.position;
        lookAt.y = GetIndividual().transform.position.y; // As� no mira ni para arriba ni para abajo

        GetIndividual().transform.LookAt(lookAt, Vector3.up);

        Vector3 direction = m_Reward.transform.position - GetIndividual().transform.position;
        direction.y = 0.0f;

        GetIndividual().transform.Translate(direction.normalized * Time.deltaTime * GetIndividual().Representation.GetRealSpeed(), Space.World);
    }

    public override State Update()
    {
        if (!GetIndividual().GetSeeing().Rewards.Contains(m_Reward) && !GetIndividual().GetSmellingHearing().Rewards.Contains(m_Reward))
            return new IndividualSearchState(GetIndividual());

        GetIndividual().RestoreEnergy();

        return this;
    }
}
