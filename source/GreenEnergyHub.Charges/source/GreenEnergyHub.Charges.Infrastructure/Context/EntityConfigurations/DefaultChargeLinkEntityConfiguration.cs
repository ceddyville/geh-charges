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

using GreenEnergyHub.Charges.Domain.DefaultChargeLinks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GreenEnergyHub.Charges.Infrastructure.Context.EntityConfigurations
{
    public class DefaultChargeLinkEntityConfiguration : IEntityTypeConfiguration<DefaultChargeLink>
    {
        public void Configure(EntityTypeBuilder<DefaultChargeLink> builder)
        {
            builder.ToTable("DefaultChargeLink", "Charges");

            builder.HasKey(x => x.Id);

            builder
                .Property(x => x.ChargeId)
                .HasColumnName("ChargeId");

            builder
                .Property(x => x.MeteringPointType)
                .HasColumnName("MeteringPointType");

            builder
                .Property(x => x.StartDateTime)
                .HasColumnName("StartDateTime");

            builder
                .Property(x => x.EndDateTime)
                .HasColumnName("EndDateTime");
        }
    }
}
