namespace Godot.Common
{
    public static class SteeringBehaviors
    {
        public static Vector2 Follow(CharacterBody2D entity, Vector2 targetPos, float maxSpeed)
        {
            Vector2 desiredVel = (targetPos - entity.GlobalPosition).Normalized() * maxSpeed;
            Vector2 steeringVec = (desiredVel - entity.Velocity);

            return steeringVec;
        }

        public static Vector2 Arrive(CharacterBody2D entity, Vector2 targetPos, float maxSpeed, float slowingRadius)
        {
            Vector2 desiredVel = targetPos - entity.GlobalPosition;
            float distance = desiredVel.Length();

            if (distance < slowingRadius)
                desiredVel = desiredVel.Normalized() * maxSpeed * (distance / slowingRadius);
            else
                desiredVel = desiredVel.Normalized() * maxSpeed;

            Vector2 steeringVec = (desiredVel - entity.Velocity);
            return steeringVec;
        }
    }
}
