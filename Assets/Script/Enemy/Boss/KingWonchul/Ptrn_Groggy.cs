using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ptrn_Groggy : BossPattern
{
    public override int AnimationCode => 3;
	private readonly int GroggyEnd = 20;

    [Header("Groggy Property")]
    public float GroggyTime;

    private void AE_Groggy_HoldBegin()
    {
        MainCamera.Instance.CameraShake(1f, 0.35f);
        StartCoroutine(GroggyHolding());
    }
    private IEnumerator GroggyHolding()
    {
        for (float i = 0f; i < GroggyTime; i += Time.deltaTime * Time.timeScale) { 
            yield return null;
        }
        _Animator.SetInteger(_AnimatorHash, GroggyEnd);
    }
}
