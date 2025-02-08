using System.Text.Json.Serialization;

namespace IISManager.Cli.Models.ResponseType;

public class ResponseType<T>
{
    public bool IsSuccess { get; set; } = true;
    public T Result { get; set; }
    public ErrorInfo Error { get; set; }
}