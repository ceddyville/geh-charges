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

using System.Threading.Tasks;
using GreenEnergyHub.Charges.Domain.Dtos.Messages.Command;

namespace GreenEnergyHub.Charges.Domain.Dtos.Validation
{
    public class BusinessValidator<TOperation> : IBusinessValidator<TOperation>
        where TOperation : OperationBase
    {
        private readonly IBusinessValidationRulesFactory<TOperation> _businessValidationRulesFactory;

        public BusinessValidator(IBusinessValidationRulesFactory<TOperation> businessValidationRulesFactory)
        {
            _businessValidationRulesFactory = businessValidationRulesFactory;
        }

        public async Task<ValidationResult> ValidateAsync(TOperation operation)
        {
            var businessValidationResult = await _businessValidationRulesFactory
                .CreateRulesAsync(operation).ConfigureAwait(false);
            return businessValidationResult.Validate();
        }
    }
}
