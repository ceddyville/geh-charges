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

using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using GreenEnergyHub.Charges.Application.Charges.Handlers.Message;

namespace GreenEnergyHub.Charges.Application.Charges.Handlers
{
    /// <summary>
    /// Contract for handling a change of charges message.
    /// </summary>
    public interface IChargesMessageHandler
    {
        /// <summary>
        /// Synchronously handle the message.
        /// </summary>
        /// <param name="message"></param>
        /// <returns>Returns the result of the synchronous handling of the message.</returns>
        Task<ChargesMessageResult> HandleAsync(ChargesMessage message);
    }
}
