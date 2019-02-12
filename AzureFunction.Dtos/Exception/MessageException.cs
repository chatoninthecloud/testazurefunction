using System;

namespace AzureFunction.Dtos
{
    public class EventException : Exception
    {
        public EventException(string message) : base(message)
        {
            
        }
    }
}