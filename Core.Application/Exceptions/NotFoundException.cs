using Core.Application.Transform;

namespace Core.Application.Exceptions
{
    public class NotFoundException : ApplicationException
    {
        public NotFoundException(string name, object key) : base($"{name} ({key}) was not found")
        {

        }

        public NotFoundException(string key, string value) : base(ValidatorTransform.ValidValue(key, value))
        {

        }

        public NotFoundException(string name) : base(ValidatorTransform.ValidValue(name))
        {

        }
    }
}
