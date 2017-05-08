using System;
using System.Runtime.Serialization;

namespace BusinessLayer.BusinessModels.Exeptions
{
    [Serializable]
    public class DatabaseCategoryItemAlreadyExistsException : Exception
    {
        public DatabaseCategoryItemAlreadyExistsException()
        {
        }

        public DatabaseCategoryItemAlreadyExistsException(string message) : base(message)
        {
        }

        public DatabaseCategoryItemAlreadyExistsException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DatabaseCategoryItemAlreadyExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}