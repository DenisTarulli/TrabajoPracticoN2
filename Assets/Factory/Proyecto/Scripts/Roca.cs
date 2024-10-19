using UnityEngine;

public class Roca : Object
{
    public override string ObjectName => "Roca";

    private void Update()
    {
        transform.localScale += Vector3.one * Time.deltaTime;
    }

    public override void Behaviour()
    {
        Destroy(gameObject, lifeTime);
    }
}
