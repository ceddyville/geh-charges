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

using GreenEnergyHub.Charges.Domain.Dtos.ChargeCommands;
using GreenEnergyHub.Charges.Domain.Dtos.ChargeLinksCommands;
using GreenEnergyHub.Charges.Domain.Dtos.Validation;
using GreenEnergyHub.Charges.MessageHub.Models.AvailableData;
using GreenEnergyHub.Charges.MessageHub.Models.Shared;

namespace GreenEnergyHub.Charges.MessageHub.Models.AvailableChargeLinksReceiptData
{
    public class AvailableChargeLinksReceiptValidationErrorFactory : IAvailableChargeLinksReceiptValidationErrorFactory
    {
        private readonly ICimValidationErrorCodeFactory _cimValidationErrorCodeFactory;
        private readonly ICimValidationErrorTextFactory<ChargeLinksCommand, ChargeLinkDto> _cimValidationErrorTextFactory;

        public AvailableChargeLinksReceiptValidationErrorFactory(
            ICimValidationErrorCodeFactory cimValidationErrorCodeFactory,
            ICimValidationErrorTextFactory<ChargeLinksCommand, ChargeLinkDto> cimValidationErrorTextFactory)
        {
            _cimValidationErrorCodeFactory = cimValidationErrorCodeFactory;
            _cimValidationErrorTextFactory = cimValidationErrorTextFactory;
        }

        public AvailableReceiptValidationError Create(
            IValidationError validationError,
            ChargeLinksCommand command,
            ChargeLinkDto chargeLinkDto)
        {
            var reasonCode = _cimValidationErrorCodeFactory.Create(validationError.ValidationRule.ValidationRuleIdentifier);
            var reasonText = _cimValidationErrorTextFactory.Create(validationError, command, chargeLinkDto);

            return new AvailableReceiptValidationError(reasonCode, reasonText);
        }
    }
}
