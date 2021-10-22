using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(BoxCollider))]
public class DeathCube : MonoBehaviour
{
    MeshRenderer meshR;
    BoxCollider boxC;

    [SerializeField] float yScaleMax = 2;
    [SerializeField] float scaleSpeed = 0.02f;
    private float currentYScale;
    Vector3 currentScale;

    bool activated;

    // Start is called before the first frame update
    void Start()
    {
        meshR = GetComponent<MeshRenderer>();
        boxC = GetComponent<BoxCollider>();
        meshR.enabled = false;
        boxC.enabled = false;
    }

    // Update is called once per frame
    public void ActivateCall(bool active)
    {
        StartCoroutine(Activate(active));
    }

    IEnumerator Activate(bool Activating)
    {
        Debug.Log(Activating);

        if (!Activating)
        {
            meshR.enabled = true;
            boxC.enabled = true;

            currentScale = transform.localScale;
            currentYScale = currentScale.y;

            currentScale.y = 0.001f;
            transform.localScale = currentScale;

            while (currentYScale < yScaleMax)
            {
                currentYScale += scaleSpeed * Time.deltaTime;
                currentScale.y = currentYScale;
                transform.localScale = currentScale;

                yield return null;
            }

            activated = true;
        }
        else
        {
            currentScale = transform.localScale;
            currentYScale = currentScale.y;

            while (currentYScale > 0)
            {
                currentYScale -= scaleSpeed * Time.deltaTime;
                currentScale.y = currentYScale;
                transform.localScale = currentScale;

                yield return null;
            }

            meshR.enabled = false;
            boxC.enabled = false;
            activated = false;
        }
        yield return null;
    }
}
