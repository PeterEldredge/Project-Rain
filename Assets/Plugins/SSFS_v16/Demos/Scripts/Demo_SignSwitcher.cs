/*
This script switches a renderer's material.

Email :: thomas.ir.rasor@gmail.com
*/
using UnityEngine;
using System.Collections;

public class Demo_SignSwitcher : MonoBehaviour
{
    public Material[] materials;
    public bool autoswitch = false;
    public float delay = 6f;
    public float transitionTime = 1f;
    public KeyCode nextKey = KeyCode.RightArrow;
    public KeyCode prevKey = KeyCode.LeftArrow;

    bool isswitching = false;
    int currentId = 0;
    float cphase = 0f;
    Renderer r;
    float t = 0f;

    void Start()
    {
        r = GetComponent<Renderer>();
        t = delay;
        StartCoroutine( GoToMaterial() );
    }

    public IEnumerator GoToMaterial()
    {
        if ( materials.Length > 0 )
        {
            Material nextMat = materials[ currentId ];

            isswitching = true;
            while ( cphase > 0f )
            {
                cphase -= Time.deltaTime / transitionTime;
                r.sharedMaterial.SetFloat( "_Phase" , cphase );
                yield return new WaitForEndOfFrame();
            }

            r.sharedMaterial = nextMat;

            while ( cphase < 1f )
            {
                cphase += Time.deltaTime / transitionTime;
                r.sharedMaterial.SetFloat( "_Phase" , cphase );
                yield return new WaitForEndOfFrame();
            }
            isswitching = false;
        }
    }

    void ShiftMaterial( int offset )
    {
        currentId = ( int )Mathf.Repeat( currentId + offset , materials.Length );
        StartCoroutine( GoToMaterial() );
    }

    void Update()
    {
        if ( isswitching || r == null )
            return;

        if ( autoswitch )
        {
            if ( t > 0f )
            {
                t -= Time.deltaTime;
            }
            else
            {
                t = delay;
                ShiftMaterial( 1 );
            }
        }
        else
        {
            if ( Input.GetKeyDown( nextKey ) )
                ShiftMaterial( 1 );

            if ( Input.GetKeyDown( prevKey ) )
                ShiftMaterial( -1 );
        }
    }
}
