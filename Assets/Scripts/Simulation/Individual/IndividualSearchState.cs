// Copyright Cristian Pagán Díaz. All Rights Reserved.

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class IndividualSearchState : IndividualState
{
    private enum IndividualsDecisionType { Attack, RunAway }

    public IndividualSearchState(Individual individual) : base(individual) { }

    public override State Update()
    {
        GetIndividual().RestoreEnergy();

        int randomPct = ReproducibleRandom.Range(0, 100);
        if (randomPct >= 0 && randomPct < GetIndividual().Representation.AggressivenessPct)
        {
            return IndividualsDecision(IndividualsDecisionType.Attack);
        }
        else
        {
            randomPct = ReproducibleRandom.Range(0, 100);
            if (randomPct >= 0 && randomPct < GetIndividual().Representation.ScaryPct)
                return IndividualsDecision(IndividualsDecisionType.RunAway);
            else
                return RewardDecision();
        }
    }

    private State IndividualsDecision(IndividualsDecisionType individualsDecisionType)
    {
        List<Individual> seeingIndividuals = GetIndividual().GetSeeing().Individuals.ToList();
        List<Individual> smellingHearingIndividuals = GetIndividual().GetSmellingHearing().Individuals.ToList();
        List<Individual> individualsIntersection = seeingIndividuals.Intersect(smellingHearingIndividuals).ToList();

        if (individualsIntersection.Count > 0)
        {
            return new IndividualAttackState(GetIndividual(), GetClosest(individualsIntersection));
        }
        else
        {
            int seeingPriorityPct = GetIndividual().Representation.SeeingPriorityPct;
            //int smellingHearingPriorityPct = GetIndividual().Representation.SmellingHearingPriorityPct;

            int randomPct = ReproducibleRandom.Range(0, 100);
            if (randomPct >= 0 && randomPct < seeingPriorityPct)
            {
                if (seeingIndividuals.Count > 0)
                    return IndividualsDecision(individualsDecisionType, GetClosest(seeingIndividuals));
                else if (ReproducibleRandom.Range(0, 100) < (100 - seeingPriorityPct) && smellingHearingIndividuals.Count > 0)
                    return IndividualsDecision(individualsDecisionType, GetClosest(smellingHearingIndividuals));
                else
                    return RewardDecision();
            }
            else // if (randomPct >= seeingPriorityPct && randomPct < seeingPriorityPct + smellingHearingPriorityPct)
            {
                if (smellingHearingIndividuals.Count > 0)
                    return IndividualsDecision(individualsDecisionType, GetClosest(smellingHearingIndividuals));
                else if (ReproducibleRandom.Range(0, 100) < seeingPriorityPct && seeingIndividuals.Count > 0)
                    return IndividualsDecision(individualsDecisionType, GetClosest(seeingIndividuals));
                else
                    return RewardDecision();
            }
        }
    }

    private State IndividualsDecision(IndividualsDecisionType type, Individual selectedIndividual)
    {
        if (type == IndividualsDecisionType.Attack)
            return new IndividualAttackState(GetIndividual(), selectedIndividual);
        else // if (type == IndividualsDecisionType.RunAway)
            return new IndividualRunAwayState(GetIndividual(), selectedIndividual);
    }

    private State RewardDecision()
    {
        List<Reward> seeingRewards = GetIndividual().GetSeeing().Rewards.ToList();
        List<Reward> smellingHearingRewards = GetIndividual().GetSmellingHearing().Rewards.ToList();
        List<Reward> rewardsIntersection = seeingRewards.Intersect(smellingHearingRewards).ToList();

        if (rewardsIntersection.Count > 0)
        {
            return new IndividualGatherState(GetIndividual(), GetClosest(rewardsIntersection));
        }
        else
        {
            int seeingPriorityPct = GetIndividual().Representation.SeeingPriorityPct;
            //int smellingHearingPriorityPct = GetIndividual().Representation.SmellingHearingPriorityPct;

            int randomPct = ReproducibleRandom.Range(0, 100);
            if (randomPct >= 0 && randomPct < seeingPriorityPct)
            {
                if (seeingRewards.Count > 0)
                    return new IndividualGatherState(GetIndividual(), GetClosest(seeingRewards));
                else if (ReproducibleRandom.Range(0, 100) < (100 - seeingPriorityPct) && smellingHearingRewards.Count > 0)
                    return new IndividualGatherState(GetIndividual(), GetClosest(smellingHearingRewards));
                else
                    return new IndividualMoveState(GetIndividual());
            }
            else // if (randomPct >= seeingPriorityPct && randomPct < seeingPriorityPct + smellingHearingPriorityPct)
            {
                if (smellingHearingRewards.Count > 0)
                    return new IndividualGatherState(GetIndividual(), GetClosest(smellingHearingRewards));
                else if (ReproducibleRandom.Range(0, 100) < seeingPriorityPct && seeingRewards.Count > 0)
                    return new IndividualGatherState(GetIndividual(), GetClosest(seeingRewards));
                else
                    return new IndividualMoveState(GetIndividual());
            }
        }
    }

    private T GetClosest<T>(List<T> list) where T : MonoBehaviour
    {
        Vector3 position = GetIndividual().transform.position;

        T closestIndividual = list.First();
        float closestDistance = Vector3.Distance(position, closestIndividual.transform.position);

        foreach (T individual in list)
        {
            float distance = Vector3.Distance(position, individual.transform.position);
            if (distance < closestDistance)
            {
                closestIndividual = individual;
                closestDistance = distance;
            }
        }

        return closestIndividual;
    }
}
