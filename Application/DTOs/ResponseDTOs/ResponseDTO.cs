using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.ResponseDTOs
{
    public class ResponseDTO<T>
    {
        public string Message { get; set; }
        public bool IsSuccess { get; set; }
        public T? Result { get; set; }

        public ResponseDTO(string message, bool isSuccess, T? result)
        {
            Message = message;
            IsSuccess = isSuccess;
            Result = result;
        }

        public ResponseDTO(string message, bool isSuccess)
        {
            Message = message;
            IsSuccess = isSuccess;
        }
    }
}
