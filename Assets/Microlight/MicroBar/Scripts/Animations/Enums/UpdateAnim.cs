namespace Microlight.MicroBar
{
    // ****************************************************************************************************
    // Defines what animation will be used to update the bar
    // *Enum must be public because editor scripts are in another assembly
    // ****************************************************************************************************

    public enum UpdateAnim
    {
        Damage,
        Heal,
        CriticalDamage,
        CriticalHeal,
        Armor,   // When armor is damaged
        DOT,   // When DOT ticks
        HOT,   // When HOT ticks
        MaxHP,   // When changing max HP
        Revive,   // When user is revived
        Death,   // When user dies
        Custom   // When custom animation will be used
    }
}
