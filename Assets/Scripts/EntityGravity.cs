using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EntityGravity
{
    public static Vector2 modifyEntityGravity(Rigidbody2D entity_rb)
    {
        if (entity_rb.velocity.y < 0)
            return entity_rb.velocity;

        Vector2 temp_velocity = entity_rb.velocity;
        temp_velocity.y *= 1f - 0.03f;

        return temp_velocity;
    }

}
