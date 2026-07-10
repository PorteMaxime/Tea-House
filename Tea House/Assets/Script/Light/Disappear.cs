using System;
using System.Globalization;
using UnityEngine;

public class Disappear : MonoBehaviour
{
    public Daylight mDaylight;
    private MeshRenderer mMeshRenderer = null;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mMeshRenderer = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if(mDaylight.mDayTimeSpent >= 7f)
        {
            float current = (mDaylight.mDayLength - mDaylight.mDayTimeSpent) / 6f;
            SetAplha(current);
        }
        else
            SetAplha(1f);
    }

    void SetAplha(float newAlpha)
    {
        Color newColor = mMeshRenderer.material.color;
        newColor.a = newAlpha;
        mMeshRenderer.material.color = newColor;
    }
}
