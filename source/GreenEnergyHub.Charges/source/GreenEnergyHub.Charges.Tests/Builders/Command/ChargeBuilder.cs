// Copyright 2020 Energinet DataHub A/S
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
using GreenEnergyHub.Charges.Domain.Charges;

namespace GreenEnergyHub.Charges.Tests.Builders.Command
{
    public class ChargeBuilder
    {
        private List<Point> _points = new();
        private List<ChargePeriod> _periods = new();
        private string _name = "senderProvidedChargeId";
        private Guid _marketParticipantId = Guid.NewGuid();
        private ChargeType _chargeType = ChargeType.Tariff;
        private Resolution _resolution = Resolution.PT1H;
        private bool _taxIndicator;

        public ChargeBuilder WithPoints(IEnumerable<Point> points)
        {
            _points = points.ToList();
            return this;
        }

        public ChargeBuilder WithPeriods(IEnumerable<ChargePeriod> periods)
        {
            _periods = periods.ToList();
            return this;
        }

        public ChargeBuilder WithChargeName(string chargeName)
        {
            _name = chargeName;
            return this;
        }

        public ChargeBuilder WithMarketParticipantId(Guid marketParticipantId)
        {
            _marketParticipantId = marketParticipantId;
            return this;
        }

        public ChargeBuilder WithChargeType(ChargeType chargeType)
        {
            _chargeType = chargeType;
            return this;
        }

        public ChargeBuilder WithResolution(Resolution resolution)
        {
            _resolution = resolution;
            return this;
        }

        public ChargeBuilder WithTaxIndicator(bool taxIndicator)
        {
            _taxIndicator = taxIndicator;
            return this;
        }

        public Charge Build()
        {
            var chargeResult = Charge.CreateCharge(
                Guid.NewGuid(),
                _name,
                _marketParticipantId,
                _chargeType,
                _resolution,
                _taxIndicator,
                _points,
                _periods);
            return chargeResult.Charge;
        }

        public ChargeResult BuildWithChargeResult()
        {
            var chargeResult = Charge.CreateCharge(
                Guid.NewGuid(),
                _name,
                _marketParticipantId,
                _chargeType,
                _resolution,
                _taxIndicator,
                _points,
                _periods);
            return chargeResult;
        }
    }
}
