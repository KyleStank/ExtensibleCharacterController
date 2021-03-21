using UnityEngine;

namespace ExtensibleCharacterController.Characters.Behaviours
{
    public class ECCMovementBehaviour : ECCBaseCharacterBehaviour
    {
        float horizontal;
        float vertical;
        Vector3 input;
        bool canMove = true;

        public ECCMovementBehaviour(ECCCharacter character) : base(character) {}

        public override void Initialize()
        {

        }

        public override void Update()
        {
            horizontal = Input.GetAxis("Horizontal");
            vertical = Input.GetAxis("Vertical");
            input = new Vector3(horizontal, 0.0f, vertical);

            // // TODO: Random collision testing. Can be removed.
            // https://roundwide.com/physics-overlap-capsule/
            // Vector3 pos = transform.position;
            // pos.y += 1.0f;
            // Collider[] colliders = Physics.OverlapSphere(pos, 1.0f);
            // for (int i = 0; i < colliders.Length; i++)
            // {
            //     Collider collider = colliders[i];
            //     if (collider.name == "Body" || collider.name == "Plane") continue;

            //     Debug.Log(collider.name);
            //     canMove = false;
            // }
        }

        public override void FixedUpdate()
        {
            // if (!canMove) return;

            Vector3 pos = rigidbody.position;
            pos += input * 10.0f * Time.deltaTime;
            rigidbody.MovePosition(pos);
        }
    }
}
