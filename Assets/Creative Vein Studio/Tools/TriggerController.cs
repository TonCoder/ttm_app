using UnityEngine;
using UnityEngine.Events;

public class TriggerController : MonoBehaviour
{
    public GameObject objectToCheckFor;

    public UnityEvent OnEnter;
    public UnityEvent OnStay;
    public UnityEvent OnExit;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(objectToCheckFor.tag))
        {
            OnEnter?.Invoke();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(objectToCheckFor.tag))
        {
            OnStay?.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(objectToCheckFor.tag))
        {
            OnExit?.Invoke();
        }
    }
}