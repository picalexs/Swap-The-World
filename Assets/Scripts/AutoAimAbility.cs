using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoAimAbility : MonoBehaviour
{
    public string targetTag;
    public float angleRadius = 5f;
    public float maxDistance = 100f;
    public float angleStep;
    public float numRaycasts;
    public LineRenderer directionLine;
    public LineRenderer closestObjectLine;
    public Transform playerTransform;
    public GameObject closestObject;

    private Camera cam;
    private Vector3 aimDirection;
    private RaycastHit2D[] hits;

    private void Start()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        FindClosestObject();
        DrawDirectionLine();
    }

    private void FindClosestObject()
    {
        Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        aimDirection = (mousePos - playerTransform.position).normalized;
        closestObject = null;
        float closestDistance = Mathf.Infinity;

        for (int i = 0; i < numRaycasts; i++)
        {
            float angle = i * angleStep - angleRadius / 2f;
            Vector3 dir = Quaternion.AngleAxis(angle, Vector3.forward) * aimDirection;
            hits = Physics2D.RaycastAll(playerTransform.position, dir, maxDistance);

            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider.CompareTag(targetTag))
                {
                    Vector3 dirToHit = hit.collider.transform.position - playerTransform.position;
                    float angleToHit = Vector3.Angle(aimDirection, dirToHit);

                    if (angleToHit <= angleRadius)
                    {
                        float distance = Vector3.Distance(playerTransform.position, hit.collider.transform.position);
                        if (distance < closestDistance)
                        {
                            closestObject = hit.collider.gameObject;
                            closestDistance = distance;
                        }
                    }
                }
            }
        }

        if (closestObject != null)
        {
            closestObjectLine.enabled = true;
            closestObjectLine.SetPosition(0, playerTransform.position);
            closestObjectLine.SetPosition(1, closestObject.transform.position);
            Debug.Log(closestObject.name);
        }
        else
        {
            closestObjectLine.enabled = false;
        }
    }


    private void DrawDirectionLine()
    {
        directionLine.SetPosition(0, playerTransform.position);
        directionLine.SetPosition(1, playerTransform.position + aimDirection * maxDistance);

        Vector3 lineDirection = aimDirection;
        lineDirection.z = 0;
        directionLine.transform.rotation = Quaternion.FromToRotation(Vector3.right, lineDirection);
    }
}
