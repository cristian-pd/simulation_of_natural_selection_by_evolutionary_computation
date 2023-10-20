// Copyright Cristian Pagán Díaz. All Rights Reserved.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public abstract class Senses : MonoBehaviour
{
    [SerializeField] protected Individual m_Individual;

    public LinkedList<Individual> Individuals { get; private set; }
    public LinkedList<Reward> Rewards { get; private set; }

    protected SphereCollider m_SphereCollider;

    private void Awake()
    {
        ResetSense();
    }

    private void Start()
    {
        m_SphereCollider = GetComponent<SphereCollider>();

        OnSenseStart();
    }

    /* Se hace todo esto por evitar el OnTriggerStay(). El OnTriggerStay() es muy 
     * ineficiente cuando hay muchos individuos en la simulaci�n y como 
     * son llamadas que hace Unity internamente poco puedo controlar */
    private void FixedUpdate()
    {
        /* Creo que si usara DOTS eliminaria todos los problemas
         * de rendimiento que sigue causando esta llamada,
         * de hecho si usara DOTS creo que seria incluso buena idea
         * usar el evento OnTriggerStay */
        StartCoroutine(OnProccesing());
    }

    private IEnumerator OnProccesing()
    {
        yield return new WaitForFixedUpdate();
        OnSenseProcessing();
        yield return null;
    }

    private void OnTriggerEnter(Collider other)
    {
        Individual individual = other.GetComponent<Individual>();
        Reward reward = other.GetComponent<Reward>();

        if (individual != null && individual != m_Individual)
            OnSenseEnter(individual);
        else if (reward != null)
            OnSenseEnter(reward);
    }

    private void OnEnable()
    {
        /* Evitamos que se llame antes de llamar a su m�todo Start al menos una
         * vez ya que en este momento todavia no se ha terminado de inicializar el collider */
        if(m_SphereCollider != null)
            SenseOnEnable();
    }

    private void OnTriggerExit(Collider other)
    {
        Individual individual = other.GetComponent<Individual>();
        Reward reward = other.GetComponent<Reward>();

        if (individual != null && individual != m_Individual)
            OnSenseExit(individual);
        else if (reward != null)
            OnSenseExit(reward);
    }

    protected virtual void OnSenseStart() { }

    // Se ejecuta entre el Update y el OnTriggerX
    protected virtual void OnSenseProcessing()
    {
        RemoveInactive(Individuals);
        RemoveInactive(Rewards);
    }

    protected void RemoveInactive<T>(LinkedList<T> list) where T : MonoBehaviour
    {
        var currentNode = list.First;
        while (currentNode != null)
        {
            var nextNode = currentNode.Next;

            if (!currentNode.Value.gameObject.activeSelf)
                list.Remove(currentNode);

            currentNode = nextNode;
        }
    }

    protected virtual void OnSenseEnter(Individual other) { }
    protected virtual void OnSenseExit(Individual other) { }
    protected virtual void OnSenseEnter(Reward other) { }
    protected virtual void OnSenseExit(Reward other) { }

    public virtual void ResetSense()
    {
        Individuals = new LinkedList<Individual>();
        Rewards = new LinkedList<Reward>();
    }

    protected abstract void SenseOnEnable();
}
