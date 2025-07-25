namespace LinkIt.BubbleSheetPortal.Services.CodeGen
{
    public class UniqueTestCodeValidator : ITestCodeValidator
    {
        private readonly QTITestClassAssignmentService _service;

        public UniqueTestCodeValidator(QTITestClassAssignmentService service)
        {
            _service = service;
        }

        public bool Validate(string code)
        {
            var result = !_service.InValidCode(code);
            return result;
        }
    }
}
