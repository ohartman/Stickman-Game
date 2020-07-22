using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimbPart{
    private Rigidbody2D rigidbody;
    private Transform transform;
    
    public Vector2 RelativeOffset => offset.x * transform.right + offset.y * transform.up;
    public Vector2 offset;

    private bool flip = false;
    private bool sin = false;
    
    public LimbPart(Rigidbody2D rigidbody, bool fliped, bool sin) {
        this.rigidbody = rigidbody;
        this.transform = rigidbody.transform;
        this.flip = fliped;
        this.sin = sin;

        var hinge = transform.GetComponent<HingeJoint2D>();
        offset = hinge.anchor;
    }
    Vector2 avgDirection;
    Vector2 avgWorldPos;
    const float wratio = .95f;
    public void RotateToward(Vector2 worldPos, float force, float ratio) {
        var dif = (worldPos - avgWorldPos).magnitude;
        if(dif > 1) {
            dif = 1;

        }
        dif = 1 - dif;
        avgWorldPos = worldPos * (1 - wratio) + avgWorldPos * wratio;
        ratio += (1-ratio) * dif * .95f;
        var direction = worldPos - (RelativeOffset + (Vector2)transform.position);
        if(flip) {
            direction *= -1;
        }
        avgDirection = direction * (1 - ratio) + avgDirection * ratio;
        var angle = AngleFromDirection(avgDirection);
        var cangle = transform.eulerAngles.z;
        if(sin) {
            cangle -= 90;
        }
        angle = Mathf.DeltaAngle(cangle, angle);
        var x = angle > 0 ? 1 : -1;
        angle = Mathf.Abs(angle* .1f);
        if(angle > 2) {
            angle = 2;
        }
        angle *= .5f;
        angle *= (1 + angle);
        
        rigidbody.angularVelocity *= angle * .5f;
        rigidbody.AddTorque(angle * force * x);
        Debug.Log(angle);
    }

    private float AngleFromDirection(Vector2 dir) {
        dir = dir.normalized;
        var angle = Mathf.Acos(dir.x) * Mathf.Rad2Deg;
        return dir.y > 0 ? angle : 360 - angle;
    }
    
    public void OnDrawGizmos() {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere((Vector2)transform.position + RelativeOffset, .1f);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere((Vector2)transform.position - RelativeOffset, .1f);
    }
}
