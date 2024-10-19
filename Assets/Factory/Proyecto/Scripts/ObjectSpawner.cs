using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    [SerializeField] private ObjectFactory _objectFactory;
    [SerializeField] private Transform spawnPoint;

    public void SpawnObject(string objectName)
    {
        Object ObjectToInstantiate = _objectFactory.CreateObject(objectName, spawnPoint);
        ObjectToInstantiate.Behaviour();
    }
}
