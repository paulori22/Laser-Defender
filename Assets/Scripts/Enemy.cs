using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Stats")]
    [SerializeField] float health = 100;
    [SerializeField] int scoreValue = 100;

    [Header("Shooting")]
    float shotCounter;
    [SerializeField] float minTimeBetweenShots = 0.2f;
    [SerializeField] float maxTimeBetweenShots = 3f;
    [SerializeField] GameObject laserPrefab;
    [SerializeField] float laserSpeed = 3f;

    [Header("Sound Effects")]
    [SerializeField] GameObject deathVFS;
    [SerializeField] float durationOfExplosion = 1f;
    [SerializeField] AudioClip deathSound;
    [SerializeField][Range(0, 1)] float deathSoundVolume = 0.75f;
    [SerializeField] AudioClip shootSound;
    [SerializeField][Range(0, 1)] float shootSoundVolume = 0.25f;


    // Start is called before the first frame update
    void Start()
    {
        shotCounter = GetRandomShotCounter();
    }

    // Update is called once per frame
    void Update()
    {
        CountDownAndShoot();
    }

    private void CountDownAndShoot()
    {
        shotCounter -= Time.deltaTime;
        if (shotCounter <= 0)
        {
            Fire();
            shotCounter = GetRandomShotCounter();
        }
    }

    private void Fire()
    {
        GameObject laser = Instantiate(
            laserPrefab,
            transform.position,
            Quaternion.identity
        );
        laser.GetComponent<Rigidbody2D>().velocity = new Vector2(0, -laserSpeed);
        TriggerLaserSoundEffect();
    }

    private void TriggerLaserSoundEffect()
    {
        AudioSource.PlayClipAtPoint(shootSound, Camera.main.transform.position, shootSoundVolume);
    }

    private float GetRandomShotCounter()
    {
        return Random.Range(minTimeBetweenShots, maxTimeBetweenShots);
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
        TriggerExplosionEffect();
        TriggerDeathSoundEffect();
        AddPointsToScore();
        Destroy(gameObject);
    }
    private void TriggerExplosionEffect()
    {
        GameObject explosion = Instantiate(
            deathVFS,
            transform.position,
            Quaternion.identity
        );
        Destroy(explosion, durationOfExplosion);
    }

    private void TriggerDeathSoundEffect()
    {
        AudioSource.PlayClipAtPoint(deathSound, Camera.main.transform.position, deathSoundVolume);
    }

    private void AddPointsToScore()
    {
        FindObjectOfType<GameSession>().AddScore(scoreValue);
    }
}
