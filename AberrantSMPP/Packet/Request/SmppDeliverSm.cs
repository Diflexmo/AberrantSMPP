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
using System.Collections;
using System.Text;

using Aberrant.SMPP.Core.Utility;

namespace Aberrant.SMPP.Core.Packet.Request
{
    /// <summary>
    /// This class defines a deliver_sm that is SMSC generated.  This does
    /// NOT handle anything other than strings in the short message.
    /// </summary>
    public class SmppDeliverSm : SmppRequest
    {
        #region private fields

        private string _ServiceType = string.Empty;
        private string _SourceAddress = string.Empty;
        private string _DestinationAddress = string.Empty;
        private string _ScheduleDeliveryTime = string.Empty;
        private string _ValidityPeriod = string.Empty;

        #endregion private fields

        protected override CommandId DefaultCommandId => CommandId.deliver_sm;

        /// <summary>
        /// Used to indicate the SMS Application service associated with the message.
        /// If this is unknown, null is returned.
        /// </summary>
        public string ServiceType
        {
            get => _ServiceType;

            set => _ServiceType = (value == null) ? string.Empty : value;
        }

        /// <summary>
        /// Type of Number for source address.
        /// </summary>
        public TonType SourceAddressTon { get; set; } = TonType.International;

        /// <summary>
        /// Numbering Plan Indicator for source address.
        /// </summary>
        public NpiType SourceAddressNpi { get; set; } = NpiType.ISDN;

        /// <summary>
        /// Address of origination entity.
        /// </summary>
        public string SourceAddress
        {
            get => _SourceAddress;

            set => _SourceAddress = (value == null) ? string.Empty : value;
        }

        /// <summary>
        /// Type of number of destination entity.
        /// </summary>
        public TonType DestinationAddressTon { get; set; } = TonType.International;

        /// <summary>
        /// Numbering Plan Indicator of destination entity.
        /// </summary>
        public NpiType DestinationAddressNpi { get; set; } = NpiType.ISDN;

        /// <summary>
        /// Destination address of entity.
        /// </summary>
        public string DestinationAddress
        {
            get => _DestinationAddress;

            set => _DestinationAddress = (value == null) ? string.Empty : value;
        }

        /// <summary>
        /// Indicates Message Mode and Message Type.  See the SMSC
        /// version 3.4 specification for details on this.
        /// </summary>
        public byte EsmClass { get; set; } = 0;

        /// <summary>
        /// Protocol Identifier; network specific.
        /// </summary>
        public byte ProtocolId { get; set; } = 0;

        /// <summary>
        /// Designates the priority level of the message.
        /// </summary>
        public PriorityType PriorityFlag { get; set; } = PriorityType.Lowest;

        /// <summary>
        /// Scheduled delivery time for the message delivery.  Set to null for immediate 
        /// delivery.  Otherwise, use YYMMDDhhmmsstnn as the format.  See section 7.1.1 of 
        /// the SMPP spec for more details.
        /// </summary>
        public string ScheduleDeliveryTime
        {
            get => _ScheduleDeliveryTime;
            set
            {
                if (value != null && value != string.Empty)
                {
                    if (value.Length == DATE_TIME_LENGTH)
                    {
                        _ScheduleDeliveryTime = value;
                    }
                    else
                    {
                        throw new ArgumentException("Scheduled delivery time not in correct format.");
                    }
                }
                else
                {
                    _ScheduleDeliveryTime = string.Empty;
                }
            }
        }

        /// <summary>
        /// The validity period of this message.  Set to null to request the SMSC default 
        /// validity period.  Otherwise, use YYMMDDhhmmsstnn as the format.  See section 7.1.1 of 
        /// the SMPP spec for more details.
        /// </summary>
        public string ValidityPeriod
        {
            get => _ValidityPeriod;
            set
            {
                if (value != null && value != string.Empty)
                {
                    if (value.Length == DATE_TIME_LENGTH)
                    {
                        _ValidityPeriod = value;
                    }
                    else
                    {
                        throw new ArgumentException("Validity period not in correct format.");
                    }
                }
                else
                {
                    _ValidityPeriod = string.Empty;
                }
            }
        }

