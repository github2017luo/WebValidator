using System;
using System.Collections.Generic;
using WebValidator.Validator.Error;

namespace WebValidator.Validator
{
    public interface IValidator
    {
        List<ErrorDto> ValidateUrls(ICollection<string> urls);
        //List<ErrorDto> ValidateImages();
    }
}