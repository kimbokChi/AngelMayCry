using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheKingWonchul : MonoBehaviour
{
    private const int Idle = 0;

    [SerializeField] private Animator _Animator;
    private int _AnimControlKey;

    [SerializeField] private float _PatternWait;

    [Header("BossPattern_Special")]
    [SerializeField] private BossPattern _Appears;
    [SerializeField] private BossPattern _Groggy;
    [SerializeField] private BossPattern _ShoutingLong;
    [SerializeField] private BossPattern _Move;

    [Header("BossPattern_Normal")]
    [SerializeField] private BossPattern[] _Patterns;

    private void Awake()
    {
        _AnimControlKey = _Animator.GetParameter(0).nameHash;

        // Special Pattern Init
        {
            _Appears.Init();
            _Groggy.Init();
            _ShoutingLong.Init();
            _Move.Init();
        }
        for (int i = 0; i < _Patterns.Length; i++) 
        {
            _Patterns[i].Init();
        }
        _Appears.Action();
    }
    public void Awaken()
    {
        _ShoutingLong.Action();
        StartCoroutine(PatternTimer());
    }
    private IEnumerator PatternTimer()
    {
        while (_Animator.GetInteger(_AnimControlKey) != Idle)
            yield return null;

        while (gameObject.activeSelf)
        {
            for (float i = 0f; i < _PatternWait; i += Time.deltaTime * Time.timeScale)
                yield return null;

            _Move.Action();
            // _Patterns[Random.Range(0, _Patterns.Length)].Action();

            while (_Animator.GetInteger(_AnimControlKey) != Idle)
                yield return null;
        }
    }
    private void AE_SetIdleState()
    {
        _Animator.SetInteger(_AnimControlKey, Idle);
    }
}
