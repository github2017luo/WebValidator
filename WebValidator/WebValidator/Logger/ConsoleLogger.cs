using System;
using System.Collections.Generic;
using WebValidator.Validator.Error;

namespace WebValidator.Logger
{
    public class ConsoleLogger : ILogger
    {
        public void LogErrors(List<ErrorDto> errors)
        {
            if (errors.Count == 0)
            {
                Console.WriteLine("No http errors!");
                return;
            }

            foreach (var error in errors)
            {
                Console.WriteLine($"{error.Uri}, code: {error.StatusCode}");
            }
        }
    }
}