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

using GreenEnergyHub.Charges.Application.ChargeLinks.Handlers;
using GreenEnergyHub.Charges.Domain.Dtos.ChargeLinksDataAvailableNotifiedEvents;
using GreenEnergyHub.Charges.Infrastructure.Core.MessagingExtensions.Registration;
using Microsoft.Extensions.DependencyInjection;

namespace GreenEnergyHub.Charges.FunctionHost.Configuration
{
    internal static class DefaultChargeLinkEventReplierConfiguration
    {
        internal static void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<ICreateDefaultChargeLinksReplyHandler, CreateDefaultChargeLinksReplyHandler>();
            serviceCollection.AddScoped<IChargeLinksDataAvailableNotifiedEventFactory, ChargeLinksDataAvailableNotifiedEventFactory>();
            serviceCollection
                .AddMessaging()
                .AddInternalMessageExtractor<ChargeLinksDataAvailableNotifiedEvent>();
        }
    }
}
