using UnityEngine;

public class TurretAI : MonoBehaviour 
{
    public enum TurretType
    {
        Single,
        Dual,
        Catapult
    }

    [Header("Turret stats")]
    [SerializeField] private TurretType turretType;
    [SerializeField] private PoolObjectType bulletType;
    [SerializeField] private float attackDistance;
    [SerializeField] private float attackDamage;
    [SerializeField] private float shotCoolDown;
    [SerializeField] private float lockSpeed;
    [SerializeField] private float targetCheckDelay;
    [SerializeField] private float maxRotationDegreesDeltaMultiplier;

    [Header("References")]
    [SerializeField] private Transform turretHead;
    [SerializeField] private Transform muzzleMain;
    [SerializeField] private Transform muzzleSub;
    [SerializeField] private GameObject muzzleEffect;
    [SerializeField] private GameObject bullet;

    #region private fields
    private float nextTimeToShoot;
    private GameObject currentTarget;
    private bool shootLeft = true;
    private Transform lockOnPosition;
    private Vector3 randomRotation;
    private Animator animator;
    #endregion

    void Start () 
    {
        InvokeRepeating(nameof(CheckForTarget), 0, targetCheckDelay);

        if (turretHead.TryGetComponent<Animator>(out Animator component))
        {
            animator = component;
        }

        randomRotation = new(0, Random.Range(0, 359), 0);

        nextTimeToShoot = Time.time;
    }
	
	void Update () 
    {
        if (currentTarget != null)
        {
            FollowTarget();

            if (GetDistanceToTarget() > attackDistance)
            {
                currentTarget = null;
            }
        }
        else
        {
            IdleRotate();
        }

        ShootCheckTrigger();

        if (Input.GetMouseButtonDown(0))
        {
            Transform player = GameObject.FindWithTag("Player").transform;
            ProjectileThrow(muzzleMain, player);
        }

	}

    private float GetDistanceToTarget()
    {
        float currentTargetDistance = Vector3.Distance(transform.position, currentTarget.transform.position);

        return currentTargetDistance;
    }

    private void ShootCheckTrigger()
    {
        if (currentTarget == null) return;

        if (nextTimeToShoot <= Time.time)
        {
            nextTimeToShoot = Time.time + shotCoolDown;

            Shoot(currentTarget);

            if (animator != null)
                animator.SetTrigger("Fire");
        }
    }

    private void CheckForTarget()
    {
        Collider[] collidersInRange = Physics.OverlapSphere(transform.position, attackDistance);

        for (int i = 0; i < collidersInRange.Length; i++)
        {
            if (collidersInRange[i].CompareTag("Player"))
            {
                currentTarget = collidersInRange[i].gameObject;
                break;
            }
        }
    }

    private void FollowTarget()
    {
        Vector3 targetDir = currentTarget.transform.position - turretHead.position;
        targetDir.y = 0;
        
        if (turretType == TurretType.Single)
        {
            turretHead.forward = targetDir;
        }
        else
        {
            turretHead.transform.rotation = Quaternion.RotateTowards(turretHead.rotation, 
                Quaternion.LookRotation(targetDir), 
                lockSpeed * Time.deltaTime);
        }
    }

    public void IdleRotate()
    {
        if (turretHead.rotation != Quaternion.Euler(randomRotation))
        {
            turretHead.rotation = Quaternion.RotateTowards(turretHead.transform.rotation, 
                Quaternion.Euler(randomRotation), 
                lockSpeed * Time.deltaTime * maxRotationDegreesDeltaMultiplier);
        }
        else
        {
            int randomAngle = Random.Range(0, 359);
            randomRotation = new(0, randomAngle, 0);
        }
    }

    public void Shoot(GameObject target)
    {
        if (turretType == TurretType.Catapult)
        {
            lockOnPosition = target.transform;
            ProjectileThrow(muzzleMain, lockOnPosition);
        }
        else if(turretType == TurretType.Dual)
        {
            if (shootLeft)
            {
                ProjectileThrow(muzzleMain, target.transform);
            }
            else
            {
                ProjectileThrow(muzzleSub, target.transform);
            }

            shootLeft = !shootLeft;
        }
        else
        {
            ProjectileThrow(muzzleMain, target.transform);
        }
    }

    private void ProjectileThrow(Transform shootPoint, Transform newTarget)
    {
        GameObject bullet = PoolManager.Instance.GetPooledObject(bulletType);
        Projectile projectile = bullet.GetComponent<Projectile>();
        projectile.Target = newTarget;        

        bullet.SetActive(true);
        bullet.transform.position = shootPoint.position;

        if (!(turretType == TurretType.Catapult))
            projectile.SetRotation();

        Instantiate(muzzleEffect, muzzleMain.transform.position, muzzleMain.rotation);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackDistance);
    }
}
