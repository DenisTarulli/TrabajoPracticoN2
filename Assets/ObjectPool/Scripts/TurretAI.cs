using UnityEngine;

public class TurretAI : MonoBehaviour 
{
    public enum TurretType
    {
        Single = 1,
        Dual = 2,
        Catapult = 3,
    }

    [Header("Turret stats")]
    [SerializeField] private TurretType turretType = TurretType.Single;
    [SerializeField] private float attackDistance;
    [SerializeField] private float attackDamage;
    [SerializeField] private float shotCoolDown;
    private float nextTimeToShoot;
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
    private GameObject currentTarget;
    private bool shootLeft = true;
    private Transform lockOnPos;
    private Vector3 randomRotation;
    private Animator animator;
    #endregion

    void Start () 
    {
        InvokeRepeating(nameof(CheckForTarget), 0, targetCheckDelay);

        if (transform.GetChild(0).TryGetComponent<Animator>(out Animator component))
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

            float currentTargetDistance = Vector3.Distance(transform.position, currentTarget.transform.position);

            if (currentTargetDistance > attackDistance)
            {
                currentTarget = null;
            }
        }
        else
        {
            IdleRotate();
        }

        ShootCheckTrigger();
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
            lockOnPos = target.transform;
            //Aplicar POOL OBJECT
            Instantiate(muzzleEffect, muzzleMain.transform.position, muzzleMain.rotation);

            GameObject bullet = PoolManager.Instance.GetPooledObject(PoolObjectType.CatapultBullet);
            bullet.SetActive(true);
            bullet.transform.SetPositionAndRotation(muzzleMain.position, muzzleMain.rotation);
            Projectile projectile = bullet.GetComponent<Projectile>();
            projectile.target = lockOnPos;
        }
        else if(turretType == TurretType.Dual)
        {
            if (shootLeft)
            {
                //Aplicar POOL OBJECT
                Instantiate(muzzleEffect, muzzleMain.transform.position, muzzleMain.rotation);
                GameObject missleGo = Instantiate(bullet, muzzleMain.transform.position, muzzleMain.rotation);
                Projectile projectile = missleGo.GetComponent<Projectile>();
                projectile.target = transform.GetComponent<TurretAI>().currentTarget.transform;
            }
            else
            {
                //Aplicar POOL OBJECT
                Instantiate(muzzleEffect, muzzleSub.transform.position, muzzleSub.rotation);
                GameObject missleGo = Instantiate(bullet, muzzleSub.transform.position, muzzleSub.rotation);
                Projectile projectile = missleGo.GetComponent<Projectile>();
                projectile.target = transform.GetComponent<TurretAI>().currentTarget.transform;
            }

            shootLeft = !shootLeft;
        }
        else
        {
            //Aplicar POOL OBJECT
            Instantiate(muzzleEffect, muzzleMain.transform.position, muzzleMain.rotation);
            GameObject missleGo = Instantiate(bullet, muzzleMain.transform.position, muzzleMain.rotation);
            Projectile projectile = missleGo.GetComponent<Projectile>();
            projectile.target = currentTarget.transform;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackDistance);
    }
}
