using UnityEngine;

namespace ExtensibleCharacterController.Core.Variables
{
    [System.Serializable]
    public class ECCVector2Reference : ECCBaseVariableReference<Vector2, Variables.ECCVector2>
    {
        public ECCVector2Reference() : base() {}
        public ECCVector2Reference(Vector2 value) : base(value) {}

        public static implicit operator ECCVector2Reference(Vector2 value) => new ECCVector2Reference(value);
        public static implicit operator Vector2(ECCVector2Reference value) => value == null ? new ECCVector2Reference().Value : value.Value;

        public static ECCVector2Reference operator +(ECCVector2Reference first, ECCVector2Reference second) => first.Value + second.Value;
        public static ECCVector2Reference operator -(ECCVector2Reference first, ECCVector2Reference second) => first.Value - second.Value;
        public static ECCVector2Reference operator *(ECCVector2Reference first, ECCVector2Reference second) => first.Value * second.Value;
        public static ECCVector2Reference operator /(ECCVector2Reference first, ECCVector2Reference second) => first.Value / second.Value;
    }
}
