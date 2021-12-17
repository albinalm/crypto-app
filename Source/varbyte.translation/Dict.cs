namespace varbyte.translation
{
    public record Dict
    {
        //Add a public string Name { get; set; }

        public string GetPropertyValue(string input)
        {
            return GetType().GetProperty(input)?.GetValue(this, null)?.ToString();
        }
    }
}