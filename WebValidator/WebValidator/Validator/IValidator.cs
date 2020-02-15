using System.Collections.Generic;

namespace WebValidator.Validator
{
    public interface IValidator
    {
        void Validate(IEnumerable<Node> nodes);
    }
}