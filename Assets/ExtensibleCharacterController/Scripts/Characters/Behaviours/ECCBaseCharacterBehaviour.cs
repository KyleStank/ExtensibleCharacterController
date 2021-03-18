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

        public ECCBaseCharacterBehaviour(ECCCharacter character)
        {
            Character = character;
        }

        public abstract void Initialize();
    }
}
