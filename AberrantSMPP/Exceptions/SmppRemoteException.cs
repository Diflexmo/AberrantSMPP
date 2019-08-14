using System;

using Aberrant.SMPP.Core.Packet;
using Aberrant.SMPP.Core.Packet.Request;
using Aberrant.SMPP.Core.Packet.Response;

namespace Aberrant.SMPP.Core.Exceptions
{
    /// <summary>
    /// Remote party reported an error to our request.
    /// </summary>
    [Serializable]
    public class SmppRemoteException : Exception
    {
        public SmppRequest Request { get; }
        public SmppResponse Response { get; }
        public CommandStatus CommandStatus { get; }

        protected SmppRemoteException()
        {
        }

        protected SmppRemoteException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }

        public SmppRemoteException(string message, CommandStatus status)
            : base(message)
        {
            CommandStatus = status;
        }

        public SmppRemoteException(string message, SmppRequest request, SmppResponse response)
            : base(message)
        {
            Request = request ?? throw new ArgumentNullException("request");
            Response = response ?? throw new ArgumentNullException("response");
            CommandStatus = response.CommandStatus;
        }
    }
}