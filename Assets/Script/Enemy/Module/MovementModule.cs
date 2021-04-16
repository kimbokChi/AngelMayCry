using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementModule : MonoBehaviour
{
    private static readonly Quaternion LookAtRight = 
        Quaternion.Euler(0, 180, 0);

    public event System.Action MoveBeginAction;
    public event System.Action MoveEndAction;

    // =========== ============== =========== //
    // =========== Inspector Vlew =========== //
    public AnimatorUpdateMode TimeMode;
    public SlipingEffector Sliping;
    public Rigidbody2D Rigidbody;

    [Header("WaitTime Property")]
    [Range(0f, 2f)] public float WaitTimeMin;
    [Range(0f, 2f)] public float WaitTimeMax;

    [Header("MoveTime Property")]
    [Range(0f, 4f)] public float MoveTimeMin;
    [Range(0f, 4f)] public float MoveTimeMax;

    [Header("Movement Property")]
    public float MoveSpeed;
    public float MoveSpeedMax;

    [Space()]
    public float MoveRangeLeft;
    public float MoveRangeRight;
    // =========== Inspector Vlew =========== //
    // =========== ============== =========== //

    [HideInInspector] public Vector2 NextMoveDirection = Vector2.zero;
    [HideInInspector] public Vector2 OriginalPostion;

    private float _RangeLeftX;
    private float _RangeRightX;

    public bool RoutineEnable
    {
        get => _RoutineEnable;
        set
        {
            if (_RoutineEnable = value)
            {
                if (_MoveCycleRoutine == null)
                    StartCoroutine(_MoveCycleRoutine = MoveCycleRoutine());
            }
            else
            {
                if (_MoveRoutine != null)
                {
                    StopCoroutine(_MoveRoutine);
                    _MoveRoutine = null;
                }
                if (_MoveCycleRoutine != null)
                {
                    StopCoroutine(_MoveCycleRoutine);
                    _MoveCycleRoutine = null;
                }
            }
        }
    }
    private bool _RoutineEnable = true;

    private IEnumerator _MoveCycleRoutine = null;
    private IEnumerator _MoveRoutine = null;

    private void OnEnable()
    {
        RoutineEnable = _RoutineEnable;
        OriginalPostion = transform.position;

         _RangeLeftX = OriginalPostion.x - MoveRangeLeft;
        _RangeRightX = OriginalPostion.x + MoveRangeRight;
    }
    public void MoveStop()
    {
        _MoveRoutine = null;
    }
    private float DeltaTime()
    {
        switch (TimeMode)
        {
            case AnimatorUpdateMode.Normal:
                return Time.deltaTime;

            case AnimatorUpdateMode.AnimatePhysics:
                return Time.fixedDeltaTime;

            case AnimatorUpdateMode.UnscaledTime:
                return Time.unscaledTime;

            default: return Time.deltaTime;
        }
    }
    private IEnumerator MoveCycleRoutine()
    {
        while (RoutineEnable)
        {
            Vector2 dir;
            if (Mathf.Abs(NextMoveDirection.x) > 0) {
                dir = NextMoveDirection;
            }
            else 
                dir = Random.value < 0.5f ? Vector2.left : Vector2.right;
            transform.rotation = dir.x > 0 ? LookAtRight : Quaternion.identity;
            
            NextMoveDirection = Vector2.zero;

            float move = Random.Range(MoveTimeMin, MoveTimeMax);
            StartCoroutine(_MoveRoutine = MoveRoutine(dir, move));

            while (_MoveRoutine != null) 
                yield return null;

            Sliping.Start();
            while (Sliping.IsProceeding)
                yield return null;

            float wait = Random.Range(WaitTimeMin, WaitTimeMax);
            for (float i = 0; i < wait; i += DeltaTime())
                yield return null;
        }
    }
    private IEnumerator MoveRoutine(Vector2 direction, float moveTime)
    {
        MoveBeginAction?.Invoke();

        for (float i = 0; i < moveTime; i += DeltaTime())
        {
            Rigidbody.AddForce(direction * MoveSpeed * DeltaTime());
            Vector2 vel = Rigidbody.velocity;

            Rigidbody.velocity = 
                new Vector2(Mathf.Clamp(vel.x, -MoveSpeedMax, MoveSpeedMax), vel.y);

            if ((transform.position.x <=  _RangeLeftX && direction.x < 0) || 
                (transform.position.x >= _RangeRightX && direction.x > 0)) {

                float x = Mathf.Clamp(transform.position.x, _RangeLeftX, _RangeRightX);
                transform.position = new Vector2(x, transform.position.y);

                break;
            }
            yield return null;
        }
        MoveEndAction?.Invoke();
        _MoveRoutine = null;
    }
}