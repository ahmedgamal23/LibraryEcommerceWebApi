namespace LibraryEcommerceWeb.Shared
{
    public class StateResponse
    {
        public bool IsSuccess { get; set; }
        public string? Reason { get; set; }
        public object? data { get; set; }
    }
}
