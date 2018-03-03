using System.Diagnostics;
using AlleyCat.Autowire;
using AlleyCat.Common;
using Godot;
using JetBrains.Annotations;

namespace AlleyCat.Locomotion
{
    [Singleton(typeof(ILocomotion))]
    public abstract class Locomotion<T> : AutowiredNode, ILocomotion where T : Spatial
    {
        [Export]
        public bool Active { get; set; } = true;

        [CanBeNull, Node]
        public T Target { get; private set; }

        public Vector3 Velocity { get; private set; }

        public Vector3 RotationalVelocity { get; private set; }

        protected virtual ProcessMode ProcessMode { get; } = ProcessMode.Idle;

        [Export, UsedImplicitly] private NodePath _target = "..";

        private Vector3 _requestedMovement;

        private Vector3 _requestedRotation;

        [PostConstruct]
        protected virtual void OnInitialize()
        {
            _requestedMovement = new Vector3();
            _requestedRotation = new Vector3();
        }

        public override void _Process(float delta)
        {
            base._Process(delta);

            if (Active && ProcessMode == ProcessMode.Idle)
            {
                HandleProcess(delta);
            }
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);

            if (Active && ProcessMode == ProcessMode.Physics)
            {
                HandleProcess(delta);
            }
        }

        public void Move(Vector3 velocity) => _requestedMovement = velocity;

        public void Rotate(Vector3 velocity) => _requestedRotation = velocity;

        protected abstract void Process(float delta, Vector3 velocity, Vector3 rotationalVelocity);

        private void HandleProcess(float delta)
        {
            var target = Target;

            Debug.Assert(target != null, "Target != null");

            var before = target.GlobalTransform;

            Process(delta, _requestedMovement, _requestedRotation);

            var after = target.GlobalTransform;

            if (delta > 0)
            {
                Velocity = (target.ToLocal(after.origin) - target.ToLocal(before.origin)) / delta;
                RotationalVelocity = (before.basis.Inverse() * after.basis).GetEuler() / delta;
            }
        }
    }
}