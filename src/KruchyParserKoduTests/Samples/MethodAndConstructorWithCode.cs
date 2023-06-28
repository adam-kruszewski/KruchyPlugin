namespace KruchyParserKoduTests.Samples
{
    internal class MethodAndConstructorWithCode
    {
        private readonly string _a1;

        public MethodAndConstructorWithCode(
            string a1)
        {
            _a1 = a1;

            var v1 = 1 + 2;
        }

        private void Method1()
        {
            var v2 = _a1;

            if (1 == 2)
            {
                var v3 = _a1;
            }
        }
    }
}
