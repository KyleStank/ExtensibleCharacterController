namespace ExtensibleCharacterController.Core.Variables
{
    [System.Serializable]
    public class ECCIntReference : ECCBaseVariableReference<int, Variables.ECCInt>
    {
        public ECCIntReference() : base() {}
        public ECCIntReference(int value) : base(value) {}

        public static implicit operator ECCIntReference(int value) => new ECCIntReference(value);
        public static implicit operator int(ECCIntReference value) => value == null ? new ECCIntReference().Value : value.Value;

        public static ECCIntReference operator +(ECCIntReference first, ECCIntReference second) => first.Value + second.Value;
        public static ECCIntReference operator -(ECCIntReference first, ECCIntReference second) => first.Value - second.Value;
        public static ECCIntReference operator *(ECCIntReference first, ECCIntReference second) => first.Value * second.Value;
        public static ECCIntReference operator /(ECCIntReference first, ECCIntReference second) => first.Value / second.Value;
    }
}
