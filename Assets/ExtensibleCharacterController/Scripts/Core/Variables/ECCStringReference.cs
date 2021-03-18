namespace ExtensibleCharacterController.Core.Variables
{
    [System.Serializable]
    public class ECCStringReference : ECCBaseVariableReference<string, Variables.ECCString>
    {
        public ECCStringReference() : base() {}
        public ECCStringReference(string value) : base(value) {}

        public static implicit operator ECCStringReference(string value) => new ECCStringReference(value);
        public static implicit operator string(ECCStringReference value) => value == null ? new ECCStringReference().Value : value.Value;

        public static ECCStringReference operator +(ECCStringReference first, ECCStringReference second) => first.Value + second.Value;
    }
}
