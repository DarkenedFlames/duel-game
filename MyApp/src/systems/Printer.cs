namespace CBA
{
    public static class Printer
    {
        // === General Helpers ===
        public static void ClearAndHeader(string header)
        {
            Console.Clear();
            Console.WriteLine(new string('-', header.Length));
            Console.WriteLine(header);
            Console.WriteLine(new string('-', header.Length));
        }

        // === Player Stats ===
        public static void PrintStats(Entity player)
        {
            var resources = player.GetComponent<ResourcesComponent>();
            var stats = player.GetComponent<StatsComponent>();

            if (resources != null && stats != null)
            {
                Console.WriteLine($"Health: {resources.Get("Health")} / {stats.Get("MaximumHealth")}");
                Console.WriteLine($"Stamina: {resources.Get("Stamina")} / {stats.Get("MaximumStamina")}");
                Console.WriteLine($"Armor: {stats.Get("Armor")}");
                Console.WriteLine($"Shield: {stats.Get("Shield")}");
                Console.WriteLine($"Critical: {stats.Get("Critical")}");
                Console.WriteLine($"Dodge: {stats.Get("Dodge")}");
                Console.WriteLine($"Peer: {stats.Get("Peer")}");
                Console.WriteLine($"Luck: {stats.Get("Luck")}");
                Console.WriteLine($"Healing Modifier: {resources.GetRestoreMultiplier("Health"):P}");
                Console.WriteLine($"Stimming Modifier: {resources.GetRestoreMultiplier("Stamina"):P}");
            }
            else
            {
                Console.WriteLine($"{player.GetComponent<PlayerData>()?.Name} missing Stats or Resources component.");
            }
        }

        // === Status Effects ===
        public static void PrintEffects(Entity player)
        {
            var effects = World.Instance.GetEntitiesWith<EffectData>()
                .Where(e => e.GetComponent<EffectData>()?.PlayerEntity == player);

            if (!effects.Any())
            {
                Console.WriteLine("(No active effects)");
                return;
            }

            foreach (var effect in effects)
            {
                var data = effect.GetComponent<EffectData>();
                var duration = effect.GetComponent<EffectDuration>()?.Remaining;
                Console.WriteLine($"- {data?.Name ?? "Unknown"} (Duration: {duration} turns)");
            }
        }

        // === Item Lists ===
        public static void PrintItemList(IEnumerable<Entity> items)
        {
            int idx = 1;
            foreach (var item in items)
            {
                var itemName = item.GetComponent<ItemData>()?.Name ?? "Unknown";
                Console.WriteLine($"{idx}. {itemName}");
                idx++;
            }

            if (idx == 1)
                Console.WriteLine("(No items found)");
        }

        // === Equipment Slots ===
        public static void PrintEquipment(Entity player)
        {
            var equippedItems = World.Instance.GetEntitiesWith<Wearable>()
                .Where(e =>
                {
                    var wearable = e.GetComponent<Wearable>();
                    var itemData = e.GetComponent<ItemData>();
                    return wearable != null &&
                           itemData != null &&
                           wearable.IsEquipped &&
                           itemData.PlayerEntity == player;
                })
                .ToList();

            var slots = new List<(EquipType type, string label)>
            {
                (EquipType.Weapon, "Weapon"),
                (EquipType.Helmet, "Helmet"),
                (EquipType.Chestplate, "Chestplate"),
                (EquipType.Leggings, "Leggings"),
                (EquipType.Accessory, "Accessory")
            };

            foreach (var (type, label) in slots)
            {
                var itemName = equippedItems.FirstOrDefault(e => e.GetComponent<Wearable>()?.EquipType == type)
                    ?.GetComponent<ItemData>()?.Name ?? "(empty)";
                Console.WriteLine($"{label}: {itemName}");
            }
        }

        // === Item Menu Actions ===
        public static string? PrintItemMenu(Entity itemEntity)
        {
            var itemData = itemEntity.GetComponent<ItemData>();
            var player = itemData?.PlayerEntity;
            var playerData = player?.GetComponent<PlayerData>();
            var wearable = itemEntity.GetComponent<Wearable>();
            var allPlayers = World.Instance.GetEntitiesWith<PlayerData>().ToList();

            if (itemData == null || playerData == null)
            {
                Console.WriteLine("Invalid entity or missing components.");
                return null;
            }

            ClearAndHeader($"Item Menu: {playerData.Name} using {itemData.Name}");

            // Determine available actions
            var actions = new List<string> { "Remove" };

            if (itemData.PlayerEntity == player)
            {
                if (wearable != null)
                {
                    actions.Add(wearable.IsEquipped ? "Unequip" : "Equip");

                    if (itemData.Type == ItemType.Weapon && wearable.IsEquipped)
                        actions.Add("Use");
                }
                else if (itemData.Type == ItemType.Consumable)
                {
                    actions.Add("Use");
                }
            }

            // Display actions
            for (int i = 0; i < actions.Count; i++)
                Console.WriteLine($"{i + 1}. {actions[i]}");

            Console.WriteLine("Enter a number to select an action, or press Enter to cancel.");

            string? input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input))
                return null;

            if (!int.TryParse(input, out int choice) || choice < 1 || choice > actions.Count)
            {
                Console.WriteLine("Invalid choice.");
                return null;
            }

            return actions[choice - 1]; // return the selected action string
        }

        //================== Event Printers =================//
        public static void PrintTurnStartHeader(Entity player)
        {
            Console.Clear();
            Console.WriteLine($"-----\n{player.GetComponent<PlayerData>()?.Name}'s turn has began!-----");
        }
        public static void PrintTurnEndHeader(Entity player)
        {
            Console.WriteLine($"-----\n{player.GetComponent<PlayerData>()?.Name}'s turn has ended!-----");
        }


        public static void PrintEntityAdded(Entity entity)
        {
            if (entity.GetComponent<PlayerData>() != null)
            {
                Console.WriteLine($"\n{entity.GetComponent<PlayerData>()?.Name} has joined the game!");
                TurnManager.WaitForKey();
            }
            if (entity.GetComponent<ItemData>() != null)
            {
                var itemData = entity.GetComponent<ItemData>();
                Console.WriteLine($"\n{itemData?.PlayerEntity.GetComponent<PlayerData>()?.Name} has picked up {itemData?.Name}!");
                TurnManager.WaitForKey();
            }
            if (entity.GetComponent<EffectData>() != null)
            {
                var effectData = entity.GetComponent<EffectData>();
                var effectDuration = entity.GetComponent<EffectDuration>();
                if (effectDuration != null)
                {
                    Console.WriteLine($"\n{effectData?.PlayerEntity.GetComponent<PlayerData>()?.Name} has gained an effect: {effectData?.Name}! Remaining turns: {effectDuration.Remaining}");
                    TurnManager.WaitForKey();
                }
                else
                {
                    Console.WriteLine($"\n{effectData?.PlayerEntity.GetComponent<PlayerData>()?.Name} has gained an effect: {effectData?.Name}!");
                    TurnManager.WaitForKey();
                }
            }
        }
        public static void PrintEntityRemoved(Entity entity)
        {
            if (entity.GetComponent<PlayerData>() != null)
            {
                Console.WriteLine($"\n{entity.GetComponent<PlayerData>()?.Name} has died!");
                TurnManager.WaitForKey();
            }
            if (entity.GetComponent<ItemData>() != null)
            {
                var itemData = entity.GetComponent<ItemData>();
                Console.WriteLine($"\n{itemData?.PlayerEntity.GetComponent<PlayerData>()?.Name} has lost {itemData?.Name}!");
                TurnManager.WaitForKey();
            }
            if (entity.GetComponent<EffectData>() != null)
            {
                var effectData = entity.GetComponent<EffectData>();
                var effectDuration = entity.GetComponent<EffectDuration>();
                if (effectDuration != null)
                {
                    Console.WriteLine($"\n{effectData?.PlayerEntity.GetComponent<PlayerData>()?.Name}'s effect: {effectData?.Name} has expired!");
                    TurnManager.WaitForKey();
                }
                else
                {
                    Console.WriteLine($"\n{effectData?.PlayerEntity.GetComponent<PlayerData>()?.Name} has lost an effect: {effectData?.Name}!");
                    TurnManager.WaitForKey();
                }
            }
        }

        public static void PrintItemEquipped(Entity item)
        {
            var itemData = item.GetComponent<ItemData>();
            var playerName = itemData?.PlayerEntity.GetComponent<PlayerData>()?.Name;
            var itemName = itemData?.Name;
            Console.WriteLine($"\n{playerName} equipped {itemName}!");
            TurnManager.WaitForKey();
        }
        public static void PrintItemUnequipped(Entity item)
        {
            var itemData = item.GetComponent<ItemData>();
            var playerName = itemData?.PlayerEntity.GetComponent<PlayerData>()?.Name;
            var itemName = itemData?.Name;
            Console.WriteLine($"\n{playerName} unequipped {itemName}!");
            TurnManager.WaitForKey();
        }
        public static void PrintItemUsed(Entity item, Entity target)
        {
            var itemData = item.GetComponent<ItemData>();
            var itemName = itemData?.Name;
            var player = itemData?.PlayerEntity;
            bool usedOnSelf = player == target;
            var playerName = player?.GetComponent<PlayerData>()?.Name;
            var targetName = target.GetComponent<PlayerData>()?.Name;

            if (!usedOnSelf)
            {
                Console.WriteLine($"\n{playerName} used {itemName} on themselves!");
                TurnManager.WaitForKey();
            }
            else
            {
                Console.WriteLine($"\n{playerName} used {itemName} on {targetName}!");
                TurnManager.WaitForKey();
            }

        }
        public static void PrintItemConsumed(Entity item)
        {
            var itemData = item.GetComponent<ItemData>();
            var itemName = itemData?.Name;
            var playerName = itemData?.PlayerEntity.GetComponent<PlayerData>()?.Name;
            Console.WriteLine($"\n{playerName} consumed their {itemName}!");
            TurnManager.WaitForKey();
        }

        public static void PrintInsufficientStamina(Entity item)
        {
            var itemData = item.GetComponent<ItemData>();
            var playerName = itemData?.PlayerEntity.GetComponent<PlayerData>()?.Name;
            var itemName = itemData?.Name;
            Console.WriteLine($"\n{playerName} lacks stamina to use {itemName}.");
            TurnManager.WaitForKey();
        }

        public static void PrintStatChanged(StatsComponent stats, string statName)
        {
            var playerName = stats.Owner.GetComponent<PlayerData>();
            Console.WriteLine($"\n{playerName}'s {statName} changed to {stats.Get(statName)}.");
            TurnManager.WaitForKey();
        }
        public static void PrintResourceChanged(ResourcesComponent resources, string resourceName)
        {
            var playerName = resources.Owner.GetComponent<PlayerData>();
            Console.WriteLine($"\n{playerName}'s {resourceName} is now {resources.Get(resourceName)}.");
            TurnManager.WaitForKey();
        }
        public static void PrintResourceDepleted(ResourcesComponent resources, string resourceName)
        {
            var playerName = resources.Owner.GetComponent<PlayerData>();
            Console.WriteLine($"\n{playerName}'s {resourceName} has been depleted!");
            TurnManager.WaitForKey();
        }

        public static void PrintDamageDealt(Entity itemOrEffect, Entity target, int finalDamage)
        {
            var itemData = itemOrEffect.GetComponent<ItemData>();
            var effectData = itemOrEffect.GetComponent<EffectData>();

            bool isItem = itemData != null;
            var user = isItem ? itemData?.PlayerEntity : effectData?.PlayerEntity;
            bool selfDamage = user == target;

            string? userName = user?.GetComponent<PlayerData>()?.Name;
            string? targetName = target.GetComponent<PlayerData>()?.Name;
            string? sourceName = isItem ? itemData?.Name : effectData?.Name;

            if (isItem)
            {
                if (selfDamage)
                {
                    Console.WriteLine($"\n{userName} damaged themselves with {sourceName} for {finalDamage} damage!");
                    TurnManager.WaitForKey();
                }
                else
                {
                    Console.WriteLine($"\n{userName} dealt {finalDamage} damage to {targetName} with {sourceName}!");
                    TurnManager.WaitForKey();
                }
            }
            else
            {
                Console.WriteLine($"\n{userName} took {finalDamage} damage from {sourceName}!");
                TurnManager.WaitForKey();
            }
        }

        public static void PrintDodged(Entity itemOrEffect, Entity target)
        {
            var itemData = itemOrEffect.GetComponent<ItemData>();
            var effectData = itemOrEffect.GetComponent<EffectData>();
            var targetName = target.GetComponent<PlayerData>()?.Name;

            if (itemData != null)
            {
                var itemName = itemData.Name;
                var playerName = itemData.PlayerEntity.GetComponent<PlayerData>()?.Name;
                Console.WriteLine($"\n{playerName}'s attack with {itemName} was dodged by {targetName}!");
                TurnManager.WaitForKey();

            }
            else if (effectData != null)
            {
                var effectName = effectData.Name;
                Console.WriteLine($"\n{targetName} dodged damage from their {effectName} effect!");
                TurnManager.WaitForKey();
            }
            else
            {
                throw new NullReferenceException($"{itemOrEffect} was null when it should have been an <Entity>.");
            }
        }

        public static void PrintCritical(Entity itemOrEffect, Entity target)
        {
            var itemData = itemOrEffect.GetComponent<ItemData>();
            var effectData = itemOrEffect.GetComponent<EffectData>();
            var targetName = target.GetComponent<PlayerData>()?.Name;

            if (itemData != null)
            {
                var itemName = itemData.Name;
                var playerName = itemData.PlayerEntity.GetComponent<PlayerData>()?.Name;
                Console.WriteLine($"\n{playerName}'s attack with {itemName} scored a critical hit on {targetName}!");
                TurnManager.WaitForKey();

            }
            else if (effectData != null)
            {
                var effectName = effectData.Name;
                Console.WriteLine($"\n{targetName} was critically damaged by their {effectName} effect!");
                TurnManager.WaitForKey();
            }
            else
            {
                throw new NullReferenceException($"{itemOrEffect} was null when it should have been an <Entity>.");
            }
        }

        public static void PrintPeered(List<String> targetItems, Entity target)
        {
            Console.WriteLine($"\nPeer activated! {target.GetComponent<PlayerData>()?.Name ?? "Enemy"}'s inventory:");
            Console.WriteLine(string.Join(", ", targetItems));
            TurnManager.WaitForKey();
        }
        /*
        public static void PrintEffectStacked(Entity player, Effect effect)
        {
            Console.WriteLine($"\n{player.Name}'s effect {effect.Name} stacked to {effect.RemainingStacks} stacks!");
        }
        */
    }
}