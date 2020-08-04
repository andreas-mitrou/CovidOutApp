using System;

namespace CovidOut.Web.ServiceLayer.Verification
{
    public interface IVerification
    {
       void Verify(Guid ApplicatonId);
    }
}