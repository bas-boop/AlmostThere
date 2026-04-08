using UnityEngine;

public class SeeThrough : MonoBehaviour
{
    // Focus Object and Target Material
    [Header("Focus Object & Material")]
    public Transform focus_object;
    public Material target_material;
    public float offset;
    [Header("Variables")]
    private Camera mainCamera;
    public string distance_variable = "_DistanceCutoff";
    public string focus_object_screenspace_position = "_FocusObjectPosition";
    private int distancePropertyID;
    private int screenPosPropertyID;

    // Update is called once per frame
    void Start()
    {
        mainCamera = Camera.main;
        distancePropertyID = Shader.PropertyToID(distance_variable);
        screenPosPropertyID = Shader.PropertyToID(focus_object_screenspace_position);
    }
    void Update()
    {
        if (focus_object == null || target_material == null) return;

        float distance = Vector3.Distance(mainCamera.transform.position, focus_object.position);
        Vector2 screenPos = mainCamera.WorldToViewportPoint(focus_object.position);
        target_material.SetFloat(distancePropertyID, distance + offset);
        target_material.SetVector(screenPosPropertyID, screenPos);
    }
}
