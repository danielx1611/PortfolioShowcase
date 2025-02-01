using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetherManager : MonoBehaviour
{
    private List<LineRenderer> lineRenderers = new List<LineRenderer>();
    private List<GameObject> targetObjects = new List<GameObject>();

    public float minWidth = 0.05f;
    public float maxWidth = 0.2f;
    public Material lineMaterial; // Assign a material for the line in the Inspector

    [SerializeField] private Color startColor = Color.white;
    [SerializeField] private Color endColor = Color.white;

    public void UpdateTethers()
    {
        List<LineRenderer> nullTethers = new List<LineRenderer>();
        List<GameObject> nullObjects = new List<GameObject>();

        for (int i = 0; i < lineRenderers.Count; i++)
        {
            LineRenderer lineRenderer = lineRenderers[i];
            BasicEnemy target = targetObjects[i].GetComponent<BasicEnemy>();

            if (target.IsDead())
            {
                nullTethers.Add(lineRenderer);
                nullObjects.Add(targetObjects[i]);
                continue;
            }

            // Set start point at self and end point of line at target
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, target.transform.position);

            // Adjust width based on distance
            float distance = Vector2.Distance(transform.position, target.transform.position);
            float width = Mathf.Lerp(maxWidth, minWidth, distance / 5f);
            lineRenderer.startWidth = width;
            lineRenderer.endWidth = width;
        }

        for (int i = 0; i < nullTethers.Count; i++)
        {
            RemoveTetherFromList(nullTethers[i], nullObjects[i]);
        }
    }

    public void CreateNewTether(GameObject target)
    {
        GameObject lineObj = new GameObject("LineRenderer"); // Create a new GameObject for the line
        LineRenderer lineRenderer = lineObj.AddComponent<LineRenderer>(); // Attach a lineRenderer to it

        // Set lineRenderer attributes
        lineRenderer.positionCount = 2;
        lineRenderer.material = lineMaterial;
        lineRenderer.startColor = startColor;
        lineRenderer.endColor = endColor;
        lineRenderer.startWidth = maxWidth;
        lineRenderer.endWidth = maxWidth;
        lineRenderer.useWorldSpace = true;

        // Add newly created tether to tether list
        lineRenderers.Add(lineRenderer);

        targetObjects.Add(target);
    }

    public void DestroyAllTethers()
    {
        for (int i = 0; i < lineRenderers.Count; i++)
        {
            Destroy(lineRenderers[i]); // Don't destroy targets, they'll destroy themselves if needed
        }

        lineRenderers.Clear();
        targetObjects.Clear();
    }

    private void RemoveTetherFromList(LineRenderer renderer, GameObject target)
    {
        Destroy(renderer);
        lineRenderers.Remove(renderer);
        targetObjects.Remove(target);
    }
}
