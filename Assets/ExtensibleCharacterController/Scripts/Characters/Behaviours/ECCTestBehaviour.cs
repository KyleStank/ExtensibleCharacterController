using UnityEngine;

namespace ExtensibleCharacterController.Characters.Behaviours
{
    public class ECCTestBehaviour : ECCBaseCharacterBehaviour
    {
        public ECCTestBehaviour(ECCCharacter character) : base(character) {}

        public override void Initialize()
        {
            Debug.Log("Test Behaviour Character Name: " + Character.name);
        }
    }
}
