using System;
using System.Runtime.Serialization;

namespace BusinessLayer.BusinessModels.Exeptions
{
    [Serializable]
    public class DatabaseCategoryItemNotExistException : Exception
    {
        public DatabaseCategoryItemNotExistException()
        {
        }

        public DatabaseCategoryItemNotExistException(string message) : base(message)
        {
        }

        public DatabaseCategoryItemNotExistException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DatabaseCategoryItemNotExistException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}