namespace KruchyParserKodu.ParserKodu.Models.Instructions
{
    public class AssignmentInstruction : Instruction
    {
        public string LeftSide { get; set; }

        public string AssignedValue { get; set; }
    }
}
