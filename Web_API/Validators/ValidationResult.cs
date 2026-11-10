namespace Web_API.Validators
{
    public class ValidationResult
    {
        public bool IsValid { get; set; } = true;
        public List<string> Errors { get; set; } = new();
        public string Message => string.Join("; ", Errors);
    }
}
