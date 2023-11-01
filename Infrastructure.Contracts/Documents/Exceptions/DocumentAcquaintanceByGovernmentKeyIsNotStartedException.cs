using System;

namespace Infrastructure.Contracts.Documents.Exceptions
{
    public class DocumentAcquaintanceByGovernmentKeyIsNotStartedException : Exception
    {
        public DocumentAcquaintanceByGovernmentKeyIsNotStartedException()
            : base("Document acquaintance by Government Key is not started yet")
        {

        }
    }
}
