using System.Collections.Generic;
using UnityEngine;

public class ObjectFactory : MonoBehaviour
{
    [SerializeField] private Object[] objects;
    [SerializeField] private Transform objectsParent;

    private Dictionary<string, Object> objectsByName;

    private void Awake()
    {
        objectsByName = new Dictionary<string, Object>();

        foreach (var obj in objects)
        {
            objectsByName.Add(obj.ObjectName, obj);
        }
    }

    public Object CreateObject(string objectName, Transform playerTransform)
    {
        if (objectsByName.TryGetValue(objectName, out Object objectPrefab))
        {
            Object objectInstance = Instantiate(objectPrefab, playerTransform.position, Quaternion.identity, objectsParent);

            return objectInstance;
        }

        else
        {
            Debug.LogWarning($"El objeto '{objectName}' no existe en la base de datos de objetos.");

            return null;
        }
    }
}
