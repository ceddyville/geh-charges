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
using GreenEnergyHub.Charges.Domain.Dtos.SharedDtos;
using GreenEnergyHub.Charges.Infrastructure.MarketParticipantRegistry.MarketParticipantsSynchronization;
using GreenEnergyHub.Charges.Infrastructure.MarketParticipantRegistry.Persistence.Actors;
using GreenEnergyHub.Charges.TestCore.Attributes;
using GreenEnergyHub.Charges.Tests.Builders.Command;
using Xunit;

namespace GreenEnergyHub.Charges.Tests.Infrastructure.MarketParticipantRegistry
{
    public class MarketParticipantUpdaterTests
    {
        [Theory]
        [InlineAutoMoqData]
        public void Update_SetsActive(MarketParticipantBuilder marketParticipantBuilder, Actor actor)
        {
            var anyValidRole = MarketParticipantRole.SystemOperator;
            var marketParticipant = marketParticipantBuilder
                .WithIsActive(!actor.Active)
                .WithRole(MarketParticipantRole.EnergySupplier)
                .WithMarketParticipantId(actor.IdentificationNumber)
                .Build();

            MarketParticipantUpdater.Update(marketParticipant, actor, anyValidRole);

            marketParticipant.IsActive.Should().Be(actor.Active);
        }

        [Theory]
        [InlineAutoMoqData]
        public void Update_SetsRole(MarketParticipantBuilder marketParticipantBuilder, Actor actor)
        {
            var expectedRole = MarketParticipantRole.SystemOperator;
            var marketParticipant = marketParticipantBuilder
                .WithMarketParticipantId(actor.IdentificationNumber)
                .Build();

            MarketParticipantUpdater.Update(marketParticipant, actor, expectedRole);

            marketParticipant.BusinessProcessRole.Should().Be(expectedRole);
        }

        /// <summary>
        /// This is not a valid business operation. We however need to support it for now as the temporary
        /// market participant registry exhibits this kind of updates to actors.
        /// </summary>
        [Theory]
        [InlineAutoMoqData]
        public void Update_SetsMarketParticipantId(MarketParticipantBuilder marketParticipantBuilder, Actor actor)
        {
            var anyValidRole = MarketParticipantRole.SystemOperator;
            var marketParticipant = marketParticipantBuilder.Build();

            MarketParticipantUpdater.Update(marketParticipant, actor, anyValidRole);

            marketParticipant.MarketParticipantId.Should().Be(actor.IdentificationNumber);
        }
    }
}
