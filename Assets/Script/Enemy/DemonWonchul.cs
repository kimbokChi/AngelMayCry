using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonWonchul : MonoBehaviour
{
    private const int Idle = 0;
    private const int Move = 1;

    [SerializeField] private MovementModule _MovementModule;
    [SerializeField] private Animator _Animator;

    [SerializeField] private SecondaryCollider _DetectionRange;

    private bool _IsAlreadyInit = false;
    private int _AnimHash;

    private void OnEnable()
    {
        if (!_IsAlreadyInit)
        {
            _AnimHash = _Animator.GetParameter(0).nameHash;

            _MovementModule.MoveBeginAction += () =>
            {
                _Animator.SetInteger(_AnimHash, Move);
            };
            _MovementModule.MoveEndAction += () =>
            {
                _Animator.SetInteger(_AnimHash, Idle);
            };
            _MovementModule.TrackingEndAction += () =>
            {
                _Animator.SetInteger(_AnimHash, Idle);
            };
            _DetectionRange.OnTriggerAction = (other, enter) =>
            {
                if (other.CompareTag("Player"))
                {
                    if (enter)
                    {
                        _MovementModule.TrackingComplete = AttackRoutine();
                        _MovementModule.TrackingStart(other.transform);
                    }
                    else
                    {
                        _MovementModule.TrackingStop();
                    }
                }
            };
            _IsAlreadyInit = true;

            _MovementModule.Operation();
        }
    }

    private IEnumerator AttackRoutine()
    {
        _Animator.SetInteger(_AnimHash, 2);
        yield return new WaitForSeconds(0.9f);

        _Animator.SetInteger(_AnimHash, 0);
        yield return new WaitForSeconds(0.4f);
    }
}
