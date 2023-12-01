using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerShip : Unit
{
    [SerializeField] Transform bulletSpawnTransform;
    [SerializeField] ParticleSystem rocketParticle;
    UnitDataPlayer playerData;

    InputHandler inputHandler;
    
    int fadeBlinkTimes = 8;

    float fadeBlinkTimer = 0.0f;
    float fadeBlinkMaxTimer = 0.0f;
    float alphaColorValue = 1.0f;

    float invincibilityTimer = 0.0f;
    float fireRateCooldownTimer = 0.0f;


    protected override void Start()
    {
        playerData = UnitDataPlayer;

        if (playerData == null)
        {
            Debug.LogError("Couldn't read player data");
            ReturnObject();
            return;
        }

        base.Start();
        inputHandler.ShootingActionRef.action.performed += Shoot;
    }

    protected override void Update()
    {
        base.Update();

        if (fireRateCooldownTimer > 0.0f) { fireRateCooldownTimer -= Time.deltaTime; }
        UpdateInvincibilityTimer();
        UpdateMovement();
    }

    public void AttachInputHandler(InputHandler inputHandler) => this.inputHandler = inputHandler;

    private void UpdateMovement()
    {
        if (inputHandler != null)
        {
            float forward = 0.0f;
            float rotationX = 0.0f;

            if (inputHandler.IsAccelerating) 
            {
                var particle = rocketParticle.emission;
                particle.rateOverTime = 40.0f;
                forward = 1.0f; 
            }
            else
            {
                var particle = rocketParticle.emission;
                particle.rateOverTime = 0.0f;
            }

            if (inputHandler.IsRotatingLeft) { rotationX += -1.0f; }
            if (inputHandler.IsRotatingRight) { rotationX += 1.0f; }

            rigidBody.AddForce(transform.up * forward * speed);

            if (rotationX != 0.0f)
            {
                float targetRotation = rigidBody.rotation - rotationX * playerData.rotationSpeed * Time.deltaTime;
                rigidBody.MoveRotation(targetRotation);
            }

            if (rigidBody.velocity.magnitude > speed)
            {
                rigidBody.velocity = rigidBody.velocity.normalized * speed;
            }
        }
    }

    private void Shoot(UnityEngine.InputSystem.InputAction.CallbackContext callback)
    {
        if (fireRateCooldownTimer > 0) { return; }

        GameObject bulletObj = ObjectPoolManager.Instance.GetPooledObject(EObjectPooling.Bullet);
        bulletObj.transform.position = bulletSpawnTransform.position;
        Bullet bulletComp = bulletObj.GetComponent<Bullet>();
        bulletComp.SetBulletVariables(transform.up, LayerMask.NameToLayer(StaticDefines.LAYER_ENEMY), LayerMask.NameToLayer(StaticDefines.LAYER_PLAYER), playerData.shootingSpeed);
        fireRateCooldownTimer = playerData.maxFireRateCooldown;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (invincibilityTimer > 0.0f) { return; }
        if (collision.CompareTag(StaticDefines.TAG_BULLET) || collision.CompareTag(StaticDefines.TAG_ENEMY))
        {
            Hit();
            DestroyShip();
        }
    }

    private void DestroyShip()
    {
        AddScore();
        ManagerSystem.Instance.GameManager.AddLife(-1);

        if (ManagerSystem.Instance.GameManager.GetLives() > 0)
        {
            alphaColorValue = 0.0f;
            fadeBlinkMaxTimer = playerData.maxInvincibilityTimer / fadeBlinkTimes;
            invincibilityTimer = playerData.maxInvincibilityTimer;
            rigidBody.velocity = Vector3.zero;
            ManagerSystem.Instance.GameManager.SetObjectPositionOnAvailableSpot(boxCollider2d);
        }
    }

    private void UpdateInvincibilityTimer()
    {
        if (invincibilityTimer > 0.0f)
        {
            fadeBlinkTimer += Time.deltaTime;
            invincibilityTimer -= Time.deltaTime;
            LerpFadeColor(fadeBlinkTimer);

            if (invincibilityTimer < 0.0f)
            {
                fadeBlinkTimer = 0.0f;
                alphaColorValue = 1.0f;
                LerpFadeColor(fadeBlinkMaxTimer);
            }
        }
    }
    
    private void LerpFadeColor(float timer)
    {
        Color currentColor = spriteRenderer.color;
        currentColor.a = Mathf.Lerp(currentColor.a, alphaColorValue, timer / fadeBlinkMaxTimer);
        spriteRenderer.color = currentColor;

        if (timer >= fadeBlinkMaxTimer)
        {
            alphaColorValue = alphaColorValue == 1.0f ? 0.0f : 1.0f;
            fadeBlinkTimer = 0.0f;
        }
    }

}