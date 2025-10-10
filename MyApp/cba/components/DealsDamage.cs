using System;

namespace CBA
{
    public class DealsDamage(Entity owner, int damage,
                            DamageType damageType = DamageType.Physical,
                            bool canCrit = false,
                            bool canDodge = false) : Component(owner)
    {
        public int Damage { get; } = damage;
        public DamageType DamageType { get; init; } = damageType;
        public bool CanCrit { get; init; } = canCrit;
        public bool CanDodge { get; init; } = canDodge;

        public event Action<Entity, Entity, int>? OnDamageDealt;

        protected override void Subscribe()
        {
            OnDamageDealt += (user, target, finalDamage) => Printer.PrintDamageDealt(user, target, finalDamage);

            var usable = Owner.GetComponent<Usable>();
            if (usable != null)
            {
                usable.OnUseSuccess += (user, target) =>
                {
                    int finalDamage = Damage;

                    var playerData = target.GetComponent<PlayerData>();
                    string targetName = playerData?.Name ?? "Unknown";

                    var targetStats = target.GetComponent<StatsComponent>();
                    if (targetStats != null)
                    {
                        // Dodge logic
                        float dodgeChance = targetStats.Get("Dodge") / (targetStats.Get("Dodge") + 100f);
                        if (CanDodge && Random.Shared.NextDouble() < dodgeChance)
                        {
                            Console.WriteLine($"{targetName} dodged the attack!");
                            finalDamage = 0;
                        }

                        // Crit logic
                        float critChance = user.GetComponent<StatsComponent>()?.Get("Critical") ?? 0;
                        critChance /= critChance + 100f;
                        if (CanCrit && Random.Shared.NextDouble() < critChance)
                        {
                            finalDamage = (int)(finalDamage * 2.0f);
                            Console.WriteLine("Critical Hit!");
                        }

                        // Damage type reduction
                        switch (DamageType)
                        {
                            case DamageType.Physical:
                                finalDamage = (int)(finalDamage * (100f / (targetStats.Get("Armor") + 100f)));
                                break;
                            case DamageType.Magical:
                                finalDamage = (int)(finalDamage * (100f / (targetStats.Get("Shield") + 100f)));
                                break;
                            case DamageType.True:
                                break;
                        }

                        var targetResources = target.GetComponent<ResourcesComponent>();
                        targetResources?.Change("Health", finalDamage);
                        OnDamageDealt?.Invoke(Owner, target, finalDamage);
                    }
                };
            }
        }
    }
}
