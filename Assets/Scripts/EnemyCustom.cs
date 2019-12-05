using UnityEngine;

public class EnemyCustom : MonoBehaviour
{
    [Header("Enemy Control")]
    [SerializeField] int health = 100;
    [SerializeField] int killPoints = 50;
    [SerializeField] GameObject laserHitVFX;
    [SerializeField] GameObject enemyExplosionVFX;

    [Header("Projectile Control")]
    [SerializeField] float shotCounter;
    [SerializeField] float minTimeBetweenShoots = 0.2f;
    [SerializeField] float maxTimeBetweenShoots = 3f;
    [SerializeField] GameObject laserPrefab;
    [SerializeField] GameObject laserFireVFX;
    [SerializeField] float fireVFXOffset = 1f;
    [SerializeField] float laserSpeed = 10;

    [Header("Sound FX")]
    [SerializeField] AudioClip onDestroySoundClip;
    [SerializeField] [Range(0, 1)] float destroySoundVolume = 0.5f;
    [SerializeField] AudioClip onFireSoundClip;
    [SerializeField] [Range(0, 1)] float fireSoundVolume = 0.3f;
    [SerializeField] AudioClip onHitSoundClip;
    [SerializeField] [Range(0, 1)] float hitSoundVolume = 0.3f;


    GameSession _gameSession;

    // Start is called before the first frame update
    void Start()
    {
        _gameSession = FindObjectOfType<GameSession>();

        ResetShootCounter();
    }

    // Update is called once per frame
    void Update()
    {
        CountDownAndShoot();
    }

    private void CountDownAndShoot()
    {
        shotCounter -= Time.deltaTime;

        if (shotCounter <= 0f)
        {
            Fire();
            ResetShootCounter();
        }
    }

    private void Fire()
    {
        PlayFireEfects();
        GameObject laser = Instantiate(laserPrefab, transform.position, Quaternion.identity);
        laser.GetComponent<Rigidbody2D>().velocity = new Vector2(0, laserSpeed);
    }

    private void ResetShootCounter()
    {
        shotCounter = Random.Range(minTimeBetweenShoots, maxTimeBetweenShoots);
    }

    private void PlayFireEfects()
    {
        AudioSource.PlayClipAtPoint(onFireSoundClip, Camera.main.transform.position, fireSoundVolume);
        Vector3 effectPosition = new Vector3(transform.position.x, transform.position.y + fireVFXOffset);
        GameObject sparkles = Instantiate(laserFireVFX, effectPosition, transform.rotation);
        Destroy(sparkles, 1f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        DamageDealer damageDealer = collision.gameObject.GetComponent<DamageDealer>();

        if (!damageDealer) { return; }

        ProcessHit(damageDealer, collision.transform);
    }

    private void ProcessHit(DamageDealer damageDealer, Transform hitPosition)
    {
        LaserHitVFX(hitPosition);
        damageDealer.Hit(); // detsroy laser beam

        health -= damageDealer.GetDamage();

        if (health <= 0)
        {
            _gameSession.IncreaseScore(killPoints);
            Destroy(gameObject);
            EnemyDestroyVFX();
        }
    }

    private void EnemyDestroyVFX()
    {
        AudioSource.PlayClipAtPoint(onDestroySoundClip, Camera.main.transform.position, destroySoundVolume);
        Vector3 effectPosition = new Vector3(transform.position.x, transform.position.y);
        Instantiate(enemyExplosionVFX, effectPosition, transform.rotation);
    }

    private void LaserHitVFX(Transform hitPosition)
    {
        AudioSource.PlayClipAtPoint(onHitSoundClip, Camera.main.transform.position, hitSoundVolume);
        Vector3 effectPosition = new Vector3(hitPosition.position.x, hitPosition.position.y);
        Instantiate(laserHitVFX, effectPosition, transform.rotation);
    }
}