        /// <summary>
        /// Use this to indicate if you want delivery confirmation.
        /// </summary>
        public RegisteredDeliveryType RegisteredDelivery { get; set; } = RegisteredDeliveryType.None;

        /// <summary>
        /// Indicates the encoding scheme of the short message.
        /// </summary>
        public DataCoding DataCoding { get; set; } = DataCoding.SMSCDefault;

        /// <summary>
        /// Short message length in octets(bytes for x86).
        /// </summary>
        public byte SmLength { get; private set; } = 0;

        /// <summary>
        /// The short message for this Pdu.  This holds up to 160 characters.
        /// If the message is longer, the MessagePayload property will be used.
        /// If this is the case, the short message length will be zero.  Note
        /// that both the ShortMessage and MessagePayload cannot be used
        /// simultaneously.
        /// </summary>
        public string ShortMessage { get; set; } = null;

        #region optional parameters

        /// <summary>
        /// The message reference number assigned by the ESME.
        /// </summary>
        public UInt16? UserMessageReference
        {
            get => GetHostOrderUInt16FromTlv(OptionalParamCodes.user_message_reference);

            set => SetHostOrderValueIntoTlv(OptionalParamCodes.user_message_reference, value);
        }

        /// <summary>
        /// The port number associated with the source address of the message.  This
        /// parameter will be present for WAP applications.
        /// </summary>
        public UInt16? SourcePort
        {
            get => GetHostOrderUInt16FromTlv(OptionalParamCodes.source_port);

            set => SetHostOrderValueIntoTlv(OptionalParamCodes.source_port, value);
        }

        /// <summary>
        /// The port number associated with the destination address of the message.  This
        /// parameter will be present for WAP applications.
        /// </summary>
        public UInt16? DestinationPort
        {
            get => GetHostOrderUInt16FromTlv(OptionalParamCodes.destination_port);

            set => SetHostOrderValueIntoTlv(OptionalParamCodes.destination_port, value);
        }

        /// <summary>
        /// The reference number for a particular concatenated short message.
        /// </summary>
        public UInt16? SarMsgRefNumber
        {
            get => GetHostOrderUInt16FromTlv(OptionalParamCodes.sar_msg_ref_num);

            set => SetHostOrderValueIntoTlv(OptionalParamCodes.sar_msg_ref_num, value);
        }

        /// <summary>
        /// Total number of short message fragments within the concatenated short message.
        /// </summary>
        public byte? SarTotalSegments
        {
            get => GetOptionalParamByte(OptionalParamCodes.sar_total_segments);

            set => SetOptionalParamByte(OptionalParamCodes.sar_total_segments, value);
        }

        /// <summary>
        /// The sequence number of a particular short message fragment within the 
        /// concatenated short message.
        /// </summary>
        public byte? SarSegmentSeqnum
        {
            get => GetOptionalParamByte(OptionalParamCodes.sar_segment_seqnum);

            set => SetOptionalParamByte(OptionalParamCodes.sar_segment_seqnum, value);
        }

        /// <summary>
        /// A user response code. The actual response codes are SMS application specific.
        /// </summary>
        public byte? UserResponseCode
        {
            get => GetOptionalParamByte(OptionalParamCodes.user_response_code);

            set => SetOptionalParamByte(OptionalParamCodes.user_response_code, value);
        }

        /// <summary>
        /// Indicates a level of privacy associated with the message.
        /// </summary>
        public PrivacyType? PrivacyIndicator
        {
            get => GetOptionalParamByte<PrivacyType>(OptionalParamCodes.privacy_indicator);

            set => SetOptionalParamByte(OptionalParamCodes.privacy_indicator, value);
        }

        /// <summary>
        /// Defines the type of payload(e.g. WDP, WCMP, etc.)
        /// </summary>
        public PayloadTypeType? PayloadType
        {
            get => GetOptionalParamByte<PayloadTypeType>(OptionalParamCodes.payload_type);

            set => SetOptionalParamByte(OptionalParamCodes.payload_type, value);
        }

