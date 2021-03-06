using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ShakeStyle
{
    WaterFall, Cliff, Rise
}
public class MainCamera : Singleton<MainCamera>
{
    [SerializeField] private Camera _Camera;
	[SerializeField] private Image _FadeMask;

    [Header("Shake Curves")]
    [SerializeField] private AnimationCurve _ShakeCurve_WaterFall;
    [SerializeField] private AnimationCurve _ShakeCurve_Cliff;
    [SerializeField] private AnimationCurve _ShakeCurve_Rise;

    private AnimationCurve _CrntShakeCurve;

    private float _RestShakeTime;
    private float _ShakeTime;
    private float _ShakeForcePerFrame;

    public void CameraShake(float time, float force, ShakeStyle style = ShakeStyle.WaterFall)
    {
        float forcePerFrame = force;
        float ratio = 1f - Mathf.Min(_RestShakeTime / _ShakeTime, 1f);

        if (_CrntShakeCurve == null)
        {
            _ShakeForcePerFrame = forcePerFrame;
            _RestShakeTime = _ShakeTime = time;
            SetShakeCurve(style);
            return;
        }
        if (forcePerFrame > _ShakeForcePerFrame * _CrntShakeCurve.Evaluate(ratio))
        {
            _ShakeForcePerFrame = forcePerFrame;
            _RestShakeTime = _ShakeTime = time;
            SetShakeCurve(style);
        }
        else
        {
            _ShakeForcePerFrame += forcePerFrame;
        }
    }
	public void Fade(Color start, Color end, float time = 1, System.Action callback = null)
	{
		StartCoroutine(FadeRoutine(start, end, time, callback));
	}
	private IEnumerator FadeRoutine(Color start, Color end, float time, System.Action callback = null)
	{
		float curTime = 0;
		while (curTime < 1)
		{
			_FadeMask.color = Color.Lerp(start, end, curTime);
			curTime += 1f / time * Time.deltaTime;
			yield return null;
		}
		callback?.Invoke();
	}
	private void Start()
	{
		Fade(Color.black, new Color(0, 0, 0, 0));
	}
    public void SetShakeCurve(ShakeStyle curveStyle)
    {
        AnimationCurve GetStyle(ShakeStyle style) => style switch
        {
            ShakeStyle.WaterFall => _ShakeCurve_WaterFall,
            ShakeStyle.Cliff => _ShakeCurve_Cliff,
            ShakeStyle.Rise => _ShakeCurve_Rise,
            _ => null
        };
        _CrntShakeCurve = GetStyle(curveStyle);
    }
    public void SetCameraScale(float scale, float time)
    {
        StartCoroutine(CameraScaleRoutine(scale, time));
    }

    private void Update()
    {
        if (_RestShakeTime > 0)
        {
            _RestShakeTime -= Time.unscaledDeltaTime;

            float ratio = 1f - _RestShakeTime / _ShakeTime;
            transform.position = Random.onUnitSphere * _ShakeForcePerFrame * _CrntShakeCurve.Evaluate(ratio);

            if (_RestShakeTime <= 0f) {
                _RestShakeTime = _ShakeForcePerFrame = _ShakeTime = 0f;

                _CrntShakeCurve = null;
            }
        }
    }

    private IEnumerator CameraScaleRoutine(float scale, float time)
    {
        float startScale = _Camera.orthographicSize;

        for (float i = 0f; i < time; i += Time.deltaTime)
        {
            _Camera.orthographicSize = Mathf.Lerp(startScale, scale, _ShakeCurve_WaterFall.Evaluate(Mathf.Min(1f, 1 - i / time)));
            yield return null;
        }
    }
}
