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
using System.Linq;
using System.Threading.Tasks;
using GreenEnergyHub.Charges.Domain.AvailableChargeLinkReceiptData;
using GreenEnergyHub.Charges.Domain.AvailableData;
using GreenEnergyHub.Charges.Domain.Dtos.ChargeLinksAcceptedEvents;

namespace GreenEnergyHub.Charges.Application.ChargeLinks.MessageHub
{
    public class AvailableChargeLinkReceiptDataFactory
        : IAvailableDataFactory<AvailableChargeLinkReceiptData, ChargeLinksAcceptedEvent>
    {
        private readonly IMessageMetaDataContext _messageMetaDataContext;

        public AvailableChargeLinkReceiptDataFactory(
            IMessageMetaDataContext messageMetaDataContext)
        {
            _messageMetaDataContext = messageMetaDataContext;
        }

        public Task<IReadOnlyList<AvailableChargeLinkReceiptData>> CreateAsync(
            ChargeLinksAcceptedEvent acceptedEvent)
        {
            IReadOnlyList<AvailableChargeLinkReceiptData> result = acceptedEvent.ChargeLinksCommand.ChargeLinks.Select(
                    link => new AvailableChargeLinkReceiptData(
                        acceptedEvent.ChargeLinksCommand.Document.Sender.Id, // The sender is now the recipient of the receipt
                        acceptedEvent.ChargeLinksCommand.Document.Sender.BusinessProcessRole,
                        acceptedEvent.ChargeLinksCommand.Document.BusinessReasonCode,
                        _messageMetaDataContext.RequestDataTime,
                        Guid.NewGuid(), // ID of each available piece of data must be unique
                        ReceiptStatus.Confirmed,
                        link.OperationId,
                        acceptedEvent.ChargeLinksCommand.MeteringPointId,
                        new List<AvailableChargeLinkReceiptDataReasonCode>()))
                .ToList();
            return Task.FromResult(result);
        }
    }
}