        /// <summary>
        /// This can hold up to 64K octets of short message data.
        /// The actual limit is network/SMSC dependent.
        /// </summary>
        public string MessagePayload
        {
            get
            {
                var data = GetOptionalParamBytes(OptionalParamCodes.message_payload);
                return data == null ? null : PduUtil.GetDecodedText(DataCoding, data);
            }

            set => PduUtil.SetMessagePayload(this, DataCoding, value);
        }

        /// <summary>
        /// Associates a callback number with a message.  See section 5.3.2.36 of the
        /// SMPP spec for details.  This must be between 4 and 19 characters in length.
        /// </summary>
        public byte[] CallbackNum
        {
            get => GetOptionalParamBytes(OptionalParamCodes.callback_num);

            set => PduUtil.SetCallbackNum(this, value);
        }

        /// <summary>
        /// Specifies a source subaddress associated with the originating entity.
        /// See section 5.3.2.15 of the SMPP spec for details on setting this parameter.
        /// </summary>
        public byte[] SourceSubaddress
        {
            get => GetOptionalParamBytes(OptionalParamCodes.source_subaddress);

            set => PduUtil.SetSourceSubaddress(this, value);
        }

        /// <summary>
        /// Specifies a source subaddress associated with the receiving entity.
        /// See section 5.3.2.15 of the SMPP spec for details on setting this parameter.
        /// </summary>
        public byte[] DestinationSubaddress
        {
            get => GetOptionalParamBytes(OptionalParamCodes.dest_subaddress);

            set => PduUtil.SetDestSubaddress(this, value);
        }

        /// <summary>
        /// The language of the short message.
        /// </summary>
        public LanguageIndicator? LanguageIndicator
        {
            get => GetOptionalParamByte<LanguageIndicator>(OptionalParamCodes.language_indicator);

            set => SetOptionalParamByte(OptionalParamCodes.language_indicator, value);
        }

        /// <summary>
        /// From the SMPP spec:
        /// The its_session_info parameter is a required parameter for the CDMA Interactive
        /// Teleservice as defined by the Korean PCS carriers [KORITS]. It contains control
        /// information for the interactive session between an MS and an ESME.
        ///
        /// See section 5.3.2.43 of the SMPP spec for how to set this.
        /// </summary>
        public byte[] ItsSessionInfo
        {
            get => GetOptionalParamBytes(OptionalParamCodes.its_session_info);

            set => PduUtil.SetItsSessionInfo(this, value);
        }

        /// <summary>
        /// Network Error Code.  May be present for Intermediate Notifications
        /// and SMSC Delivery Receipts.  See SMPP spec 5.3.2.31 for details.
        /// </summary>
        public byte[] NetworkErrorCode
        {
            get => GetOptionalParamBytes(OptionalParamCodes.network_error_code);

            set => PduUtil.SetNetworkErrorCode(this, value);
        }

        /// <summary>
        /// Indicates to the ESME the final message state for an SMSC Delivery Receipt.
        /// </summary>
        public MessageStateType? MessageState
        {
            get => GetOptionalParamByte<MessageStateType>(OptionalParamCodes.message_state);

            set => SetOptionalParamByte(OptionalParamCodes.message_state, value);
        }

        /// <summary>
        /// Indicates the ID of the message being receipted in an SMSC Delivery Receipt.
        /// </summary>
        public string ReceiptedMessageId
        {
            get => GetOptionalParamString(OptionalParamCodes.receipted_message_id);

            set => PduUtil.SetReceiptedMessageId(this, value);
        }

        /// <summary>
        /// Text(ASCII)giving additional info on the meaning of the response.
        /// </summary>
        public string AdditionalStatusInfoText
        {
            get => GetOptionalParamString(OptionalParamCodes.additional_status_info_text);

            set
            {
                const int MAX_STATUS_LEN = 264;

                if (value == null || value.Length <= MAX_STATUS_LEN)
                {
                    SetOptionalParamString(OptionalParamCodes.additional_status_info_text, value, true);
                }
                else
                {
                    throw new ArgumentException(
                        "additional_status_info_text must have length <= " + MAX_STATUS_LEN);
                }
            }
        }

