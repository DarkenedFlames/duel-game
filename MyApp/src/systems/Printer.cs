using System.Text.RegularExpressions;

namespace CBA
{
    public static class Printer
    {
        public static int MultiChoiceList(string header, List<string> options, bool allowZero = true)
        {
            ClearAndHeader(header);
            PrintMenu(options);
            return InputHandler.GetNumberInput(options.Count, "Select an option: ", allowZero);
        }
        public static void PrintMenu(List<string> options)
        {
            for (int i = 0; i < options.Count; i++)
                Console.WriteLine($"{i + 1}. {options[i]}");
        }
        public static void ClearAndHeader(string header)
        {
            Console.Clear();
            Console.WriteLine(new string('-', header.Length));
            Console.WriteLine(header);
            Console.WriteLine(new string('-', header.Length));
        }
        public static void PrintMessage(string message) => Console.WriteLine(message);

        public static void PrintStats(Entity player)
        {
            ResourcesComponent resources = player.GetComponent<ResourcesComponent>();
            StatsComponent stats = player.GetComponent<StatsComponent>();

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
        public static void PrintEffects(Entity player)
        {
            IEnumerable<Entity> effects = World.Instance.GetAllForPlayer<Entity>(player, EntityCategory.Effect);

            if (!effects.Any()) { Console.WriteLine("(No active effects)"); return; }

            foreach (Entity effect in effects)
            {
                if (effect.HasComponent<EffectDuration>())
                    Console.WriteLine($"- {effect.DisplayName} (Duration: {effect.GetComponent<EffectDuration>().Remaining} turns)");
                else
                    Console.WriteLine($"- {effect.DisplayName}");
            }
        }
        // --- Unified Inventory Menu ---
        public static (Entity? SelectedItem, string? Action) ShowItemMenu(Entity player, bool equipment = false)
        {
            // --- Get items from world ---
            List<Entity> items = [.. World.Instance.GetAllForPlayer<Entity>(player, EntityCategory.Item, null, equipment)];

            // --- If equipment, order by predefined slots ---
            if (equipment)
            {
                List<(EquipType type, string label)> equipmentSlots = new()
                {
                    (EquipType.Weapon, "Weapon"),
                    (EquipType.Helmet, "Helmet"),
                    (EquipType.Chestplate, "Chestplate"),
                    (EquipType.Leggings, "Leggings"),
                    (EquipType.Accessory, "Accessory")
                };

                List<Entity> orderedEquipment = new();

                for (int i = 0; i < equipmentSlots.Count; i++)
                {
                    var (type, label) = equipmentSlots[i];
                    Entity? item = items.FirstOrDefault(e => e.HasComponent<Wearable>() && e.GetComponent<Wearable>().EquipType == type);
                    string itemName = item?.DisplayName ?? "(empty)";
                    Console.WriteLine($"{i + 1}. {label}: {itemName}");
                    if (item != null) orderedEquipment.Add(item);
                }

                if (orderedEquipment.Count == 0)
                {
                    Console.WriteLine("(No items found)");
                    return (null, null);
                }

                items = orderedEquipment;
            }
            else
            {
                if (items.Count == 0)
                {
                    Console.WriteLine("(No items found)");
                    return (null, null);
                }
            }

            // --- Player selects an item ---
            Entity? selected = InputHandler.GetChoice(items, e => e.DisplayName, "Choose an item:");
            if (selected == null) return (null, null);

            // --- Player selects an action ---
            List<string> actions = BuildItemActions(selected);
            int actionChoice = MultiChoiceList($"Item Menu: {player.DisplayName} using {selected.DisplayName}", actions);
            if (actionChoice == 0) return (null, null);

            return (selected, actions[actionChoice - 1].ToLower());
        }

        // --- Helper to generate valid actions for the selected item ---
        private static List<string> BuildItemActions(Entity item)
        {
            var actions = new List<string> { "Remove" };

            if (item.HasComponent<Usable>() && !item.HasComponent<Wearable>())
                actions.Add("Use");

            if (item.HasComponent<Wearable>())
            {
                var wearable = item.GetComponent<Wearable>();
                if (wearable.IsEquipped)
                {
                    actions.Add("Unequip");
                    if (item.HasComponent<Usable>())
                        actions.Add("Use");
                }
                else
                {
                    actions.Add("Equip");
                }
            }

            return actions;
        }

        //================== Event Printers =================//
        public static void PrintEntityAdded(Entity entity)
        {
            switch (entity.Id.Category)
            {
                case EntityCategory.Player:
                    Console.WriteLine($"\n{entity.DisplayName} has joined the game!");
                    break;
                case EntityCategory.Item:
                    Console.WriteLine($"\n{entity.GetComponent<ItemData>().PlayerEntity.DisplayName} has picked up {entity.DisplayName}!");
                    break;
                case EntityCategory.Effect:
                    string playerName = entity.GetComponent<EffectData>().PlayerEntity.DisplayName;
                    string effectName = entity.DisplayName;
                    if (entity.HasComponent<EffectDuration>())
                    {
                        EffectDuration effectDuration = entity.GetComponent<EffectDuration>();
                        Console.WriteLine($"\n{playerName} has gained an effect: {effectName}! Remaining turns: {effectDuration.Remaining}");
                    }
                    else
                    {
                        Console.WriteLine($"\n{playerName} has gained an effect: {effectName}!");
                    }
                    break;
                default:
                    throw new Exception($"Printer couldn't find type of entity removed.");
            }
            InputHandler.WaitForKey();
        }
        public static void PrintEntityRemoved(Entity entity)
        {
            switch (entity.Id.Category)
            {
                case EntityCategory.Player:
                    Console.WriteLine($"\n{entity.DisplayName} has died!");
                    break;
                case EntityCategory.Item:
                    Console.WriteLine($"\n{entity.GetComponent<ItemData>().PlayerEntity.DisplayName} has lost {entity.DisplayName}!");
                    break;
                case EntityCategory.Effect:
                    string playerName = entity.GetComponent<EffectData>().PlayerEntity.DisplayName;
                    string effectName = entity.DisplayName;

                    if (entity.HasComponent<EffectDuration>())
                    {
                        Console.WriteLine($"\n{playerName}'s effect: {effectName} has expired!");
                    }
                    else
                    {
                        Console.WriteLine($"\n{playerName} has lost an effect: {effectName}!");
                    }
                    break;
                default:
                    throw new Exception($"Printer couldn't find type of entity removed.");
            }
            InputHandler.WaitForKey();
        }

        public static void PrintItemEquipped(Entity item)
        {
            Console.WriteLine($"\n{item.GetComponent<ItemData>().PlayerEntity.DisplayName} equipped {item.DisplayName}!");
        }
        public static void PrintItemUnequipped(Entity item)
        {
            Console.WriteLine($"\n{item.GetComponent<ItemData>().PlayerEntity.DisplayName} unequipped {item.DisplayName}!");
        }
        public static void PrintItemUsed(Entity item, Entity target)
        {

            Entity user = item.GetComponent<ItemData>().PlayerEntity;

            string itemName = item.DisplayName;
            string userName = user.DisplayName;
            string targetName = target.DisplayName;

            if (user == target)
            {
                Console.WriteLine($"\n{userName} used {itemName} on themselves!");
            }
            else
            {
                Console.WriteLine($"\n{userName} used {itemName} on {targetName}!");
            }

        }
        public static void PrintItemConsumed(Entity item)
        {
            string itemName = item.DisplayName;
            string playerName = item.GetComponent<ItemData>().PlayerEntity.DisplayName;
            Console.WriteLine($"\n{playerName} consumed their {itemName}!");
        }

        public static void PrintInsufficientStamina(Entity item)
        {
            Console.WriteLine($"\n{item.GetComponent<ItemData>().PlayerEntity.DisplayName} lacks stamina to use {item.DisplayName}.");
        }

        public static void PrintStatChanged(StatsComponent stats, string statName)
        {
            var formattedStatName =  Regex.Replace(statName, "(?<=[a-z])(?=[A-Z])|(?<=[A-Z])(?=[A-Z][a-z])|(?<=[a-zA-Z])(?=[0-9])|(?<=[0-9])(?=[a-zA-Z])", " ");
            Console.WriteLine($"\n{stats.Owner.DisplayName}'s {formattedStatName} changed to {stats.Get(statName)}.");
        }
        public static void PrintResourceChanged(ResourcesComponent resources, string resourceName)
        {
            string formattedResourceName =  Regex.Replace(resourceName, "(?<=[a-z])(?=[A-Z])|(?<=[A-Z])(?=[A-Z][a-z])|(?<=[a-zA-Z])(?=[0-9])|(?<=[0-9])(?=[a-zA-Z])", " ");
            Console.WriteLine($"\n{resources.Owner.DisplayName}'s {formattedResourceName} is now {resources.Get(resourceName)}.");
        }
        public static void PrintResourceDepleted(ResourcesComponent resources, string resourceName)
        {
            string formattedResourceName =  Regex.Replace(resourceName, "(?<=[a-z])(?=[A-Z])|(?<=[A-Z])(?=[A-Z][a-z])|(?<=[a-zA-Z])(?=[0-9])|(?<=[0-9])(?=[a-zA-Z])", " ");
            Console.WriteLine($"\n{resources.Owner.DisplayName}'s {formattedResourceName} has been depleted!");
        }

        public static void PrintDamageDealt(Entity itemOrEffect, Entity target, int finalDamage)
        {
            string userName;
            string sourceName = itemOrEffect.DisplayName;
            string targetName = target.DisplayName;

            if (itemOrEffect.Id.Category == EntityCategory.Item)
            {
                if (itemOrEffect.GetComponent<ItemData>().PlayerEntity == target)
                {
                    userName = targetName;
                    Console.WriteLine($"\n{userName} damaged themselves with {sourceName} for {finalDamage} damage!");
                }
                else
                {
                    userName = itemOrEffect.GetComponent<ItemData>().PlayerEntity.DisplayName;
                    Console.WriteLine($"\n{userName} dealt {finalDamage} damage to {targetName} with {sourceName}!");
                }
            }
            else if (itemOrEffect.Id.Category == EntityCategory.Effect)
            {
                sourceName = itemOrEffect.DisplayName;
                Console.WriteLine($"\n{targetName} took {finalDamage} damage from {sourceName}!");
            }
        }

        public static void PrintDodged(Entity itemOrEffect, Entity target)
        {
            string userName;
            string targetName = target.DisplayName;
            string sourceName = itemOrEffect.DisplayName;

            if (itemOrEffect.Id.Category == EntityCategory.Item)
            {
                userName = itemOrEffect.GetComponent<ItemData>().PlayerEntity.DisplayName;
                Console.WriteLine($"\n{userName}'s attack with {sourceName} was dodged by {targetName}!");

            }
            else if (itemOrEffect.Id.Category == EntityCategory.Effect)
            {
                Console.WriteLine($"\n{targetName} dodged damage from their {sourceName} effect!");
            }
            else
            {
                throw new NullReferenceException($"{itemOrEffect} was null when it should have been an <Entity>.");
            }
        }

        public static void PrintCritical(Entity itemOrEffect, Entity target)
        {
            string userName;
            string targetName = target.DisplayName;
            string sourceName = itemOrEffect.DisplayName;

            if (itemOrEffect.Id.Category == EntityCategory.Item)
            {
                userName = itemOrEffect.GetComponent<ItemData>().PlayerEntity.DisplayName;
                Console.WriteLine($"\n{userName}'s attack with {sourceName} scored a critical hit on {targetName}!");

            }
            else if (itemOrEffect.Id.Category == EntityCategory.Effect)
            {
                Console.WriteLine($"\n{targetName} was critically damaged by their {sourceName} effect!");
            }
            else
            {
                throw new NullReferenceException($"{itemOrEffect} was null when it should have been an <Entity>.");
            }
        }

        public static void PrintPeered(List<string> targetItems, Entity target)
        {
            Console.WriteLine($"\nPeer activated! {target.DisplayName}'s inventory:");
            Console.WriteLine(string.Join(", ", targetItems));
            InputHandler.WaitForKey();
        }
        /*
        public static void PrintEffectStacked(Entity player, Effect effect)
        {
            Console.WriteLine($"\n{player.Name}'s effect {effect.Name} stacked to {effect.RemainingStacks} stacks!");
        }
        */
    }
}
