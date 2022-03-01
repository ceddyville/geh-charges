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
using GreenEnergyHub.Charges.Domain.Dtos.ChargeCommands;
using GreenEnergyHub.Charges.Domain.Dtos.ChargeCommands.Validation.InputValidation.ValidationRules;
using GreenEnergyHub.Charges.Domain.Dtos.Validation;
using GreenEnergyHub.Charges.TestCore.Attributes;
using GreenEnergyHub.Charges.Tests.Builders;
using GreenEnergyHub.Charges.Tests.Builders.Command;
using GreenEnergyHub.TestHelpers;
using Xunit;
using Xunit.Categories;

namespace GreenEnergyHub.Charges.Tests.Domain.Dtos.ChargeCommands.Validation.InputValidation.ValidationRules
{
    [UnitTest]
    public class ChargeDescriptionHasMaximumLengthRuleTests
    {
        private const int ChargeDescriptionMaximumLength = 2048;

        [Theory]
        [InlineAutoMoqData(ChargeDescriptionMaximumLength - 1, true)]
        [InlineAutoMoqData(ChargeDescriptionMaximumLength, true)]
        [InlineAutoMoqData(ChargeDescriptionMaximumLength + 1, false)]
        public void ChargeDescriptionHasMaximumLengthRule_WhenDescriptionTooLong_IsFalse(
            int chargeDescriptionLength,
            bool expected,
            ChargeCommandBuilder builder)
        {
            var command = builder.WithDescription(GenerateStringWithLength(chargeDescriptionLength)).Build();
            var sut = new ChargeDescriptionHasMaximumLengthRule(command);
            sut.IsValid.Should().Be(expected);
        }

        [Theory]
        [InlineAutoDomainData]
        public void ValidationRuleIdentifier_ShouldBe_EqualTo(ChargeCommandBuilder builder)
        {
            var invalidCommand = CreateInvalidCommand(builder);
            var sut = new ChargeDescriptionHasMaximumLengthRule(invalidCommand);
            sut.ValidationRuleIdentifier.Should().Be(ValidationRuleIdentifier.ChargeDescriptionHasMaximumLength);
        }

        private static string GenerateStringWithLength(int stringLength)
        {
            return new string('a', stringLength);
        }

        private static ChargeCommand CreateInvalidCommand(ChargeCommandBuilder builder)
        {
            var toLongDescription = new string('x', ChargeDescriptionMaximumLength + 1);
            return builder.WithDescription(toLongDescription).Build();
        }
    }
}