        /// <summary>
        /// Indicates the reason for delivery failure.
        /// </summary>
        public DeliveryFailureReason? DeliveryFailureReason
        {
            get => GetOptionalParamByte<DeliveryFailureReason>(OptionalParamCodes.delivery_failure_reason);

            set
            {
                if (value.HasValue)
                {
                    SetOptionalParamBytes(OptionalParamCodes.delivery_failure_reason,
                        BitConverter.GetBytes(UnsignedNumConverter.SwapByteOrdering((byte) value)));
                }
                else
                {
                    SetOptionalParamBytes(OptionalParamCodes.delivery_failure_reason, null);
                }
            }
        }

        #endregion optional parameters

        #region constructors

        /// <summary>
        /// Creates a deliver_sm Pdu.
        /// </summary>
        /// <param name="incomingBytes">The bytes received from an ESME.</param>
        public SmppDeliverSm(byte[] incomingBytes) : base(incomingBytes)
        {
        }

        /// <summary>
        /// Creates a deliver_sm Pdu.
        /// </summary>
        public SmppDeliverSm() : base()
        {
        }

        #endregion constructors

        /// <summary>
        /// This decodes the deliver_sm Pdu.  The Pdu has basically the same format as
        /// the submit_sm Pdu, but in this case it is a response.
        /// </summary>
        protected override void DecodeSmscResponse()
        {
            byte[] remainder = BytesAfterHeader;
            ServiceType = SmppStringUtil.GetCStringFromBody(ref remainder);
            SourceAddressTon = (TonType) remainder[0];
            SourceAddressNpi = (NpiType) remainder[1];
            SourceAddress = SmppStringUtil.GetCStringFromBody(ref remainder, 2);
            DestinationAddressTon = (TonType) remainder[0];
            DestinationAddressNpi = (NpiType) remainder[1];
            DestinationAddress = SmppStringUtil.GetCStringFromBody(ref remainder, 2);
            EsmClass = remainder[0];
            ProtocolId = remainder[1];
            PriorityFlag = (PriorityType) remainder[2];
            ScheduleDeliveryTime = SmppStringUtil.GetCStringFromBody(ref remainder, 3);
            ValidityPeriod = SmppStringUtil.GetCStringFromBody(ref remainder);
            RegisteredDelivery = (RegisteredDeliveryType) remainder[0];
            //replace_if_present is always null, so don't bother reading it
            DataCoding = (DataCoding) remainder[2];
            //sm_default_msg_id is always null, so don't bother reading it
            SmLength = remainder[4];
            ShortMessage = SmppStringUtil.GetStringFromBody(ref remainder, 5, 5 + SmLength);
            TranslateTlvDataIntoTable(remainder);
        }

        protected override void AppendPduData(ArrayList pdu)
        {
            pdu.AddRange(SmppStringUtil.ArrayCopyWithNull(Encoding.ASCII.GetBytes(ServiceType)));
            pdu.Add((byte) SourceAddressTon);
            pdu.Add((byte) SourceAddressNpi);
            pdu.AddRange(SmppStringUtil.ArrayCopyWithNull(Encoding.ASCII.GetBytes(SourceAddress)));
            pdu.Add((byte) DestinationAddressTon);
            pdu.Add((byte) DestinationAddressNpi);
            pdu.AddRange(SmppStringUtil.ArrayCopyWithNull(Encoding.ASCII.GetBytes(DestinationAddress)));
            pdu.Add(EsmClass);
            pdu.Add((byte) ProtocolId);
            pdu.Add((byte) PriorityFlag);
            pdu.AddRange(SmppStringUtil.ArrayCopyWithNull(Encoding.ASCII.GetBytes(ScheduleDeliveryTime)));
            pdu.AddRange(SmppStringUtil.ArrayCopyWithNull(Encoding.ASCII.GetBytes(ValidityPeriod)));
            pdu.Add((byte) RegisteredDelivery);
            //replace_if_present is always null, so set it to zero
            pdu.Add((byte) 0);
            pdu.Add((byte) DataCoding);
            //sm_default_msg_id is always null, so set it to zero
            pdu.Add((byte) 0);
            SmLength = PduUtil.InsertShortMessage(pdu, DataCoding, ShortMessage);
        }
    }
}