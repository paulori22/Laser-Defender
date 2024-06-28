using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float moveBoundriePaddingX = 0.6f;
    [SerializeField] float moveBoundriePaddingY = 0.5f;
    [SerializeField] float health = 200;
    [SerializeField] AudioClip deathSound;
    [SerializeField][Range(0, 1)] float deathSoundVolume = 0.75f;

    [Header("Projectile")]
    [SerializeField] GameObject laserPrefab;
    [SerializeField] float laserSpeed = 10f;
    [SerializeField] float laserFiringPeriod = 0.1f;

    [Header("Sound Effects")]
    [SerializeField] AudioClip shootSound;
    [SerializeField][Range(0, 1)] float shootSoundVolume = 0.25f;


    Coroutine firingCoroutine;
    float xMin;
    float xMax;
    float yMin;
    float yMax;

    // Start is called before the first frame update
    void Start()
    {
        SetUpMoveBoundries();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Fire();
    }

    private void Fire()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            firingCoroutine = StartCoroutine(FireContinuously());
        }
        if (Input.GetButtonUp("Fire1"))
        {
            StopCoroutine(firingCoroutine);
        }
    }

    IEnumerator FireContinuously()
    {
        while (true)
        {
            GameObject laser = Instantiate(
                laserPrefab,
                transform.position,
                Quaternion.identity
            );
            laser.GetComponent<Rigidbody2D>().velocity = new Vector2(0, laserSpeed);
            TriggerLaserSoundEffect();
            yield return new WaitForSeconds(laserFiringPeriod);
        }
    }

    private void TriggerLaserSoundEffect()
    {
        AudioSource.PlayClipAtPoint(shootSound, Camera.main.transform.position, shootSoundVolume);
    }

    private void Move()
    {
        var factor = Time.deltaTime * moveSpeed;
        var deltaX = Input.GetAxis("Horizontal") * factor;
        var deltaY = Input.GetAxis("Vertical") * factor;

        var newXPos = Mathf.Clamp(transform.position.x + deltaX, xMin, xMax);
        var newYPos = Mathf.Clamp(transform.position.y + deltaY, yMin, yMax);
        transform.position = new Vector2(newXPos, newYPos);
    }

    private void SetUpMoveBoundries()
    {
        Camera gameCamera = Camera.main;
        xMin = gameCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).x + moveBoundriePaddingX;
        xMax = gameCamera.ViewportToWorldPoint(new Vector3(1, 0, 0)).x - moveBoundriePaddingX;
        yMin = gameCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).y + moveBoundriePaddingY;
        yMax = gameCamera.ViewportToWorldPoint(new Vector3(0, 1, 0)).y - moveBoundriePaddingY;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        DamageDealer damageDealer = other.GetComponent<DamageDealer>();
        if (damageDealer)
        {
            ProcessHit(damageDealer);
        }
    }

    private void ProcessHit(DamageDealer damageDealer)
    {
        health -= damageDealer.GetDamage();
        damageDealer.Hit();
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        TriggerDeathSoundEffect();
        Destroy(gameObject);
        LoadGameOver();
    }

    private void TriggerDeathSoundEffect()
    {
        AudioSource.PlayClipAtPoint(deathSound, Camera.main.transform.position, deathSoundVolume);
    }

    private void LoadGameOver()
    {
        FindObjectOfType<Level>().LoadGameOver();
    }

    public float GetHealth()
    {
        return health;
    }
}
