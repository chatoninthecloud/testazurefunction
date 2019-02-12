using System;
namespace AzureFunction.Dtos
{
    public class LogException : Exception 
    {
        public LogException(string message) : base(message)
        {
            
        }
    }
}