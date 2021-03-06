using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ptrn_ShoutLong : BossPattern
{
    public override int AnimationCode => 5;

    private const int GroggyState = 3;
    private const float AnimHoldingTime = 2.9f;

    [Header("Owner Property")]
    [SerializeField] private TheKingWonchul _Owner;
    [SerializeField] private Collider2D _HurtBox;
    [SerializeField] private HitBox _HitBox;

    [Header("Shouting Property")]
    [SerializeField] private ParticleSystem _ShoutingEffect;

    [Header("Summon Wonchul")]
    [SerializeField] private GameObject _Wonchul;
    [SerializeField] private float _Interval;
    [SerializeField] private float _SummonRangeWidth;

    private int _ShoutingCount = 0;
    public override void Action()
    {
        if (_Owner.SuperArmor <= 0f)
        {
            StartCoroutine(ActionHolding());
        }
        else
        {
            ShoutingAction();
        }
    }
    public override void Notify_HealthUpdate(float restPercent)
    {
        base.Notify_HealthUpdate(restPercent);

        // 0.7, 0.4, 0.1
        if (restPercent <= 1 - 0.3f * (_ShoutingCount + 1))
        {
            ++_ShoutingCount;
            Action();
        }
    }
    private void ShoutingAction()
    {
        _HurtBox.enabled = false;
        _Owner.PatternTimerForceStop();
        
        base.Action();
    }
    private void AE_ShoutLong_End()
    {
        StartCoroutine(PlayEffect());
        StartCoroutine(SummonWonchul());
    }
	private void AE_ShoutLong_AnimDuration()
	{
		StartCoroutine(AnimDuration());
	}
    private IEnumerator PlayEffect()
    {
        _ShoutingEffect.Play();

        float duration = _ShoutingEffect.main.duration + _ShoutingEffect.main.startLifetime.constant;

        MainCamera.Instance.CameraShake(duration, (duration - 0.3f) * 0.2f, ShakeStyle.Cliff);
		yield return null;
    }
    private IEnumerator SummonWonchul()
    {
        Vector2 position = transform.position;

        for (int i = 0; i < _ShoutingCount * 3; i++)
        {
            for (float j = 0f; j < _Interval; j += Time.deltaTime * Time.timeScale)
            {
                if (_Animator.GetInteger(_AnimatorHash) != AnimationCode)
                    yield break;

                yield return null;
            }
            Vector2 summonPoint = position + new Vector2(Random.Range(-1f, 1f) * _SummonRangeWidth, -1.15f);
            Instantiate(_Wonchul, summonPoint, Quaternion.identity);
        }
    }
    private IEnumerator AnimDuration()
    {
		_Animator.speed = 0;
		for (float i = 0f; i < AnimHoldingTime; i += Time.deltaTime * Time.timeScale)
		{
			_HitBox.ClearCollidedObjects();
			yield return null;
		}

		_Animator.speed = 1;
        _HurtBox.enabled = true;
        _Owner.PatternTimerReStart();
    }
    private IEnumerator ActionHolding()
    {
        while (_Animator.GetInteger(_AnimatorHash) != _DefaultAnimationCode)
            yield return null;
        ShoutingAction();
    }
}
