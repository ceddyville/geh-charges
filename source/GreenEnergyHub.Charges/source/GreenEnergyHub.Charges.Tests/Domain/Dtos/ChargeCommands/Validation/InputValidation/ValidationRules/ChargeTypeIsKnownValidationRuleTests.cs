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

using FluentAssertions;
using GreenEnergyHub.Charges.Domain.Charges;
using GreenEnergyHub.Charges.Domain.Dtos.ChargeCommands.Validation.InputValidation.ValidationRules;
using GreenEnergyHub.Charges.Domain.Dtos.Validation;
using GreenEnergyHub.Charges.TestCore.Attributes;
using GreenEnergyHub.Charges.Tests.Builders.Command;
using GreenEnergyHub.TestHelpers;
using Xunit;
using Xunit.Categories;

namespace GreenEnergyHub.Charges.Tests.Domain.Dtos.ChargeCommands.Validation.InputValidation.ValidationRules
{
    [UnitTest]
    public class ChargeTypeIsKnownValidationRuleTests
    {
        [Theory]
        [InlineAutoMoqData(ChargeType.Unknown, false)]
        [InlineAutoMoqData(ChargeType.Fee, true)]
        [InlineAutoMoqData(ChargeType.Tariff, true)]
        [InlineAutoMoqData(ChargeType.Subscription, true)]
        [InlineAutoMoqData(-1, false)]
        public void ChargeTypeIsKnownValidationRuleTest(
            ChargeType chargeType,
            bool expected,
            ChargeOperationDtoBuilder builder)
        {
            var chargeOperationDto = builder.WithChargeType(chargeType).Build();
            var sut = new ChargeTypeIsKnownValidationRule(chargeOperationDto);
            Assert.Equal(expected, sut.IsValid);
        }

        [Theory]
        [InlineAutoDomainData]
        public void ValidationRuleIdentifier_ShouldBe_EqualTo(ChargeOperationDtoBuilder builder)
        {
            var invalidChargeOperationDto = builder.WithChargeType(ChargeType.Unknown).Build();
            var sut = new ChargeTypeIsKnownValidationRule(invalidChargeOperationDto);
            sut.ValidationRuleIdentifier.Should().Be(ValidationRuleIdentifier.ChargeTypeIsKnownValidation);
        }
    }
}
