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

using System.ComponentModel;
using FluentAssertions;
using GreenEnergyHub.Charges.Infrastructure.Core.Function;
using Xunit;
using Xunit.Categories;

namespace GreenEnergyHub.Charges.Tests.Infrastructure.Core.Function
{
    [UnitTest]
    public class B2BErrorMessageFactoryTests
    {
        [Fact]
        public void Create_WhenFactoryCreateWithValidB2BCode_ReturnCorrectErrorMessage()
        {
            // Arrange
            var expectedCode = "B2B-008";
            var expectedMessage = "The sender organization provided in the request body does not match the organization in the bearer token.";

            // Act
            var sut = B2BErrorMessageFactory.Create(B2BErrorCode.ActorIsNotWhoTheyClaimToBeErrorMessage);

            // Assert
            sut.Code.Should().Be(expectedCode);
            sut.Message.Should().Be(expectedMessage);
        }

        [Fact]
        public void Create_WhenFactoryCreateWithInvalidB2BCode_ThrowsException()
        {
            // Arrange
            var invalidB2BCode = (B2BErrorCode)0;

            // Assert
            Assert.Throws<InvalidEnumArgumentException>(() => B2BErrorMessageFactory.Create(invalidB2BCode));
        }
    }
}
