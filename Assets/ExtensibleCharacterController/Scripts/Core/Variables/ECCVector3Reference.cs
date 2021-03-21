using UnityEngine;

namespace ExtensibleCharacterController.Core.Variables
{
    [System.Serializable]
    public class ECCVector3Reference : ECCBaseVariableReference<Vector3, Variables.ECCVector3>
    {
        public ECCVector3Reference() : base() {}
        public ECCVector3Reference(Vector3 value) : base(value) {}

        public static implicit operator ECCVector3Reference(Vector3 value) => new ECCVector3Reference(value);
        public static implicit operator Vector3(ECCVector3Reference value) => value == null ? new ECCVector3Reference().Value : value.Value;

        public static ECCVector3Reference operator +(ECCVector3Reference first, ECCVector3Reference second) => first.Value + second.Value;
        public static ECCVector3Reference operator +(Vector3 first, ECCVector3Reference second) => first + second.Value;
        public static ECCVector3Reference operator -(ECCVector3Reference first, ECCVector3Reference second) => first.Value - second.Value;
        public static ECCVector3Reference operator -(Vector3 first, ECCVector3Reference second) => first - second.Value;
    }
}
