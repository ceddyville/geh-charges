﻿// Copyright 2020 Energinet DataHub A/S
//
// Licensed under the Apache License, Version 2.0 (the "License2");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.Diagnostics.CodeAnalysis;
using Energinet.DataHub.Core.Messaging.Protobuf;
using GreenEnergyHub.Charges.Core.DateTime;
using GreenEnergyHub.Charges.Domain.Dtos.ChargeLinksAcceptedEvents;
using GreenEnergyHub.Charges.Domain.Dtos.ChargeLinksCommands;
using GreenEnergyHub.Charges.Domain.Dtos.SharedDtos;

namespace GreenEnergyHub.Charges.Infrastructure.Internal.Mappers
{
    public class LinkCommandAcceptedOutboundMapper : ProtobufOutboundMapper<ChargeLinksAcceptedEvent>
    {
        protected override Google.Protobuf.IMessage Convert([NotNull]ChargeLinksAcceptedEvent chargeLinksAcceptedEvent)
        {
            var chargeLinkCommandAcceptedContract = new GreenEnergyHub.Charges.Infrastructure.Internal.ChargeLinkCommandAccepted.ChargeLinkCommandAccepted
            {
                PublishedTime = chargeLinksAcceptedEvent.PublishedTime.ToTimestamp(),
                ChargeLinksCommand = new ChargeLinkCommandAccepted.ChargeLinkCommand
                {
                    MeteringPointId = chargeLinksAcceptedEvent.ChargeLinksCommand.MeteringPointId,
                    Document = ConvertDocument(chargeLinksAcceptedEvent.ChargeLinksCommand.Document),
                },
            };

            foreach (var chargeLinkDto in chargeLinksAcceptedEvent.ChargeLinksCommand.ChargeLinks)
            {
             chargeLinkCommandAcceptedContract.ChargeLinksCommand.ChargeLinks.Add(ConvertChargeLink(chargeLinkDto));
            }

            return chargeLinkCommandAcceptedContract;
        }

        private static ChargeLinkCommandAccepted.Document ConvertDocument(DocumentDto documentDto)
        {
            return new GreenEnergyHub.Charges.Infrastructure.Internal.ChargeLinkCommandAccepted.Document
            {
                Id = documentDto.Id,
                RequestDate = documentDto.RequestDate.ToTimestamp(),
                Type = (ChargeLinkCommandAccepted.DocumentType)documentDto.Type,
                CreatedDateTime = documentDto.CreatedDateTime.ToTimestamp(),
                Sender = new ChargeLinkCommandAccepted.MarketParticipant
                {
                    Id = documentDto.Sender.Id,
                    BusinessProcessRole = (ChargeLinkCommandAccepted.MarketParticipantRole)documentDto.Sender.BusinessProcessRole,
                },
                Recipient = new ChargeLinkCommandAccepted.MarketParticipant
                {
                    Id = documentDto.Recipient.Id,
                    BusinessProcessRole = (ChargeLinkCommandAccepted.MarketParticipantRole)documentDto.Recipient.BusinessProcessRole,
                },
                IndustryClassification = (ChargeLinkCommandAccepted.IndustryClassification)documentDto.IndustryClassification,
                BusinessReasonCode = (ChargeLinkCommandAccepted.BusinessReasonCode)documentDto.BusinessReasonCode,
            };
        }

        private static ChargeLinkCommandAccepted.ChargeLink ConvertChargeLink(ChargeLinkDto chargeLink)
        {
            return new GreenEnergyHub.Charges.Infrastructure.Internal.ChargeLinkCommandAccepted.ChargeLink
            {
                OperationId = chargeLink.OperationId,
                SenderProvidedChargeId = chargeLink.SenderProvidedChargeId,
                ChargeOwner = chargeLink.ChargeOwner,
                Factor = chargeLink.Factor,
                ChargeType = (ChargeLinkCommandAccepted.ChargeType)chargeLink.ChargeType,
                StartDateTime = chargeLink.StartDateTime.ToTimestamp(),
                EndDateTime = chargeLink.EndDateTime.TimeOrEndDefault().ToTimestamp(),
            };
        }
    }
}
