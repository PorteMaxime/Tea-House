using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Camera_Raycast : ICameraControllerMod
{
    #region Main
    private CameraController mMain = null;
    public Camera_Raycast(CameraController main)
    {
        mMain = main;
        OnInit();
    }
    #endregion

    public void OnInit()
    {
        
    }
    public void OnUpdate()
    {
        return;
    }
    public void OnFixedUpdate() {}

    public Vector3 GetImpact(float originDistance, GameObject xAxeSnap)
    {
        Vector3 mousePos = Mouse.current.position.ReadValue();      // Coordonnées X / Y de la souris sur l'écran
        mousePos.z = originDistance;
        Vector3 worldPos = mMain.mCamera.ScreenToWorldPoint(mousePos);

        Vector3 dir = (worldPos - mMain.transform.position).normalized;
        float dist = Vector3.Distance(mMain.transform.position, new Vector3(xAxeSnap.transform.position.x, worldPos.y, worldPos.z));
        worldPos = mMain.transform.position + dir * dist;

        return worldPos;
    }

    #region Single Raycast
    static public void Raycast(Ray ray, float distance, LayerMask[] layers, string[] tags, out GameObject hit)
    {
        int totalLayers = 0;
        if (layers != null)
        {
            foreach (LayerMask layer in layers)
                totalLayers |= layer.value;
        }

        RaycastHit[] hits = Physics.RaycastAll(ray, distance, totalLayers);

        bool hasTagFilter = tags != null && tags.Length > 0;

        foreach (RaycastHit hitInfo in hits)
        {
            GameObject go = hitInfo.collider.gameObject;
            if (hasTagFilter == false)
                {
                    hit = go;
                    return;
                }
                else
                {
                    for (int i = 0; i < tags.Length; i++)
                    {
                        if (go.CompareTag(tags[i]))
                        {
                            hit = go;
                            return;
                        }
                    }
                }
        }

        hit = null;
        return;
    }
    static public void Raycast(Ray ray, float radius, float distance, LayerMask[] layers, string[] tags, out GameObject hit)
    {
        int totalLayers = 0;

        if (layers != null)
        {
            foreach (LayerMask layer in layers)
                totalLayers |= layer.value;
        }

        RaycastHit[] hits = Physics.SphereCastAll(ray, radius, distance, totalLayers);

        bool hasTagFilter = tags != null && tags.Length > 0;

        foreach (RaycastHit hitInfo in hits)
        {
            GameObject go = hitInfo.collider.gameObject;
            if (hasTagFilter == false)
                {
                    hit = go;
                    return;
                }
                else
                {
                    for (int i = 0; i < tags.Length; i++)
                    {
                        if (go.CompareTag(tags[i]))
                        {
                            hit = go;
                            return;
                        }
                    }
                }
        }

        hit = null;
        return;
    }

    static public void Raycast(Ray ray, float distance, LayerMask layer, string tag, out GameObject hit)
    {
        Raycast(ray, distance, new LayerMask[1] { layer }, new string[1] { tag }, out hit);
        return;
    }
    static public void Raycast(Ray ray, float distance, LayerMask layer, string[] tags, out GameObject hit)
    {
        Raycast(ray, distance, new LayerMask[1] { layer }, tags, out hit);
        return;
    }
    static public void Raycast(Ray ray, float distance, LayerMask[] layers, string tag, out GameObject hit)
    {
        Raycast(ray, distance, layers, new string[1] { tag }, out hit);
        return;
    }
    static public void Raycast(Ray ray, float radius, float distance, LayerMask layer, string tag, out GameObject hit)
    {
        Raycast(ray, radius, distance, new LayerMask[1] { layer }, new string[1] { tag }, out hit);
        return;
    }
    static public void Raycast(Ray ray, float radius, float distance, LayerMask layer, string[] tags, out GameObject hit)
    {
        Raycast(ray, radius, distance, new LayerMask[1] { layer }, tags, out hit);
        return;
    }
    static public void Raycast(Ray ray, float radius, float distance, LayerMask[] layers, string tag, out GameObject hit)
    {
        Raycast(ray, radius, distance, layers, new string[1] { tag }, out hit);
        return;
    }
    #endregion

    #region Mouse Single Raycast
    public void MouseRaycast(float distance, LayerMask[] layers, string[] tags, out GameObject hit)
    {
        Camera cam = Camera.main;
        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        Raycast(ray, distance, layers, tags, out hit);
        return;
    }
    public void MouseRaycast(float radius, float distance, LayerMask[] layers, string[] tags, out GameObject hit)
    {
        Camera cam = Camera.main;
        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        Raycast(ray, radius, distance, layers, tags, out hit);
        return;
    }

    public void MouseRaycast(float distance, LayerMask layer, string tag, out GameObject hit)
    {
        MouseRaycast(distance, new LayerMask[1] { layer }, new string[1] { tag }, out hit);
        return;
    }
    public void MouseRaycast(float distance, LayerMask layer, string[] tags, out GameObject hit)
    {
        MouseRaycast(distance, new LayerMask[1] { layer }, tags, out hit);
        return;
    }
    public void MouseRaycast(float distance, LayerMask[] layers, string tag, out GameObject hit)
    {
        MouseRaycast(distance, layers, new string[1] { tag }, out hit);
        return;
    }
    public void MouseRaycast(float radius, float distance, LayerMask layer, string tag, out GameObject hit)
    {
        MouseRaycast(radius, distance, new LayerMask[1] { layer }, new string[1] { tag }, out hit);
        return;
    }
    public void MouseRaycast(float radius, float distance, LayerMask layer, string[] tags, out GameObject hit)
    {
        MouseRaycast(radius, distance, new LayerMask[1] { layer }, tags, out hit);
        return;
    }
    public void MouseRaycast(float radius, float distance, LayerMask[] layers, string tag, out GameObject hit)
    {
        MouseRaycast(radius, distance, layers, new string[1] { tag }, out hit);
        return;
    }
    #endregion

    #region Multi Raycast
    static public void Raycast(Ray ray, float distance, LayerMask[] layers, string[] tags, out List<GameObject> hits)
    {
        hits = new List<GameObject>();
        int totalLayers = 0;

        if (layers != null)
        {
            foreach (LayerMask layer in layers)
                totalLayers |= layer.value;
        }

        bool hasTagFilter = tags != null && tags.Length > 0;

        RaycastHit[] hitsInfo = Physics.RaycastAll(ray, distance, totalLayers);

        foreach (RaycastHit hitInfo in hitsInfo)
        {
            GameObject go = hitInfo.collider.gameObject;
            if (hasTagFilter == false)
                    hits.Add(go);
            else
                {
                    for (int i = 0; i < tags.Length; i++)
                    {
                        if (go.CompareTag(tags[i]))
                        {
                            hits.Add(go);
                            break;
                        }
                    }
                }
        }

        return;
    }
    static public void Raycast(Ray ray, float radius, float distance, LayerMask[] layers, string[] tags, out List<GameObject> hits)
    {
        hits = new List<GameObject>();
        int totalLayers = 0;

        if (layers != null)
        {
            foreach (LayerMask layer in layers)
                totalLayers |= layer.value;
        }

        bool hasTagFilter = tags != null && tags.Length > 0;
        
        RaycastHit[] hitsInfo = Physics.SphereCastAll(ray, radius, distance, totalLayers);

        foreach (RaycastHit hitInfo in hitsInfo)
        {
            GameObject go = hitInfo.collider.gameObject;
            if (hasTagFilter == false)
                    hits.Add(go);
            else
                {
                    for (int i = 0; i < tags.Length; i++)
                    {
                        if (go.CompareTag(tags[i]))
                        {
                            hits.Add(go);
                            break;
                        }
                    }
                }
        }

        return;
    }

    static public void Raycast(Ray ray, float distance, LayerMask layer, string tag, out List<GameObject> hits)
    {
        Raycast(ray, distance, new LayerMask[1] { layer }, new string[1] { tag }, out hits);
        return;
    }
    static public void Raycast(Ray ray, float distance, LayerMask layer, string[] tags, out List<GameObject> hits)
    {
        Raycast(ray, distance, new LayerMask[1] { layer }, tags, out hits);
        return;
    }
    static public void Raycast(Ray ray, float distance, LayerMask[] layers, string tag, out List<GameObject> hits)
    {
        Raycast(ray, distance, layers, new string[1] { tag }, out hits);
        return;
    }
    static public void Raycast(Ray ray, float radius, float distance, LayerMask layer, string tag, out List<GameObject> hits)
    {
        Raycast(ray, radius, distance, new LayerMask[1] { layer }, new string[1] { tag }, out hits);
        return;
    }
    static public void Raycast(Ray ray, float radius, float distance, LayerMask layer, string[] tags, out List<GameObject> hits)
    {
        Raycast(ray, radius, distance, new LayerMask[1] { layer }, tags, out hits);
        return;
    }
    static public void Raycast(Ray ray, float radius, float distance, LayerMask[] layers, string tag, out List<GameObject> hits)
    {
        Raycast(ray, radius, distance, layers, new string[1] { tag }, out hits);
        return;
    }
    #endregion

    #region Mouse Mutli Raycast
    public void MouseRaycast(float distance, LayerMask[] layers, string[] tags, out List<GameObject> hits)
    {
        Ray ray = mMain.GetComponent<Camera>().ScreenPointToRay(Mouse.current.position.ReadValue());
        Raycast(ray, distance, layers, tags, out hits);
        return;
    }
    public void MouseRaycast(float radius, float distance, LayerMask[] layers, string[] tags, out List<GameObject> hits)
    {
        Ray ray = mMain.GetComponent<Camera>().ScreenPointToRay(Mouse.current.position.ReadValue());
        Raycast(ray, radius, distance, layers, tags, out hits);
        return;
    }

    public void MouseRaycast(float distance, LayerMask layer, string tag, out List<GameObject> hits)
    {
        MouseRaycast(distance, new LayerMask[1] { layer }, new string[1] { tag }, out hits);
        return;
    }
    public void MouseRaycast(float distance, LayerMask layer, string[] tags, out List<GameObject> hits)
    {
        MouseRaycast(distance, new LayerMask[1] { layer }, tags, out hits);
        return;
    }
    public void MouseRaycast(float distance, LayerMask[] layers, string tag, out List<GameObject> hits)
    {
        MouseRaycast(distance, layers, new string[1] { tag }, out hits);
        return;
    }
    public void MouseRaycast(float radius, float distance, LayerMask layer, string tag, out List<GameObject> hits)
    {
        MouseRaycast(radius, distance, new LayerMask[1] { layer }, new string[1] { tag }, out hits);
        return;
    }
    public void MouseRaycast(float radius, float distance, LayerMask layer, string[] tags, out List<GameObject> hits)
    {
        MouseRaycast(radius, distance, new LayerMask[1] { layer }, tags, out hits);
        return;
    }
    public void MouseRaycast(float radius, float distance, LayerMask[] layers, string tag, out List<GameObject> hits)
    {
        MouseRaycast(radius, distance, layers, new string[1] { tag }, out hits);
        return;
    }
    #endregion
}