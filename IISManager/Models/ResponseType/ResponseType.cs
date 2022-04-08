namespace IISManager.Models.ResponseType
{
    public class ResponseType<T>
    {
        public bool IsSuccess { get; set; } = true;
        public T Result { get; set; }
        public ErrorInfo Error { get; set; }
    }
}