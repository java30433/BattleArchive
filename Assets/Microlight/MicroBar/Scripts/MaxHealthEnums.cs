/*
 * Keep = Keeps current health intact, regardless of the max health change (current health still can't be higher than max health)
 * Follow = Follows max health change when increasing and lowering max health.
 *          For example if max health is increased by 20 points, current health is also increased by 20 points
 *          and same if lowering. Current health won't go lower than 1, resulting in death
 * FollowIncrease = Same as follow, same amount that is added to the max health will be added to the current health also.
 *                  Difference is that when lowering max health, current health won't follow and be changed.
 *                  For example if current health is at 50 and max is lowered from 100 to 80, current stays at 50.
 * Proportional = Keeps health proportional. For example if current health is 50 from 100 max, we are at 50%.
 *                If max health is increased to 150, in that case 50% is 75 so the new current health is 75
 */
namespace Microlight.MicroBar
{
    public enum MaxHealthCalculation
    {
        Keep,
        Follow,
        FollowIncrease,
        Proportional
    }
}
