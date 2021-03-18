namespace ExtensibleCharacterController.Core.Variables
{
    [System.Serializable]
    public class ECCFloatReference : ECCBaseVariableReference<float, Variables.ECCFloat>
    {
        public ECCFloatReference() : base() {}
        public ECCFloatReference(float value) : base(value) {}

        public static implicit operator ECCFloatReference(float value) => new ECCFloatReference(value);
        public static implicit operator float(ECCFloatReference value) => value == null ? new ECCFloatReference().Value : value.Value;

        public static ECCFloatReference operator +(ECCFloatReference first, ECCFloatReference second) => first.Value + second.Value;
        public static ECCFloatReference operator -(ECCFloatReference first, ECCFloatReference second) => first.Value - second.Value;
        public static ECCFloatReference operator *(ECCFloatReference first, ECCFloatReference second) => first.Value * second.Value;
        public static ECCFloatReference operator /(ECCFloatReference first, ECCFloatReference second) => first.Value / second.Value;
    }
}
