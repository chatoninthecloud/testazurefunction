using System;
namespace AzureFunction.Dtos
{
    public class ReferentialException : Exception 
    {
        public ReferentialException(string message) : base(message)
        {
            
        }
    }
}