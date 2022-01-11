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
using System.Threading.Tasks;
using GreenEnergyHub.Charges.MessageHub.Infrastructure.Persistence;
using GreenEnergyHub.Charges.MessageHub.Models.AvailableData;
using Microsoft.EntityFrameworkCore;

namespace GreenEnergyHub.Charges.MessageHub.Models.AvailableChargeLinkReceiptData
{
    public class AvailableChargeLinkReceiptDataRepository : IAvailableDataRepository<AvailableChargeLinkReceiptData>
    {
        private readonly IMessageHubDatabaseContext _context;

        public AvailableChargeLinkReceiptDataRepository(IMessageHubDatabaseContext context)
        {
            _context = context;
        }

        public async Task StoreAsync(IEnumerable<AvailableChargeLinkReceiptData> availableChargeLinkReceiptData)
        {
            await _context.AvailableChargeLinkReceiptData.AddRangeAsync(availableChargeLinkReceiptData);
            await _context.SaveChangesAsync();
        }

        public async Task<IReadOnlyList<AvailableChargeLinkReceiptData>> GetAsync(IEnumerable<Guid> dataReferenceIds)
        {
            return await _context
                .AvailableChargeLinkReceiptData
                .Where(x => dataReferenceIds.Contains(x.AvailableDataReferenceId))
                .OrderBy(x => x.RequestDateTime)
                .ToListAsync();
        }
    }
}
