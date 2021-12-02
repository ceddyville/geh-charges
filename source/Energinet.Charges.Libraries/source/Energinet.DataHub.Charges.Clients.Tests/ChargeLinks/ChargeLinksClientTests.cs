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
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Moq.Protected;
using Xunit;
using Xunit.Categories;

namespace Energinet.DataHub.Charges.Clients.CreateDefaultChargeLink.Tests.ChargeLinks
{
    [UnitTest]
    public class ChargeLinksClientTests
    {
        [Fact]
        public async Task GetAsync_WhenMeteringPointHasLinks_ReturnsChargeLinks()
        {
            // Arrange
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent("[{\"ChargeType\":3,\"ChargeId\":\"40000\",\"ChargeName\":\"Transmissionsnettarif\",\"ChargeOwnerId\":\"5790000432752\",\"ChargeOwnerName\":\"Energinet(SYO)\",\"TaxIndicator\":false,\"TransparentInvoicing\":false,\"Factor\":1,\"StartDateTimeUtc\":\"2019-12-31T23:00:00Z\",\"EndDateTimeUtc\":null}]", Encoding.UTF8, "application/json"),
                })
                .Verifiable();

            var httpClient = new HttpClient(mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("http://chargelinks-test.com/"),
            };

            var sut = ChargeLinksClientFactory.CreateClient(httpClient);

            // Act
            var result = await sut.GetAsync("1").ConfigureAwait(false);

            // Assert
            result.Should().NotBeNull();
            result[0].ChargeId.Should().Be("40000");

            var expectedStartDateTime = new DateTime(2019, 12, 31, 23, 00, 00);
            result[0].StartDateTimeUtc.Should().Be(expectedStartDateTime);
        }
    }
}
