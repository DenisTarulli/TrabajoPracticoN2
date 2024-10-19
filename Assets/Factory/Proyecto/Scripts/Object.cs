using UnityEngine;

public abstract class Object : MonoBehaviour
{
    [SerializeField] protected float lifeTime;
    public abstract string ObjectName { get; }
    public abstract void Behaviour();
}
