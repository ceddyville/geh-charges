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

using GreenEnergyHub.Charges.Domain.GridAreaLinks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GreenEnergyHub.Charges.Infrastructure.Persistence.EntityConfigurations
{
    public class GridAreaLinkEntityConfiguration : IEntityTypeConfiguration<GridAreaLink>
    {
        public void Configure(EntityTypeBuilder<GridAreaLink> builder)
        {
            builder.ToTable(nameof(GridAreaLink));

            builder.HasKey(l => l.Id);
            builder.Property(l => l.Id).ValueGeneratedNever();
            builder.Property(l => l.GridAreaId);
            builder.Property(l => l.OwnerId);
        }
    }
}
