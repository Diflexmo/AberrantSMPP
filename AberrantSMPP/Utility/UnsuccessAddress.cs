/* AberrantSMPP: SMPP communication library
 * Copyright (C) 2004, 2005 Christopher M. Bouzek
 * Copyright (C) 2010, 2011 Pablo Ruiz García <pruiz@crt0.net>
 *
 * This file is part of RoaminSMPP.
 *
 * RoaminSMPP is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation, version 3 of the License.
 *
 * RoaminSMPP is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lessert General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with RoaminSMPP.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;

using Aberrant.SMPP.Core.Packet;

namespace Aberrant.SMPP.Core.Utility
{
    /// <summary>
    /// Defines an Unsuccess address, used with the response to submit_multi.
    /// The spec states that both SME addresses and distribution lists can be
    /// used here, but it only defines for the SME address, so that is all that
    /// this will handle.
    /// </summary>
    public class UnsuccessAddress
    {
        #region properties

        /// <summary>
        /// Type of number for destination SME.
        /// </summary>
        public Pdu.TonType DestinationAddressTon { get; }

        /// <summary>
        /// Numbering Plan Indicator for destination SME
        /// </summary>
        public Pdu.NpiType DestinationAddressNpi { get; }

        /// <summary>
        /// Destination Address of destination SME
        /// </summary>
        public string DestinationAddress { get; }

        /// <summary>
        /// Indicates the success or failure of the submit_multi request to this
        /// SME address.
        /// </summary>
        public UInt32 ErrorStatusCode { get; }

        #endregion properties

        /// <summary>
        /// Creates an Unsuccess address.  This will trim down the address given to
        /// it for use in future operations.
        /// </summary>
        /// <param name="address">The bytes of the response.</param>
        public UnsuccessAddress(ref byte[] address)
        {
            DestinationAddressTon = (Pdu.TonType) address[0];
            DestinationAddressNpi = (Pdu.NpiType) address[1];
            DestinationAddress = SmppStringUtil.GetCStringFromBody(ref address, 2);
            //convert error status to host order
            ErrorStatusCode = UnsignedNumConverter.SwapByteOrdering(
                BitConverter.ToUInt32(address, 0));
            //now we have to trim off four octets to account for the status code
            long length = address.Length - 4;
            byte[] newRemainder = new byte[length];
            Array.Copy(address, 4, newRemainder, 0, length);
            //and change the reference
            address = newRemainder;
            newRemainder = null;
        }

        /// <summary>
        /// Creates a new UnsuccessAdress.
        /// </summary>
        /// <param name="destinationAddressTon">Type of number for destination SME.</param>
        /// <param name="destinationAddressNpi">Numbering Plan Indicator for destination SME</param>
        /// <param name="destinationAdress">Destination Address of destination SME</param>
        /// <param name="ErrorStatusCode">Indicates the success or failure of the submit_multi request 
        /// to this SME address.</param>
        public UnsuccessAddress(
            Pdu.TonType destinationAddressTon,
            Pdu.NpiType destinationAddressNpi,
            string destinationAdress,
            UInt32 ErrorStatusCode)
        {
            DestinationAddressTon = destinationAddressTon;
            DestinationAddressNpi = destinationAddressNpi;
            DestinationAddress = destinationAdress;
            this.ErrorStatusCode = ErrorStatusCode;
        }

        /// <summary>
        /// Clones this UnsuccessAddress.
        /// </summary>
        /// <returns>The cloned object.</returns>
        public object Clone()
        {
            UnsuccessAddress temp = new UnsuccessAddress(
                DestinationAddressTon, DestinationAddressNpi, DestinationAddress, ErrorStatusCode);
            return temp;
        }

        /// <summary>
        /// Checks to see if two UnsuccessAddresses are equal.
        /// </summary>
        /// <param name="obj">The UnsuccessAddresses to check</param>
        /// <returns>true if obj and this are equal</returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (GetType() != obj.GetType())
            {
                return false;
            }

            // safe because of the GetType check
            UnsuccessAddress us = (UnsuccessAddress) obj;

            // value member check
            return
                DestinationAddressTon.Equals(us.DestinationAddressTon) &&
                DestinationAddressNpi.Equals(us.DestinationAddressNpi) &&
                DestinationAddress.Equals(us.DestinationAddress) &&
                ErrorStatusCode.Equals(us.ErrorStatusCode);
        }

        /// <summary>
        /// Gets the hash code for this object.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            int hashCode = 0;
            hashCode ^= DestinationAddress.GetHashCode();
            hashCode ^= DestinationAddressNpi.GetHashCode();
            hashCode ^= DestinationAddressTon.GetHashCode();
            hashCode ^= ErrorStatusCode.GetHashCode();

            return hashCode;
        }
    }
}