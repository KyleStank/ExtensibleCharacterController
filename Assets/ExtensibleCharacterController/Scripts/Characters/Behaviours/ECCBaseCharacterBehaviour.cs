using UnityEngine;

namespace ExtensibleCharacterController.Characters.Behaviours
{
    public abstract class ECCBaseCharacterBehaviour
    {
        /// <summary>
        /// Reference to the character that this character behaviour is attached to.
        /// </summary>
        /// <value></value>
        public ECCCharacter Character { get; private set; }

        /// <summary>
        /// Reference to character's transform.
        /// Alias for Character.transform.
        /// </summary>
        public Transform transform
        {
            get => Character.transform;
        }

        /// <summary>
        /// Reference to character's game object.
        /// Alias for Character.gameObject.
        /// </summary>
        /// <value></value>
        public GameObject gameObject
        {
            get => Character.gameObject;
        }

        /// <summary>
        /// Reference to character's rigidbody.
        /// Alias for Character.Rigidbody.
        /// </summary>
        /// <value></value>
        public Rigidbody rigidbody
        {
            get => Character.Rigidbody;
        }

        public ECCBaseCharacterBehaviour(ECCCharacter character)
        {
            Character = character;
        }

        public abstract void Initialize();
        public abstract void Update();
        public abstract void FixedUpdate();
    }
}
