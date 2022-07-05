using UnityEngine;
using UnityEngine.U2D;

public class SpriteShapeWater : MonoBehaviour
{
    [SerializeField] private LineRenderer surface;
    [SerializeField] private SpriteShapeController body;
    [SerializeField] private float surfaceOffset = 0.025f;
    [SerializeField] private BoxCollider2D waterArea;
    [SerializeField] private GameObject splashPrefab;

    private float left;
    private float width;
    private float top;
    private float bottom;
    private float baseheight;
    private float surfaceLevel;
    private float[] xpositions;
    private float[] ypositions;
    private float[] velocities;
    private float[] accelerations;
    private GameObject[] colliders;

    const float springconstant = 0.02f;
    const float damping = 0.04f;
    const float spread = 0.05f;

    /*
    TODO:
        fix splash
        idle waves
        texture
        ambient particles
        liquid variants
    */

    private void Start()
    {
        RebuildWater();
        CreateTriggers();
    }

    public void Splash(float xpos, float velocity)
    {
        if (xpos >= xpositions[0] && xpos <= xpositions[xpositions.Length - 1])
        {
            xpos -= xpositions[0];
            int index = Mathf.RoundToInt((xpositions.Length - 1) * (xpos / (xpositions[xpositions.Length - 1] - xpositions[0])));
            velocities[index] = Mathf.Clamp(velocities[index] += velocity, -0.1f, 0.1f);
            float lifetime = 0.93f + Mathf.Abs(velocity) * 0.07f;
            Vector3 position = new Vector3(xpositions[index], ypositions[index] - 0.35f, 5);
            //GameObject splash = Instantiate(splashPrefab, position, Quaternion.identity);
            //Destroy(splash, lifetime + 0.3f);
        }
    }

    public void RebuildWater()
    {
        left = waterArea.bounds.min.x;
        width = waterArea.size.x;
        top = waterArea.bounds.max.y;
        bottom = waterArea.bounds.min.y;

        surfaceLevel = top + surfaceOffset;

        BuildWater();
    }

    private void BuildWater()
    {
        int edgecount = Mathf.RoundToInt(width) * 5;
        int nodecount = edgecount + 1;

        surface.positionCount = nodecount;
        surface.startWidth = 0.1f;
        surface.endWidth = 0.1f;

        xpositions = new float[nodecount];
        ypositions = new float[nodecount];
        velocities = new float[nodecount];
        accelerations = new float[nodecount];

        colliders = new GameObject[edgecount];

        baseheight = top;

        body.spline.Clear();

        body.spline.InsertPointAt(0, new Vector2(width / 2f, -waterArea.size.y));
        body.spline.InsertPointAt(1, new Vector2(-width / 2f, -waterArea.size.y));

        for (int i = 0; i < nodecount; i++)
        {
            body.spline.InsertPointAt(i + 2, new Vector2((width * i / edgecount) - width / 2f, 0));
        }

        for (int i = 0; i < nodecount; i++)
        {
            ypositions[i] = top;
            xpositions[i] = left + width * i / edgecount;
            surface.SetPosition(i, new Vector3(xpositions[i], top + surfaceOffset, 0));
            accelerations[i] = 0;
            velocities[i] = 0;
        }
    }

    private void CreateTriggers()
    {
        int edgecount = Mathf.RoundToInt(width) * 5;

        for (int i = 0; i < edgecount; i++)
        {
            colliders[i] = new GameObject
            {
                name = $"Trigger_{i}"
            };

            colliders[i].AddComponent<BoxCollider2D>();
            colliders[i].transform.parent = transform;

            colliders[i].transform.position = new Vector3(left + width * (i + 0.5f) / edgecount, top - 0.5f, 0);
            colliders[i].transform.localScale = new Vector3(width / edgecount, 1, 1);

            colliders[i].GetComponent<BoxCollider2D>().isTrigger = true;
            colliders[i].AddComponent<WaterDetector>();
        }
    }

    private void FixedUpdate()
    {
        for (int i = 0; i < xpositions.Length; i++)
        {
            float force = springconstant * (ypositions[i] - baseheight) + velocities[i] * damping;
            accelerations[i] = -force;
            ypositions[i] += velocities[i];
            velocities[i] += accelerations[i];
            surface.SetPosition(i, new Vector3(xpositions[i], ypositions[i] + surfaceOffset, 0));

            body.spline.SetPosition(i + 2, new Vector2(body.spline.GetPosition(i + 2).x, -top + ypositions[i]));
        }

        float[] leftDeltas = new float[xpositions.Length];
        float[] rightDeltas = new float[xpositions.Length];

        for (int j = 0; j < 8; j++)
        {
            for (int i = 0; i < xpositions.Length; i++)
            {
                if (i > 0)
                {
                    leftDeltas[i] = spread * (ypositions[i] - ypositions[i - 1]);
                    velocities[i - 1] += leftDeltas[i];
                }
                if (i < xpositions.Length - 1)
                {
                    rightDeltas[i] = spread * (ypositions[i] - ypositions[i + 1]);
                    velocities[i + 1] += rightDeltas[i];
                }
            }

            for (int i = 0; i < xpositions.Length; i++)
            {
                if (i > 0)
                {
                    ypositions[i - 1] += leftDeltas[i];
                }
                if (i < xpositions.Length - 1)
                {
                    ypositions[i + 1] += rightDeltas[i];
                }
            }
        }
    }
}
