using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class GlowOutline : MonoBehaviour
{
    public LineRenderer line;
    public float glowSpeed = .3f;
    private bool glowing = false;

    void Start()
    {
        //debug
        line.startWidth = 0.02f;
        line.endWidth = 0.02f;

        line.enabled = false;
        BuildOutline();
    }

    void Update()
    {
        if (glowing)
        {
            float offset = Time.time * glowSpeed;
            line.material.SetTextureOffset("_MainTex", new Vector2(offset, 0));
        }
    }
    public void Glow()
    {
        line.enabled = true;
        glowing = true;
    }

    public void Dim()
    {
        line.enabled = false;
    }
    
    void BuildOutline()
    {
        Collider2D col = GetComponent<Collider2D>();

        if (col is BoxCollider2D box)
        {
            Vector2 size = box.size / 2f;

            Vector3[] points =
            {
                new Vector3(-size.x, -size.y),
                new Vector3(-size.x,  size.y),
                new Vector3( size.x,  size.y),
                new Vector3( size.x, -size.y)
            };

            line.positionCount = points.Length;
            line.SetPositions(points);
        }

        else if (col is PolygonCollider2D poly)
        {
            Vector2[] pts = poly.points;
            line.positionCount = pts.Length;

            for (int i = 0; i < pts.Length; i++)
                line.SetPosition(i, pts[i]);
        }
    }
}

