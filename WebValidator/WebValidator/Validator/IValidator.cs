using System;
using System.Collections.Generic;

namespace WebValidator.Validator
{
    public interface IValidator
    {
        List<Tuple<int, Uri>> ValidateUrls();
    }
}