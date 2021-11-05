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
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Energinet.DataHub.Charges.Clients.IntegrationTests.Fixtures;
using Energinet.DataHub.Charges.Libraries.Common;
using Energinet.DataHub.Charges.Libraries.DefaultChargeLink;
using Energinet.DataHub.Charges.Libraries.Factories;
using Energinet.DataHub.Charges.Libraries.Models;
using FluentAssertions;
using GreenEnergyHub.TestHelpers;
using Xunit;
using Xunit.Abstractions;

namespace Energinet.DataHub.Charges.Clients.IntegrationTests.DefaultChargeLink
{
    public static class DefaultChargeLinkTests
    {
        [Collection(nameof(ChargesClientsCollectionFixture))]
        [SuppressMessage("ReSharper", "CA1034", Justification = "Integration test")]
        public class CreateDefaultChargeLinksRequestAsync : LibraryTestBase<ChargesClientsFixture>, IAsyncLifetime
        {
            private readonly string _replyToQueueName;
            private readonly string _requestQueueName;
            private readonly ServiceBusClient _serviceBusClient;
            private readonly ServiceBusTestListener _serviceBusTestListener;
            private readonly ServiceBusRequestSenderFactory _serviceBusRequestSenderFactory;

            public CreateDefaultChargeLinksRequestAsync(ChargesClientsFixture fixture, ITestOutputHelper testOutputHelper)
                : base(fixture, testOutputHelper)
            {
                _replyToQueueName = EnvironmentVariableReader.GetEnvironmentVariable(
                    EnvironmentSettingNames.CreateLinkReplyQueueName, string.Empty);
                _requestQueueName = EnvironmentVariableReader.GetEnvironmentVariable(
                    EnvironmentSettingNames.CreateLinkRequestQueueName, string.Empty);

                string serviceBusConnectionString = EnvironmentVariableReader.GetEnvironmentVariable(
                    EnvironmentSettingNames.IntegrationEventSenderConnectionString, string.Empty);
                _serviceBusClient = new ServiceBusClient(serviceBusConnectionString);

                _serviceBusTestListener = new ServiceBusTestListener(Fixture);
                _serviceBusRequestSenderFactory = new ServiceBusRequestSenderFactory();
            }

            public Task InitializeAsync()
            {
                return Task.CompletedTask;
            }

            public async Task DisposeAsync()
            {
                Fixture.ServiceBusListenerMock.ResetMessageHandlersAndReceivedMessages();
                await _serviceBusClient.DisposeAsync().ConfigureAwait(false);
            }

            [Theory]
            [AutoDomainData]
            public async Task When_CreateDefaultChargeLinksRequestAsync_Then_RequestIsSendToCharges(
                CreateDefaultChargeLinksDto createDefaultChargeLinksDto, string correlationId)
            {
                // Arrange
                using var result = await _serviceBusTestListener.ListenForMessageAsync().ConfigureAwait(false);
                await using var sut = new DefaultChargeLinkClient(
                    _serviceBusClient, _serviceBusRequestSenderFactory, _replyToQueueName, _requestQueueName);

                // Act
                await sut.CreateDefaultChargeLinksRequestAsync(createDefaultChargeLinksDto, correlationId).ConfigureAwait(false);

                // Assert
                // => Service Bus (timeout should not be more than 5 secs).
                var isMessageReceived = result.IsMessageReceivedEvent!.Wait(TimeSpan.FromSeconds(5));

                isMessageReceived.Should().BeTrue();
                result.CorrelationId.Should().Be(correlationId);
                result.Body.Should().NotBeNull();
            }

            // [Theory]
            // [AutoDomainData]
            // public async Task When_CreateDefaultChargeLinksSucceededReplyAsync_Then_ReplyIsSendFromCharges(
            //     CreateDefaultChargeLinksSucceededDto createDefaultChargeLinksSucceededDto, string correlationId)
            // {
            //     // Arrange
            //     using var result = await _serviceBusTestListener.ListenForMessageAsync().ConfigureAwait(false);
            //     await using var sut = new DefaultChargeLinkClient(
            //         _serviceBusClient, _serviceBusRequestSenderFactory, _replyToQueueName, _requestQueueName);
            //
            //     // Act
            //     await sut.CreateDefaultChargeLinksSucceededReplyAsync(
            //         createDefaultChargeLinksSucceededDto, correlationId, _replyToQueueName).ConfigureAwait(false);
            //
            //     // Assert
            //     // => Service Bus (timeout should not be more than 5 secs).
            //     var isMessageReceived = result.IsMessageReceivedEvent!.Wait(TimeSpan.FromSeconds(5));
            //
            //     isMessageReceived.Should().BeTrue();
            //     result.CorrelationId.Should().Be(correlationId);
            //     result.Body.Should().NotBeNull();
            // }
            //
            // [Theory]
            // [AutoDomainData]
            // public async Task When_CreateDefaultChargeLinksFailedReplyAsync_Then_ReplyIsSendFromCharges(
            //     CreateDefaultChargeLinksFailedDto createDefaultChargeLinksFailedDto, string correlationId)
            // {
            //     // Arrange
            //     using var result = await _serviceBusTestListener.ListenForMessageAsync().ConfigureAwait(false);
            //     await using var sut = new DefaultChargeLinkClient(
            //         _serviceBusClient, _serviceBusRequestSenderFactory, _replyToQueueName, _requestQueueName);
            //
            //     // Act
            //     await sut.CreateDefaultChargeLinksFailedReplyAsync(
            //         createDefaultChargeLinksFailedDto, correlationId, _replyToQueueName).ConfigureAwait(false);
            //
            //     // Assert
            //     // => Service Bus (timeout should not be more than 5 secs).
            //     var isMessageReceived = result.IsMessageReceivedEvent!.Wait(TimeSpan.FromSeconds(5));
            //
            //     isMessageReceived.Should().BeTrue();
            //     result.Body.Should().NotBeNull();
            //     result.CorrelationId.Should().Be(correlationId);
            // }
        }
    }
}
