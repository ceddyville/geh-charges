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
using GreenEnergyHub.Charges.Domain.Dtos.ChargeCommands.Validation;

namespace GreenEnergyHub.Charges.MessageHub.Models.AvailableChargeReceiptData
{
    /// <summary>
    /// Factory for creating CIM error text of charge rejections.
    /// </summary>
    public interface ICimValidationErrorTextFactory
    {
        /// <summary>
        /// Creates an error text by replacing occurrences of the
        /// placeholder texts with values from the <see cref="ChargeCommand"/>.
        /// </summary>
        string Create(ValidationRuleIdentifier validationRuleIdentifier, ChargeCommand chargeCommand);
    }
}
