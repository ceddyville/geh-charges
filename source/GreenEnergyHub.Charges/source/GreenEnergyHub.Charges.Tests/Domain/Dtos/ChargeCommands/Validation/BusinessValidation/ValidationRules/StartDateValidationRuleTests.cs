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

using System.Linq;
using AutoFixture.Xunit2;
using FluentAssertions;
using GreenEnergyHub.Charges.Core;
using GreenEnergyHub.Charges.Core.DateTime;
using GreenEnergyHub.Charges.Domain.Dtos.ChargeCommands;
using GreenEnergyHub.Charges.Domain.Dtos.ChargeCommands.Validation.BusinessValidation.ValidationRules;
using GreenEnergyHub.Charges.Domain.Dtos.ChargeCommands.Validation.InputValidation.ValidationRules;
using GreenEnergyHub.Charges.Domain.Dtos.Validation;
using GreenEnergyHub.Charges.TestCore;
using GreenEnergyHub.Charges.TestCore.Attributes;
using GreenEnergyHub.Charges.Tests.Builders.Command;
using GreenEnergyHub.Iso8601;
using GreenEnergyHub.TestHelpers;
using Moq;
using NodaTime;
using NodaTime.Testing;
using NodaTime.Text;
using Xunit;
using Xunit.Categories;

namespace GreenEnergyHub.Charges.Tests.Domain.Dtos.ChargeCommands.Validation.BusinessValidation.ValidationRules
{
    [UnitTest]
    public class StartDateValidationRuleTests
    {
        [Theory]
        [InlineAutoMoqData("2020-05-10T13:00:00Z", "2010-05-10T21:59:59Z", "Europe/Copenhagen", false)]
        [InlineAutoMoqData("2020-05-10T13:00:00Z", "2020-05-10T22:00:00Z", "Europe/Copenhagen", true)]
        [InlineAutoMoqData("2020-05-10T13:00:00Z", "2030-05-13T21:59:59Z", "Europe/Copenhagen", false)]
        public void IsValid_WhenStartDateIsWithinInterval_IsTrue(
            string nowIsoString,
            string effectuationDateIsoString,
            string timeZoneId,
            bool expected,
            [Frozen] ChargeOperationDtoBuilder builder)
        {
            // Arrange
            var chargeOperationDto = builder
                .WithStartDateTime(InstantPattern.General.Parse(effectuationDateIsoString).Value)
                .Build();
            var zonedDateTimeService = CreateLocalDateTimeService(timeZoneId);
            var clock = new FakeClock(InstantPattern.General.Parse(nowIsoString).Value);

            // Act (implicit)
            var sut = new StartDateValidationRule(chargeOperationDto.StartDateTime, zonedDateTimeService, clock);

            // Assert
            sut.IsValid.Should().Be(expected);
        }

        [Theory]
        [InlineAutoDomainData]
        public void ValidationRuleIdentifier_ShouldBe_EqualTo(ChargeOperationDtoBuilder builder, IClock clock)
        {
            // Arrange
            var chargeOperationDto = builder.WithStartDateTime(InstantHelper.GetEndDefault()).Build();
            var zonedDateTimeService = CreateLocalDateTimeService("Europe/Copenhagen");

            // Act (implicit)
            var sut = new StartDateValidationRule(chargeOperationDto.StartDateTime, zonedDateTimeService, clock);

            // Assert
            sut.ValidationRuleIdentifier.Should().Be(ValidationRuleIdentifier.StartDateValidation);
        }

        private static ZonedDateTimeService CreateLocalDateTimeService(string timeZoneId)
        {
            var clock = new Mock<IClock>();
            return new ZonedDateTimeService(clock.Object, new Iso8601ConversionConfiguration(timeZoneId));
        }
    }
}
