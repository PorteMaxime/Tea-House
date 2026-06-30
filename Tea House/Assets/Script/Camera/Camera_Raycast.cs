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

        Camera_Raycast.Raycast(
            new Ray(mMain.transform.position, dir),
            dist, 
            (LayerMask[])null, 
            (string[])null, 
            out List<RaycastHit> obstacles);

        if(obstacles != null && obstacles.Count > 0 && obstacles[0].collider.gameObject.tag != "Player" && obstacles[0].collider.gameObject != xAxeSnap) 
        {
            float distance = Vector3.Distance(obstacles[0].point, mMain.transform.position);
            worldPos = mMain.transform.position + dir * distance;

            Vector3 normal = obstacles[0].normal;
            Vector3 size = xAxeSnap.GetComponent<Collider>().bounds.extents;
            worldPos += new Vector3(normal.x * size.x, normal.y * size.y, normal.z * size.z);
        }

        return worldPos;
    }

    #region Single Raycast
    static public void Raycast(Ray ray, float distance, LayerMask[] layers, string[] tags, out RaycastHit hit)
    {
        int totalLayers = 0;
        if (layers != null)
        {
            foreach (LayerMask layer in layers)
                totalLayers |= layer.value;
        }
        else
            totalLayers = ~0;

        Debug.DrawRay(ray.origin, ray.direction * distance, Color.red, 2f);

        RaycastHit[] hits = Physics.RaycastAll(ray, distance, totalLayers);

        bool hasTagFilter = tags != null && tags.Length > 0;

        foreach (RaycastHit hitInfo in hits)
        {
            GameObject go = hitInfo.collider.gameObject;
            if (hasTagFilter == false)
                {
                    hit = hitInfo;
                    return;
                }
                else
                {
                    for (int i = 0; i < tags.Length; i++)
                    {
                        if (go.CompareTag(tags[i]))
                        {
                            hit = hitInfo;
                            return;
                        }
                    }
                }
        }

        hit = default;
        return;
    }
    static public void Raycast(Ray ray, float radius, float distance, LayerMask[] layers, string[] tags, out RaycastHit hit)
    {
        int totalLayers = 0;

        if (layers != null)
        {
            foreach (LayerMask layer in layers)
                totalLayers |= layer.value;
        }
        else
            totalLayers = ~0;

        RaycastHit[] hits = Physics.SphereCastAll(ray, radius, distance, totalLayers);

        Debug.DrawRay(ray.origin, ray.direction * distance, Color.pink, 2f);

        bool hasTagFilter = tags != null && tags.Length > 0;

        foreach (RaycastHit hitInfo in hits)
        {
            GameObject go = hitInfo.collider.gameObject;
            if (hasTagFilter == false)
                {
                    hit = hitInfo;
                    return;
                }
                else
                {
                    for (int i = 0; i < tags.Length; i++)
                    {
                        if (go.CompareTag(tags[i]))
                        {
                            hit = hitInfo;
                            return;
                        }
                    }
                }
        }

        hit = default;
        return;
    }

    static public void Raycast(Ray ray, float distance, LayerMask layer, string tag, out RaycastHit hit)
    {
        Raycast(ray, distance, new LayerMask[1] { layer }, new string[1] { tag }, out hit);
        return;
    }
    static public void Raycast(Ray ray, float distance, LayerMask layer, string[] tags, out RaycastHit hit)
    {
        Raycast(ray, distance, new LayerMask[1] { layer }, tags, out hit);
        return;
    }
    static public void Raycast(Ray ray, float distance, LayerMask[] layers, string tag, out RaycastHit hit)
    {
        Raycast(ray, distance, layers, new string[1] { tag }, out hit);
        return;
    }
    static public void Raycast(Ray ray, float radius, float distance, LayerMask layer, string tag, out RaycastHit hit)
    {
        Raycast(ray, radius, distance, new LayerMask[1] { layer }, new string[1] { tag }, out hit);
        return;
    }
    static public void Raycast(Ray ray, float radius, float distance, LayerMask layer, string[] tags, out RaycastHit hit)
    {
        Raycast(ray, radius, distance, new LayerMask[1] { layer }, tags, out hit);
        return;
    }
    static public void Raycast(Ray ray, float radius, float distance, LayerMask[] layers, string tag, out RaycastHit hit)
    {
        Raycast(ray, radius, distance, layers, new string[1] { tag }, out hit);
        return;
    }
    #endregion

    #region Mouse Single Raycast
    public void MouseRaycast(float distance, LayerMask[] layers, string[] tags, out RaycastHit hit)
    {
        Camera cam = Camera.main;
        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        Raycast(ray, distance, layers, tags, out hit);
        return;
    }
    public void MouseRaycast(float radius, float distance, LayerMask[] layers, string[] tags, out RaycastHit hit)
    {
        Camera cam = Camera.main;
        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        Raycast(ray, radius, distance, layers, tags, out hit);
        return;
    }

    public void MouseRaycast(float distance, LayerMask layer, string tag, out RaycastHit hit)
    {
        MouseRaycast(distance, new LayerMask[1] { layer }, new string[1] { tag }, out hit);
        return;
    }
    public void MouseRaycast(float distance, LayerMask layer, string[] tags, out RaycastHit hit)
    {
        MouseRaycast(distance, new LayerMask[1] { layer }, tags, out hit);
        return;
    }
    public void MouseRaycast(float distance, LayerMask[] layers, string tag, out RaycastHit hit)
    {
        MouseRaycast(distance, layers, new string[1] { tag }, out hit);
        return;
    }
    public void MouseRaycast(float radius, float distance, LayerMask layer, string tag, out RaycastHit hit)
    {
        MouseRaycast(radius, distance, new LayerMask[1] { layer }, new string[1] { tag }, out hit);
        return;
    }
    public void MouseRaycast(float radius, float distance, LayerMask layer, string[] tags, out RaycastHit hit)
    {
        MouseRaycast(radius, distance, new LayerMask[1] { layer }, tags, out hit);
        return;
    }
    public void MouseRaycast(float radius, float distance, LayerMask[] layers, string tag, out RaycastHit hit)
    {
        MouseRaycast(radius, distance, layers, new string[1] { tag }, out hit);
        return;
    }
    #endregion

    #region Multi Raycast
    static public void Raycast(Ray ray, float distance, LayerMask[] layers, string[] tags, out List<RaycastHit> hits)
    {
        hits = new List<RaycastHit>();
        int totalLayers = 0;

        if (layers != null)
        {
            foreach (LayerMask layer in layers)
                totalLayers |= layer.value;
        }
        else
            totalLayers = ~0;

        bool hasTagFilter = tags != null && tags.Length > 0;

        Debug.DrawRay(ray.origin, ray.direction * distance, Color.green, 2f);
        
        RaycastHit[] hitsInfo = Physics.RaycastAll(ray, distance, totalLayers);

        foreach (RaycastHit hitInfo in hitsInfo)
        {
            GameObject go = hitInfo.collider.gameObject;
            if (hasTagFilter == false)
                    hits.Add(hitInfo);
            else
                {
                    for (int i = 0; i < tags.Length; i++)
                    {
                        if (go.CompareTag(tags[i]))
                        {
                            hits.Add(hitInfo);
                            break;
                        }
                    }
                }
        }

        return;
    }
    static public void Raycast(Ray ray, float radius, float distance, LayerMask[] layers, string[] tags, out List<RaycastHit> hits)
    {
        hits = new List<RaycastHit>();
        int totalLayers = 0;

        if (layers != null)
        {
            foreach (LayerMask layer in layers)
                totalLayers |= layer.value;
        }
        else
            totalLayers = ~0;

        bool hasTagFilter = tags != null && tags.Length > 0;

        Debug.DrawRay(ray.origin, ray.direction * distance, Color.blue, 2f);
        
        RaycastHit[] hitsInfo = Physics.SphereCastAll(ray, radius, distance, totalLayers);

        foreach (RaycastHit hitInfo in hitsInfo)
        {
            GameObject go = hitInfo.collider.gameObject;
            if (hasTagFilter == false)
                    hits.Add(hitInfo);
            else
                {
                    for (int i = 0; i < tags.Length; i++)
                    {
                        if (go.CompareTag(tags[i]))
                        {
                            hits.Add(hitInfo);
                            break;
                        }
                    }
                }
        }

        return;
    }

    static public void Raycast(Ray ray, float distance, LayerMask layer, string tag, out List<RaycastHit> hits)
    {
        Raycast(ray, distance, new LayerMask[1] { layer }, new string[1] { tag }, out hits);
        return;
    }
    static public void Raycast(Ray ray, float distance, LayerMask layer, string[] tags, out List<RaycastHit> hits)
    {
        Raycast(ray, distance, new LayerMask[1] { layer }, tags, out hits);
        return;
    }
    static public void Raycast(Ray ray, float distance, LayerMask[] layers, string tag, out List<RaycastHit> hits)
    {
        Raycast(ray, distance, layers, new string[1] { tag }, out hits);
        return;
    }
    static public void Raycast(Ray ray, float radius, float distance, LayerMask layer, string tag, out List<RaycastHit> hits)
    {
        Raycast(ray, radius, distance, new LayerMask[1] { layer }, new string[1] { tag }, out hits);
        return;
    }
    static public void Raycast(Ray ray, float radius, float distance, LayerMask layer, string[] tags, out List<RaycastHit> hits)
    {
        Raycast(ray, radius, distance, new LayerMask[1] { layer }, tags, out hits);
        return;
    }
    static public void Raycast(Ray ray, float radius, float distance, LayerMask[] layers, string tag, out List<RaycastHit> hits)
    {
        Raycast(ray, radius, distance, layers, new string[1] { tag }, out hits);
        return;
    }
    #endregion

    #region Mouse Mutli Raycast
    public void MouseRaycast(float distance, LayerMask[] layers, string[] tags, out List<RaycastHit> hits)
    {
        Ray ray = mMain.GetComponent<Camera>().ScreenPointToRay(Mouse.current.position.ReadValue());
        Raycast(ray, distance, layers, tags, out hits);
        return;
    }
    public void MouseRaycast(float radius, float distance, LayerMask[] layers, string[] tags, out List<RaycastHit> hits)
    {
        Ray ray = mMain.GetComponent<Camera>().ScreenPointToRay(Mouse.current.position.ReadValue());
        Raycast(ray, radius, distance, layers, tags, out hits);
        return;
    }

    public void MouseRaycast(float distance, LayerMask layer, string tag, out List<RaycastHit> hits)
    {
        MouseRaycast(distance, new LayerMask[1] { layer }, new string[1] { tag }, out hits);
        return;
    }
    public void MouseRaycast(float distance, LayerMask layer, string[] tags, out List<RaycastHit> hits)
    {
        MouseRaycast(distance, new LayerMask[1] { layer }, tags, out hits);
        return;
    }
    public void MouseRaycast(float distance, LayerMask[] layers, string tag, out List<RaycastHit> hits)
    {
        MouseRaycast(distance, layers, new string[1] { tag }, out hits);
        return;
    }
    public void MouseRaycast(float radius, float distance, LayerMask layer, string tag, out List<RaycastHit> hits)
    {
        MouseRaycast(radius, distance, new LayerMask[1] { layer }, new string[1] { tag }, out hits);
        return;
    }
    public void MouseRaycast(float radius, float distance, LayerMask layer, string[] tags, out List<RaycastHit> hits)
    {
        MouseRaycast(radius, distance, new LayerMask[1] { layer }, tags, out hits);
        return;
    }
    public void MouseRaycast(float radius, float distance, LayerMask[] layers, string tag, out List<RaycastHit> hits)
    {
        MouseRaycast(radius, distance, layers, new string[1] { tag }, out hits);
        return;
    }
    #endregion
}