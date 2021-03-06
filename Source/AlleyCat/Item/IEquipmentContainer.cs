using System.Linq;
using AlleyCat.Common;
using AlleyCat.Item.Generic;
using EnsureThat;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Item
{
    public interface IEquipmentContainer : ISlotContainer<EquipmentSlot, Equipment>
    {
    }

    public static class EquipmentContainerExtensions
    {
        [CanBeNull]
        public static EquipmentConfiguration FindConfiguration(
            [NotNull] this IEquipmentContainer container,
            [NotNull] Equipment item,
            params string[] tags)
        {
            Ensure.Any.IsNotNull(container, nameof(container));
            Ensure.Any.IsNotNull(item, nameof(item));

            var allConfigs = item.Configurations.Values;

            return (tags.Any() ? allConfigs.TaggedAny(tags) : allConfigs).FirstOrDefault(container.AllowedFor);
        }

        [CanBeNull]
        public static Equipment Equip(
            [NotNull] this IEquipmentContainer container,
            [NotNull] Equipment item,
            params string[] tags)
        {
            var configuration = FindConfiguration(container, item, tags);

            return configuration == null ? null : Equip(container, item, configuration);
        }

        [NotNull]
        public static Equipment Equip(
            [NotNull] this IEquipmentContainer container,
            [NotNull] Equipment item,
            [NotNull] EquipmentConfiguration configuration)
        {
            Ensure.Any.IsNotNull(container, nameof(container));
            Ensure.Any.IsNotNull(item, nameof(item));
            Ensure.Any.IsNotNull(configuration, nameof(configuration));

            configuration.Activate();
            container.Add(item);

            return item;
        }

        [NotNull]
        public static Equipment Unequip(
            [NotNull] this IEquipmentContainer container,
            [NotNull] Equipment item,
            [CanBeNull] Node dropTo)
        {
            Ensure.Any.IsNotNull(container, nameof(container));
            Ensure.Any.IsNotNull(item, nameof(item));

            var parent = dropTo ?? item.GetTree().CurrentScene;
            var transform = item.GlobalTransform;

            container.Remove(item);

            item.Configuration?.Deactivate();

            parent.AddChild(item);

            item.GlobalTransform = transform;

            return item;
        }

        [CanBeNull]
        public static Equipment Unequip(
            [NotNull] this IEquipmentContainer container,
            [NotNull] string slot,
            [CanBeNull] Node dropTo)
        {
            Ensure.Any.IsNotNull(container, nameof(container));
            Ensure.Any.IsNotNull(slot, nameof(slot));

            var item = container.FindItem(slot);

            return item != null ? Unequip(container, item, dropTo) : null;
        }
    }
}
