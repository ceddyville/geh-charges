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

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GreenEnergyHub.Charges.Application.Messaging;
using GreenEnergyHub.Charges.Domain.Dtos.ChargeCommandAcceptedEvents;
using GreenEnergyHub.Charges.Domain.Dtos.ChargeCommands;
using GreenEnergyHub.Charges.Domain.Dtos.SharedDtos;
using GreenEnergyHub.Charges.Domain.MarketParticipants;
using GreenEnergyHub.Charges.Infrastructure.Core.Cim.MarketDocument;
using GreenEnergyHub.Charges.MessageHub.Models.AvailableData;

namespace GreenEnergyHub.Charges.MessageHub.Models.AvailableChargeReceiptData
{
    public class AvailableChargeReceiptDataFactory
        : AvailableDataFactoryBase<AvailableChargeReceiptData, ChargeCommandAcceptedEvent>
    {
        private readonly IMessageMetaDataContext _messageMetaDataContext;

        public AvailableChargeReceiptDataFactory(
            IMessageMetaDataContext messageMetaDataContext,
            IMarketParticipantRepository marketParticipantRepository)
            : base(marketParticipantRepository)
        {
            _messageMetaDataContext = messageMetaDataContext;
        }

        public override async Task<IReadOnlyList<AvailableChargeReceiptData>> CreateAsync(ChargeCommandAcceptedEvent input)
        {
            var recipient = input.Command.Document.Sender; // The original sender is the recipient of the receipt
            var sender = await GetSenderAsync().ConfigureAwait(false);

            var availableChargeReceiptData = new List<AvailableChargeReceiptData>();

            var operationOrder = 0;
            foreach (var chargeOperationDto in input.Command.ChargeOperations)
            {
                availableChargeReceiptData.AddRange(CreateAvailableChargeReceiptData(
                    input.Command.Document, chargeOperationDto, sender, recipient, operationOrder++));
            }

            return availableChargeReceiptData;
        }

        private IReadOnlyList<AvailableChargeReceiptData> CreateAvailableChargeReceiptData(
            DocumentDto documentDto,
            ChargeOperationDto chargeOperationDto,
            MarketParticipant sender,
            MarketParticipantDto recipient,
            int operationOrder)
        {
            return new List<AvailableChargeReceiptData>()
            {
                new AvailableChargeReceiptData(
                    sender.MarketParticipantId,
                    sender.BusinessProcessRole,
                    recipient.MarketParticipantId,
                    recipient.BusinessProcessRole,
                    documentDto.BusinessReasonCode,
                    _messageMetaDataContext.RequestDataTime,
                    Guid.NewGuid(), // ID of each available piece of data must be unique
                    ReceiptStatus.Confirmed,
                    chargeOperationDto.Id,
                    DocumentType.ConfirmRequestChangeOfPriceList, // Will be added to the HTTP MessageType header
                    operationOrder,
                    recipient.ActorId,
                    new List<AvailableReceiptValidationError>()),
            };
        }
    }
}
