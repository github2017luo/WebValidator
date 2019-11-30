using System.Collections.Generic;
using WebValidator.Validator.Error;

namespace WebValidator.Logger
{
    public interface ILogger
    {
        void LogErrors(List<ErrorDto> errors);
    }
}