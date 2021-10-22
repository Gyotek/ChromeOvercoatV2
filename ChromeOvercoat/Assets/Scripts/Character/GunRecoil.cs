using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[HideMonoScript]
public class GunRecoil : MonoBehaviour
{
    [SerializeField] float verticalRecoil;
    [SerializeField] float horizontalRecoil;
    [SerializeField] [Range(0,1)] float horizontalAngle;

    [SerializeField] float RecoilMultiplier;

    Vector3 newEulerAnlges;

    public void RecoilCall()
    {
        StartCoroutine(Recoil());
    }

    IEnumerator Recoil()
    {
        if(Mathf.Abs(transform.rotation.eulerAngles.x) < 85.0f)
        {
            int randomHorizontalDir = (int)Mathf.Sign(Random.Range(-1, 1));

            newEulerAnlges = transform.rotation.eulerAngles;
            newEulerAnlges.x -= verticalRecoil * RecoilMultiplier;
            newEulerAnlges.y -= horizontalRecoil * randomHorizontalDir * horizontalAngle * RecoilMultiplier;
            transform.rotation = Quaternion.Euler(newEulerAnlges);
        }

        yield return null;
    }
}
