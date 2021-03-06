﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using AlleyCat.Action;
using AlleyCat.Autowire;
using AlleyCat.Character;
using AlleyCat.Common;
using Godot;
using JetBrains.Annotations;
using static AlleyCat.Item.CommonEquipmentTags;

namespace AlleyCat.Item
{
    public class PickupAction : Action.Action
    {
        [Ancestor]
        public Equipment Item { get; private set; }

        [Export(PropertyHint.ExpRange, "0.1, 5")]
        public float PickupDistance { get; set; } = 1.2f;

        [Export]
        public Godot.Animation Animation { get; set; }

        public IEnumerable<string> Tags => _tags.TrimToEnumerable();

        [Export, UsedImplicitly] private string _tags = string.Join(",", Carry, Hand);

        protected override void DoExecute(IActor actor)
        {
            var character = (ICharacter) actor;

            var configuration = character.FindEquipConfiguration(Item, Tags.ToArray());

            if (configuration == null) return;

            if (Animation == null)
            {
                character.Equip(Item, configuration);

                return;
            }

            var animator = character.AnimationManager;

            animator.OnAnimationEvent
                .Where(e => e.Name == "action." + Key)
                .Take(1)
                .Subscribe(_ => character.Equip(Item, configuration))
                .AddTo(this);

            animator.Play(Animation);
        }

        public override bool AllowedFor(IActor context) =>
            !Item.Equipped &&
            context is ICharacter character &&
            character.DistanceTo(Item) <= PickupDistance;
    }
}
