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

using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Energinet.DataHub.Core.FunctionApp.TestCommon;
using FluentAssertions;
using GreenEnergyHub.Charges.IntegrationTests.Fixtures;
using GreenEnergyHub.Charges.IntegrationTests.TestFiles.ChargeLinks;
using GreenEnergyHub.Charges.IntegrationTests.TestHelpers;
using NodaTime;
using Xunit;
using Xunit.Abstractions;
using Xunit.Categories;

namespace GreenEnergyHub.Charges.IntegrationTests.DomainTests
{
    [IntegrationTest]
    public class ChargeLinkIngestionTests
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
                Fixture.MessageHubMock.Clear();
                return Task.CompletedTask;
            }

            [Fact]
            public async Task When_ChargeLinkIsReceived_Then_AHttp200ResponseIsReturned()
            {
                var request = CreateHttpRequest(ChargeLinkDocument.AnyValid, out _);

                var actualResponse = await Fixture.HostManager.HttpClient.SendAsync(request);

                actualResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            }

            [Fact]
            public async Task When_InvalidChargeLinkIsReceived_Then_AHttp400ResponseIsReturned()
            {
                // Arrange
                var request = CreateHttpRequest(ChargeLinkDocument.InvalidSchema, out _);

                // Act
                var actualResponse = await Fixture.HostManager.HttpClient.SendAsync(request);

                // Assert
                actualResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            }

            [Fact]
            public async Task Given_NewTaxChargeLinkMessage_When_GridAccessProviderPeeks_Then_MessageHubReceivesReply()
            {
                // Arrange
                var request = CreateHttpRequest(ChargeLinkDocument.AnyTax, out var correlationId);

                // Act
                var response = await Fixture.HostManager.HttpClient.SendAsync(request);

                // Assert
                // We expect two message types in the messagehub, one for the receipt and one for the charge link itself
                await Fixture.MessageHubMock.AssertPeekReceivesReplyAsync(correlationId, 2);
            }

            private static HttpRequestMessage CreateHttpRequest(string testFilePath, out string correlationId)
            {
                var clock = SystemClock.Instance;
                var chargeLinkJson = EmbeddedResourceHelper.GetEmbeddedFile(testFilePath, clock);
                correlationId = CorrelationIdGenerator.Create();

                var request = new HttpRequestMessage(HttpMethod.Post, "api/ChargeLinkIngestion")
                {
                    Content = new StringContent(chargeLinkJson, Encoding.UTF8, "application/xml"),
                };
                request.ConfigureTraceContext(correlationId);

                return request;
            }
        }
    }
}
