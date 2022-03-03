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
using GreenEnergyHub.Charges.Domain.Dtos.Validation;

namespace GreenEnergyHub.Charges.Domain.Dtos.ChargeLinksCommands.Validation.InputValidation.Factories
{
    public class ChargeLinksCommandInputValidationRulesFactory
        : IInputValidationRulesFactory<ChargeLinksCommand, ChargeLinkDto>
    {
        public IValidationRuleSet CreateRulesForCommand(ChargeLinksCommand chargeLinksCommand)
        {
            if (chargeLinksCommand == null) throw new ArgumentNullException(nameof(chargeLinksCommand));

            var rules = GetRulesForCommand();

            return ValidationRuleSet.FromRules(rules);
        }

        public IValidationRuleSet CreateRulesForOperation(ChargeLinkDto chargeOperationDto)
        {
            if (chargeOperationDto == null) throw new ArgumentNullException(nameof(chargeOperationDto));

            var rules = GetRulesForOperation();

            return ValidationRuleSet.FromRules(rules);
        }

        private static List<IValidationRule> GetRulesForCommand()
        {
            // No input validation active yet for Charge Links Command
            var rules = new List<IValidationRule>();

            return rules;
        }

        private static List<IValidationRule> GetRulesForOperation()
        {
            // No input validation active yet for Charge Link Operation
            var rules = new List<IValidationRule>();

            return rules;
        }
    }
}
