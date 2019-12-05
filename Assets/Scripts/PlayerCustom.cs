using System;
using System.Collections;
using UnityEngine;

public class PlayerCustom : MonoBehaviour
{
    // Parameters Config
    [Header("Player Control")]
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float xPadding = 1f;
    [SerializeField] float yPadding = 1f;
    [SerializeField] float health = 1000f;
    [SerializeField] GameObject laserHitVFX;
    [SerializeField] GameObject enemyExplosionVFX;

    [Header("Projectile Control")]
    [SerializeField] GameObject laserPrefab;
    [SerializeField] GameObject laserFireVFX;
    [SerializeField] float fireVFXOffset = 1f;
    [SerializeField] float laserSpeed = 10;
    [SerializeField] float projectileFiringPeriod = 0.5f;

    [Header("Sound FX")]
    [SerializeField] AudioClip onDestroySoundClip;
    [SerializeField] [Range(0, 1)] float destroySoundVolume = 0.5f;
    [SerializeField] AudioClip onFireSoundClip;
    [SerializeField] [Range(0, 1)] float fireSoundVolume = 0.3f;
    [SerializeField] AudioClip onHitSoundClip;
    [SerializeField] [Range(0, 1)] float hitSoundVolume = 0.3f;

    // State
    float xMin, xMax, yMin, yMax;
    float initialHealth;
    Coroutine firingCoroutine;
    SceneLoader _sceneLoader;
    Joystick _joystick;
    ProgressBar _progressBar;
    

    // Start is called before the first frame update
    void Start()
    {
        _sceneLoader = FindObjectOfType<SceneLoader>();
        _joystick = FindObjectOfType<Joystick>();
        _progressBar = FindObjectOfType<ProgressBar>();

        if(_progressBar != null)
        {
            initialHealth = health;
            UpdateHealthBar(health);
        }

        SetupMoveBoundaries();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Fire();
    }

    private void Move()
    {
        // Check on Edit-Project Settings-Input for different options  - Mouse input
        //var deltaX = Input.GetAxis("Mouse X") * Time.deltaTime * moveSpeed;   
        //var deltaY = Input.GetAxis("Mouse Y") * Time.deltaTime * moveSpeed;

        // Joystick input
        var deltaX = _joystick.Horizontal * Time.deltaTime * moveSpeed;
        var deltaY = _joystick.Vertical * Time.deltaTime * moveSpeed;

        var newXPos = Mathf.Clamp(transform.position.x + deltaX, xMin, xMax);
        var newYPos = Mathf.Clamp(transform.position.y + deltaY, yMin, yMax);

        transform.position = new Vector2(newXPos, newYPos);
    }

    private void Fire()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            firingCoroutine = StartCoroutine(FireContinously());
        }
        
        if (Input.GetButtonUp("Fire1"))
        {
            StopCoroutine(firingCoroutine);
        }
    }

    // Check the boundaries based on camera size, so we can get the number ob Unity Units that correspond to the camera boundaries
    private void SetupMoveBoundaries()
    {
        Camera gameCamera = Camera.main;
        xMin = gameCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).x + xPadding;
        xMax = gameCamera.ViewportToWorldPoint(new Vector3(1, 0, 0)).x - xPadding;

        yMin = gameCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).y + yPadding;
        yMax = gameCamera.ViewportToWorldPoint(new Vector3(0, 1, 0)).y - yPadding;
    }

    private IEnumerator FireContinously()
    {
        while (true)
        {
            PlayFireEfects();
            GameObject laser = Instantiate(laserPrefab, transform.position, Quaternion.identity);
            laser.GetComponent<Rigidbody2D>().velocity = new Vector2(0, laserSpeed);

            yield return new WaitForSeconds(projectileFiringPeriod);
        }

    }

    private void PlayFireEfects()
    {
        AudioSource.PlayClipAtPoint(onFireSoundClip, Camera.main.transform.position, fireSoundVolume);
        Vector3 effectPosition = new Vector3(transform.position.x, transform.position.y + fireVFXOffset);
        GameObject sparkles = Instantiate(laserFireVFX, effectPosition, transform.rotation);
        Destroy(sparkles, 0.5f);
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
        UpdateHealthBar(health);

        if (health <= 0)
        {
            PlayerKilled();
        }
    }

    private void UpdateHealthBar(float currentHealth)
    {
        _progressBar.BarValue = currentHealth / initialHealth * 100;
    }

    private void PlayerKilled()
    {
        Destroy(gameObject);
        EnemyDestroyVFX();
        GameOverLoad();
    }

    private void GameOverLoad()
    {
        _sceneLoader.LoadGameOver();
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
