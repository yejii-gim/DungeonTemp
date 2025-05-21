using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamagable
{
    void TakePhysicalDamage(int damage);
}
public class PlayerCondition : MonoBehaviour,IDamagable
{
    public UICondition uiCondition;

    Condition health { get { return uiCondition.health; } }
    Condition hunger { get { return uiCondition.hunger; } }
    Condition stamina { get { return uiCondition.stamina; } }

    public float noHungerHealthDecay;
    public event Action onTakeDamage;

    // 公利 包访
    private bool isInvincible = false;
    private float invincibleTime = 0f;
    public Color invincibleColor = new Color(1f, 1f, 1f, 0.5f); // 馆捧疙
    private Color originColor;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
            originColor = spriteRenderer.color;
    }
    private void Update()
    {
        hunger.Substact(hunger.passiveValue * Time.deltaTime);
        stamina.Add(stamina.passiveValue * Time.deltaTime);

        if(hunger.curValue == 0f)
        {
            health.Substact(noHungerHealthDecay * Time.deltaTime);
        }

        if(health.curValue == 0f)
        {
            Die();
        }

        // 公利 包访
        if (isInvincible)
        {
            invincibleTime -= Time.deltaTime;

            if (spriteRenderer != null)
                spriteRenderer.color = invincibleColor;

            if (invincibleTime <= 0f)
            {
                isInvincible = false;
                if (spriteRenderer != null)
                    spriteRenderer.color = originColor;
            }
        }
    }
    public void Die()
    {
        Debug.Log("Die");
    }

    public void TakePhysicalDamage(int damage)
    {
        // 公利 惑怕老矫 公矫
        if (isInvincible)
        {
            return;
        }
        health.Substact(damage);
        onTakeDamage?.Invoke();
    }

    public void Heal(float count)
    {
        health.Add(count);
    }

    public void Eat(float count)
    {
        hunger.Add(count);
    }

    public bool UseStamina(float amount)
    {
        if(stamina.curValue - amount < 0f)
        {
            return false;
        }
        stamina.Substact(amount);
        return true;
    }

    public void Invincibility(float count)
    {
        UseStamina(count);
    }

    public void Dash(float count)
    {
        UseStamina(count);
    }

    public void DoubleJump(float count)
    {
        UseStamina(count);
    }
}
