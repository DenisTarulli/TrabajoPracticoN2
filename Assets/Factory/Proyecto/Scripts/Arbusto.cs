using UnityEngine;

public class Arbusto : Object
{
    public override string ObjectName => "Arbusto";

    private void Update()
    {
        transform.localScale += Vector3.one * Time.deltaTime;
    }

    public override void Behaviour()
    {
        Destroy(gameObject, lifeTime);
    }
}
