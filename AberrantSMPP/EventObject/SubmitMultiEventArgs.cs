/* AberrantSMPP: SMPP communication library
 * Copyright (C) 2004, 2005 Christopher M. Bouzek
 * Copyright (C) 2010, 2011 Pablo Ruiz Garc�a <pruiz@crt0.net>
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
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with RoaminSMPP.  If not, see <http://www.gnu.org/licenses/>.
 */

using Aberrant.SMPP.Core.Packet.Request;

namespace Aberrant.SMPP.Core.EventObject
{
    /// <summary>
    /// Class that defines a submit_multi event.  
    /// </summary>
    public class SubmitMultiEventArgs : SmppEventArgs
    {
        /// <summary>
        /// Allows access to the underlying Pdu.
        /// </summary>
        public SmppSubmitMulti SubmitMultiPdu { get; }

        /// <summary>
        /// Creates a SubmitMultiEventArgs.
        /// </summary>
        /// <param name="packet">The PDU that was received.</param>
        internal SubmitMultiEventArgs(SmppSubmitMulti packet) : base(packet)
        {
            SubmitMultiPdu = packet;
        }
    }
}