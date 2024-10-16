using UnityEngine;

public class Projectile : MonoBehaviour 
{
    [Header("Type")]
    [SerializeField] private TurretAI.TurretType turretType;
    [SerializeField] private PoolObjectType objectType;
    [SerializeField] private bool catapult;

    [Header("Stats")]
    [SerializeField] private float shotSpeed;
    [SerializeField] private float shotSpeedMultiplier;
    [SerializeField] private float gravityCompensationMultiplier;
    [SerializeField] private float turnSpeed;
    [SerializeField] private float knockback;

    [Header("Explosion")]
    [SerializeField] private float maxExplosionTimer;
    [SerializeField] private float explosionTriggerHeight;
    [SerializeField] private ParticleSystem explosionEffect;

    #region extra fields
    private bool lockOnTarget;
    private float explosionTimer;
    private Transform target;
    public Transform Target { get => target; set => target = value; }
    #endregion

    private void OnEnable()
    {
        explosionTimer = maxExplosionTimer;

        if (catapult)
        {
            lockOnTarget = true;
        }
        if (turretType == TurretAI.TurretType.Single)
        {
            transform.rotation = Quaternion.LookRotation(GetDirectionToTarget());
        }
    }

    private void Update()
    {
        explosionTimer -= Time.deltaTime;

        if (CheckExplosionConditions())
        {
            Explosion();
            return;
        }

        if (turretType == TurretAI.TurretType.Catapult)
        {
            if (lockOnTarget)
            {
                Vector3 newVelocity = CalculateCatapultProjectileVelocity(target.transform.position, transform.position, 1);

                transform.GetComponent<Rigidbody>().velocity = newVelocity;
                lockOnTarget = false;
            }
        }
        else if(turretType == TurretAI.TurretType.Dual)
        {
            Vector3 directionToTarget = target.position - transform.position;
            Vector3 newDirection = Vector3.RotateTowards(transform.forward, directionToTarget, Time.deltaTime * turnSpeed, 0.0f);
            Debug.DrawRay(transform.position, newDirection, Color.red);

            transform.Translate(shotSpeed * Time.deltaTime * Vector3.forward);
            transform.rotation = Quaternion.LookRotation(newDirection);

        }
        else if (turretType == TurretAI.TurretType.Single)
        {
            float singleShotSpeed = shotSpeed * Time.deltaTime;
            transform.Translate(shotSpeedMultiplier * singleShotSpeed * transform.forward, Space.World);
        }
    }

    private Vector3 GetDirectionToTarget()
    {
        if (target == null)
            return Vector3.zero;

        Vector3 newDirection = target.position - transform.position;

        return newDirection.normalized;
    }

    public void SetRotation()
    {
        transform.rotation = Quaternion.LookRotation(GetDirectionToTarget());
    }

    private bool CheckExplosionConditions()
    {
        return target == null || transform.position.y < explosionTriggerHeight || explosionTimer < 0;
    }

    private Vector3 CalculateCatapultProjectileVelocity(Vector3 target, Vector3 origen, float time)
    {
        Vector3 distanceToTarget = target - origen;
        distanceToTarget.y = 0;

        float velocityMultiplier = distanceToTarget.magnitude / time;
        float yVelocity = gravityCompensationMultiplier * Mathf.Abs(Physics.gravity.y) * time;

        Vector3 result = distanceToTarget.normalized;

        result *= velocityMultiplier;
        result.y = yVelocity;

        return result;
    }

    public void Explosion()
    {
        Instantiate(explosionEffect, transform.position, transform.rotation);
        PoolManager.Instance.CoolObject(gameObject, objectType);
    }

    private void KnockbackPlayer(Transform player)
    {
        Vector3 knockbackDirection = player.position - transform.position;
        Vector3 playerNewPosition = player.position + (knockbackDirection.normalized * knockback);
        playerNewPosition.y = 1;
        player.position = playerNewPosition;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.transform.CompareTag("Player")) return;

        KnockbackPlayer(other.transform);
        Explosion();
    }
}
