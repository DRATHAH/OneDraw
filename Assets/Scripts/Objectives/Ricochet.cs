using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ricochet : MonoBehaviour
{
    [Tooltip("Where arrow will face upon hitting ricochet surface.")]
    public Transform ricochetDirection;
    public LayerMask arrowLayer;
    public float ricochetForce = 1000f;

    private void OnTriggerEnter(Collider collision)
    {
        Arrow arrow = collision.transform.GetComponentInParent<Arrow>();
        if (arrow)
        {
            Vector3 forward = ricochetDirection.TransformDirection(Vector3.forward);
            Vector3 direction = (arrow.transform.position - ricochetDirection.position).normalized;
            Ray directRay = new Ray(ricochetDirection.position, direction);
            RaycastHit obstacle;
            if (Physics.Raycast(directRay, out obstacle, 10, arrowLayer))
            {
                Arrow tempArrow = obstacle.transform.GetComponentInParent<Arrow>();
                if (tempArrow && tempArrow.Equals(arrow) && Vector3.Dot(forward, direction) >= 0f)
                {
                    Rigidbody arrowRB = arrow.GetComponent<Rigidbody>();
                    arrowRB.velocity = Vector3.zero;
                    arrow.transform.position = ricochetDirection.position + ricochetDirection.forward;
                    arrow.transform.rotation = ricochetDirection.rotation;
                    Vector3 forceToApply = ricochetDirection.forward * ricochetForce;
                    arrow.GetComponent<Rigidbody>().AddForce(forceToApply, ForceMode.Impulse);
                    arrow.Supercharge();
                }
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
