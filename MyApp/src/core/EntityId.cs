using System.ComponentModel.DataAnnotations;

namespace CBA
{
    public enum EntityCategory
    {
        Player,
        Item,
        Effect
    }

    public class EntityId(EntityCategory category, string typeId, int instanceId)
    {
        public EntityCategory Category { get; } = category;
        public string TypeId { get; } = typeId;
        public int InstanceId { get; } = instanceId;

        public override string ToString() => $"{Category}:{TypeId}:{InstanceId}";
    }

}