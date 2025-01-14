namespace AAAErp.ViewModel
{
    public class ReturnData<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }
    }

    public class MenuResponse
    {
        public bool Success { get; set; }
    }


    public enum InputDataType
	{
		Default = 0,
		Integer = 1,
		Decimal = 2,
		Float = 3,
		Email = 4,
		Password = 5
	}
}
