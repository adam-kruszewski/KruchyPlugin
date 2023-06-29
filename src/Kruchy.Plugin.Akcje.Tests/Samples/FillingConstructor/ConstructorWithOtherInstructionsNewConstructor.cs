namespace Kruchy.Plugin.Akcje.Tests.Samples.FillingConstructor
{
    public class ConstructorWithOtherInstructionsNewConstructor
    {
        private int a1;

        private readonly string dependency1;

        private readonly string _dependency2;

        public ConstructorWithOtherInstructionsNewConstructor()
        {
            this.a1 = 1;

            a1 = 3;
        }
    }
}
