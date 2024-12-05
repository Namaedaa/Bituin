using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisionCone : MonoBehaviour
{
    public float visionRadius = 5f;
    public float visionAngle = 45f;
    public int segments = 30;
    public LayerMask visionLayer;

    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;

    [SerializeField]
    private Material materialInstance;
    [SerializeField] private Color visionConeAlertColor, visionConeNormalColor;
    public Vector3 lastSeenPlayer;

    private void Start()
    {
        gameObject.GetComponent<Renderer>().material = materialInstance;
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public Transform EnemyVisionCheck()
    {
        Transform target = null;
        int vertexCount = segments + 2;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[segments * 3];

        vertices[0] = Vector3.zero;

        float angleIncrement = visionAngle / segments;

        for (int i = 0; i <= segments; i++)
        {
            float angle = -visionAngle * 0.5f + i * angleIncrement;
            Vector3 direction = Quaternion.Euler(0, 0, angle) * transform.up;

            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, visionRadius, visionLayer);

            if (hit.collider != null)
            {
                vertices[i + 1] = transform.InverseTransformPoint(hit.point);


                if (hit.collider.gameObject.tag == "Amelia" || hit.collider.gameObject.tag == "Robot")
                {
                    /* Vector2 player_direction = (hit.transform.position - transform.position).normalized;*/
                    materialInstance.SetColor("VisionColor", visionConeAlertColor);
                     RaycastHit2D check_obstacles_to_player = Physics2D.Raycast(transform.position, direction, visionRadius);

                    if (check_obstacles_to_player.collider.gameObject.tag != "Obstacles")
                        target = hit.transform;
                }          
            }
            else
            {
                materialInstance.SetColor("VisionColor", visionConeNormalColor);
                vertices[i + 1] = transform.InverseTransformPoint(transform.position + direction * visionRadius);
            }

            if (i > 0)
            {
                int triIndex = (i - 1) * 3;
                triangles[triIndex] = 0;
                triangles[triIndex + 1] = i;
                triangles[triIndex + 2] = i + 1;
            }
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;

        meshFilter.mesh = mesh;
        return target;
    }
}



