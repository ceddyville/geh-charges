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
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentAssertions;
using GreenEnergyHub.Charges.Application.Messaging;
using GreenEnergyHub.Charges.Domain.Dtos.ChargeCommandAcceptedEvents;
using GreenEnergyHub.Charges.Domain.Dtos.SharedDtos;
using GreenEnergyHub.Charges.Domain.MarketParticipants;
using GreenEnergyHub.Charges.Infrastructure.Core.Cim.MarketDocument;
using GreenEnergyHub.Charges.MessageHub.Models.AvailableChargeReceiptData;
using GreenEnergyHub.Charges.TestCore.Attributes;
using GreenEnergyHub.Charges.Tests.Builders.Testables;
using Moq;
using NodaTime;
using Xunit;
using Xunit.Categories;

namespace GreenEnergyHub.Charges.Tests.MessageHub.Models.AvailableChargeReceiptData
{
    [UnitTest]
    public class AvailableChargeConfirmationDataFactoryTests
    {
        [Theory]
        [InlineAutoMoqData]
        public async Task CreateAsync_WhenCalledWithAcceptedEvent_ReturnsAvailableData(
            TestMeteringPointAdministrator meteringPointAdministrator,
            [Frozen] Mock<IMarketParticipantRepository> marketParticipantRepository,
            [Frozen] Mock<IMessageMetaDataContext> messageMetaDataContext,
            ChargeCommandAcceptedEvent acceptedEvent,
            Instant now,
            AvailableChargeReceiptDataFactory sut)
        {
            // Arrange
            messageMetaDataContext.Setup(m => m.RequestDataTime).Returns(now);
            marketParticipantRepository
                .Setup(r => r.GetMeteringPointAdministratorAsync())
                .ReturnsAsync(meteringPointAdministrator);

            // Act
            var actualList = await sut.CreateAsync(acceptedEvent);

            // Assert
            actualList.Should().HaveCount(3);
            actualList[0].RecipientId.Should().Be(acceptedEvent.Command.Document.Sender.MarketParticipantId);
            actualList[0].RecipientRole.Should()
                    .Be(acceptedEvent.Command.Document.Sender.BusinessProcessRole);
            actualList[0].BusinessReasonCode.Should()
                    .Be(acceptedEvent.Command.Document.BusinessReasonCode);
            actualList[0].RequestDateTime.Should().Be(now);
            actualList[0].ReceiptStatus.Should().Be(ReceiptStatus.Confirmed);
            actualList[0].DocumentType.Should().Be(DocumentType.ConfirmRequestChangeOfPriceList);
            actualList[0].OriginalOperationId.Should().Be(acceptedEvent.Command.ChargeOperations.First().Id);
            actualList[0].ValidationErrors.Should().BeEmpty();
            var expectedList = actualList.OrderBy(x => x.OperationOrder);
            actualList.SequenceEqual(expectedList).Should().BeTrue();
        }
    }
}
