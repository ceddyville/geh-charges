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
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Energinet.DataHub.Core.FunctionApp.TestCommon;
using Energinet.DataHub.Core.FunctionApp.TestCommon.ServiceBus.ListenerMock;
using FluentAssertions;
using GreenEnergyHub.Charges.Infrastructure.Integration.ChargeCreated;
using GreenEnergyHub.Charges.IntegrationTests.Fixtures;
using GreenEnergyHub.Charges.IntegrationTests.TestCommon;
using GreenEnergyHub.Charges.IntegrationTests.TestHelpers;
using NodaTime;
using Xunit;
using Xunit.Abstractions;
using Xunit.Categories;

namespace GreenEnergyHub.Charges.IntegrationTests.DomainTests.Charges
{
    /// <summary>
    /// Proof-of-concept on integration testing a function.
    /// </summary>
    [IntegrationTest]
    public class ChargeIngestionTests
    {
        [Collection(nameof(ChargesFunctionAppCollectionFixture))]
        public class RunAsync : FunctionAppTestBase<ChargesFunctionAppFixture>, IAsyncLifetime
        {
            public RunAsync(ChargesFunctionAppFixture fixture, ITestOutputHelper testOutputHelper)
                : base(fixture, testOutputHelper)
            {
            }

            public Task InitializeAsync()
            {
                return Task.CompletedTask;
            }

            public Task DisposeAsync()
            {
                Fixture.PostOfficeListener.ResetMessageHandlersAndReceivedMessages();
                return Task.CompletedTask;
            }

            [Fact]
            public async Task When_ChargeIsReceived_Then_AHttp200ResponseIsReturned()
            {
                var request = CreateTariffWithPricesRequest();

                var actualResponse = await Fixture.HostManager.HttpClient.SendAsync(request);

                actualResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            }

            [Fact]
            public async Task When_ChargeIsReceived_Then_HttpResponseHasCorrelationId()
            {
                var request = CreateTariffWithPricesRequest();

                var actualResponse = await Fixture.HostManager.HttpClient.SendAsync(request);

                actualResponse.GetCorrelationId().Should().NotBeEmpty();
            }

            /// <summary>
            /// Test of old Message Hub simulating code. Will be refactored in upcoming stories.
            /// </summary>
            [Fact]
            public async Task When_ChargeIsReceived_Then_MessageIsSentToPostOffice()
            {
                // Arrange
                var request = CreateTariffWithPricesRequest();

                var body = string.Empty;
                using var isMessageReceivedEvent = await Fixture.PostOfficeListener
                    .WhenAny()
                    .VerifyOnceAsync(receivedMessage =>
                    {
                        body = receivedMessage.Body.ToString();

                        return Task.CompletedTask;
                    });

                // Act
                await Fixture.HostManager.HttpClient.SendAsync(request);

                // Assert
                var isMessageReceived = isMessageReceivedEvent.Wait(TimeSpan.FromSeconds(10));
                isMessageReceived.Should().BeTrue();
                body.Should().NotBeEmpty();
            }

            [Fact]
            public async Task When_ChargeIsReceived_Then_ChargeCreatedIntegrationEventIsPublished()
            {
                // Arrange
                var request = CreateTariffWithPricesRequest();

                ChargeCreated chargeCreated = null!;
                using var chargeCreatedListenerAwaiter = await Fixture.ChargeCreatedListener
                    .WhenAny()
                    .VerifyOnceAsync(receivedMessage =>
                    {
                        chargeCreated = ChargeCreated
                            .Parser
                            .ParseFrom(receivedMessage.Body);

                        return Task.CompletedTask;
                    });

                // Act
                await Fixture.HostManager.HttpClient.SendAsync(request);

                // Assert

                // => Event is published
                var isChargeCreatedReceived = chargeCreatedListenerAwaiter.Wait(TimeSpan.FromSeconds(10));
                isChargeCreatedReceived.Should().BeTrue();

                // => The published event is the expected one
                chargeCreated!.ChargeId.Should().Be("47123");
                chargeCreated.ChargeOwner.Should().Be("8100000000030");
                chargeCreated.ChargeType.Should().Be(ChargeCreated.Types.ChargeType.CtTariff);
            }

            [Fact]
            public async Task When_ChargeIncludingPriceIsReceived_Then_ChargePricesUpdatedIntegrationEventIsPublished()
            {
                // Arrange
                var request = CreateTariffWithPricesRequest();
                using var eventualChargePriceUpdatedEvent = await Fixture.ChargePricesUpdatedListener.BeginListenForMessageAsync().ConfigureAwait(false);

                // Act
                var response = await Fixture.HostManager.HttpClient.SendAsync(request);

                // Assert
                var isChargePricesUpdatedReceived = eventualChargePriceUpdatedEvent.MessageAwaiter!.Wait(TimeSpan.FromSeconds(5));
                isChargePricesUpdatedReceived.Should().BeTrue();
                eventualChargePriceUpdatedEvent.CorrelationId.Should().Be(response.GetCorrelationId());
            }

            private static HttpRequestMessage CreateTariffWithPricesRequest()
            {
                var testFilePath = "TestFiles/ValidCreateTariffCommand.xml";
                var clock = SystemClock.Instance;
                var chargeJson = EmbeddedResourceHelper.GetEmbeddedFile(testFilePath, clock);

                var request = new HttpRequestMessage(HttpMethod.Post, "api/ChargeIngestion");
                request.Content = new StringContent(chargeJson, Encoding.UTF8, "application/xml");
                return request;
            }
        }
    }
}
