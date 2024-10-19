using UnityEngine;

public class Arbol : Object
{
    public override string ObjectName => "Arbol";

    private void Update()
    {
        transform.localScale += Vector3.one * Time.deltaTime;
    }

    public override void Behaviour()
    {
        Destroy(gameObject, lifeTime);
    }
}
