using UnityEngine;

public class SeeThrough : MonoBehaviour
{
    // Focus Object and Target Material
    [Header("Focus Object & Material")]
    public Transform focus_object;
    public Material target_material;
    public float offset;
    [Header("Variables")]
    public string distance_variable;
    public string focus_object_screenspace_position;

    // Update is called once per frame
    void Update()
    {
        if (focus_object == null || target_material == null) return;

        float distance = Vector3.Distance(Camera.main.transform.position, focus_object.position);
        Vector2 screenPos = Camera.main.WorldToViewportPoint(focus_object.position);
        target_material.SetFloat(distance_variable, distance + offset);
        target_material.SetVector(focus_object_screenspace_position, screenPos);
        print(screenPos);
    }
}
