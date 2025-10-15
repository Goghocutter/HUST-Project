using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarFunctionTest : MonoBehaviour
{
    [SerializeField] private EnemyStats enemyStats;
    private float _maxHP => enemyStats._maxHP;
    [SerializeField] private float _delay = 1f;
    private float _currentHP => enemyStats._currentHP;
    private float previousHP;

    [SerializeField] private Image _healthBar;
    [SerializeField] private Image _damageBar;


    private bool _changingBar = false;

    void Start()
    {
        _damageBar.rectTransform.anchorMax = new Vector2(1, 1);
        previousHP = _maxHP;
    }

    void Update()
    {
        if(previousHP > _currentHP)
        {
            previousHP -= Time.deltaTime * 10f;
            DamageShowcaseVisualChange();
            DamageHealthVisualChange();
        }
        else if(previousHP > _currentHP && _currentHP != 0)
        {
            previousHP = _currentHP;
            DamageShowcaseVisualChange();
            DamageHealthVisualChange();
        }
    }

    private void DamageHealthVisualChange()
    {
        _healthBar.rectTransform.anchorMax = new Vector2(_currentHP / _maxHP, 1);
    }

    private void DamageShowcaseVisualChange()
    {
        _damageBar.rectTransform.anchorMax = new Vector2(previousHP / _maxHP, 1);
    }
}
