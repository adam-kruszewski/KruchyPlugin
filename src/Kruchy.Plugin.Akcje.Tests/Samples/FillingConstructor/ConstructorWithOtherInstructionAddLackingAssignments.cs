namespace Kruchy.Plugin.Akcje.Tests.Samples.FillingConstructor
{
    internal class ConstructorWithOtherInstructionAddLackingAssignments
    {
        private readonly string _old1;

        private readonly string _new1;

        public ConstructorWithOtherInstructionAddLackingAssignments(
            string old1)
        {
            _old1 = old1;

            _old1 = _old1 + "a";

            if (1 == 2)
            {
                _old1 = "abc";
            }
        }
    }
}
