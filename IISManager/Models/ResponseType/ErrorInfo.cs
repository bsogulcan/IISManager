using System;

namespace IISManager.Models.ResponseType
{
    public class ErrorInfo
    {
        public ErrorInfo(Exception exception)
        {
            this.Message = exception.Message;
        }

        public int ErrorCode { get; set; }
        public string Message { get; set; }
    }
}