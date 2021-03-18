namespace ExtensibleCharacterController.Core.Variables
{
    [System.Serializable]
    public class ECCBoolReference : ECCBaseVariableReference<bool, Variables.ECCBool>
    {
        public ECCBoolReference() : base() {}
        public ECCBoolReference(bool value) : base(value) {}

        public static implicit operator ECCBoolReference(bool value) => new ECCBoolReference(value);
        public static implicit operator bool(ECCBoolReference value) => value == null ? new ECCBoolReference().Value : value.Value;
    }
}
