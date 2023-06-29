namespace Kruchy.Plugin.Akcje.Tests.Samples.FillingConstructor
{
    public class ConstructorWithOtherInstructionsNewConstructor
    {
        private int a1;

        private readonly string dependency1;

        private readonly string _dependency2;

        public ConstructorWithOtherInstructionsNewConstructor(
            string dependency1,
            string dependency2)
        {
            this.dependency1 = dependency1;
            _dependency2 = dependency2;

            this.a1 = 1;

            a1 = 3;
        }

    }
}
