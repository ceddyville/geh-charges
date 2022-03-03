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
using System.Threading.Tasks;
using GreenEnergyHub.Charges.Application.Charges.Acknowledgement;
using GreenEnergyHub.Charges.Domain.Charges;
using GreenEnergyHub.Charges.Domain.Dtos.ChargeCommandReceivedEvents;
using GreenEnergyHub.Charges.Domain.Dtos.ChargeCommands;
using GreenEnergyHub.Charges.Domain.Dtos.Validation;
using GreenEnergyHub.Charges.Infrastructure.Core.Persistence;

namespace GreenEnergyHub.Charges.Application.Charges.Handlers
{
    public class ChargeCommandReceivedEventHandler : IChargeCommandReceivedEventHandler
    {
        private readonly IChargeCommandReceiptService _chargeCommandReceiptService;
        private readonly IValidator<ChargeCommand, ChargeOperationDto> _validator;
        private readonly IChargeRepository _chargeRepository;
        private readonly IChargeFactory _chargeFactory;
        private readonly IChargePeriodFactory _chargePeriodFactory;
        private readonly IUnitOfWork _unitOfWork;

        public ChargeCommandReceivedEventHandler(
            IChargeCommandReceiptService chargeCommandReceiptService,
            IValidator<ChargeCommand, ChargeOperationDto> validator,
            IChargeRepository chargeRepository,
            IChargeFactory chargeFactory,
            IChargePeriodFactory chargePeriodFactory,
            IUnitOfWork unitOfWork)
        {
            _chargeCommandReceiptService = chargeCommandReceiptService;
            _validator = validator;
            _chargeRepository = chargeRepository;
            _chargeFactory = chargeFactory;
            _chargePeriodFactory = chargePeriodFactory;
            _unitOfWork = unitOfWork;
        }

        public async Task HandleAsync(ChargeCommandReceivedEvent commandReceivedEvent)
        {
            if (commandReceivedEvent == null) throw new ArgumentNullException(nameof(commandReceivedEvent));

            foreach (var chargeOperationDto in commandReceivedEvent.Command.Charges)
            {
                var inputValidationResult = _validator.InputValidate(commandReceivedEvent.Command);
                if (inputValidationResult.IsFailed)
                {
                    await _chargeCommandReceiptService
                        .RejectAsync(commandReceivedEvent.Command, inputValidationResult).ConfigureAwait(false);
                    continue;
                }

                var businessValidationResult = await _validator
                    .BusinessValidateAsync(commandReceivedEvent.Command).ConfigureAwait(false);
                if (businessValidationResult.IsFailed)
                {
                    await _chargeCommandReceiptService.RejectAsync(
                        commandReceivedEvent.Command, businessValidationResult).ConfigureAwait(false);
                    continue;
                }

                var charge = await GetChargeAsync(chargeOperationDto).ConfigureAwait(false);
                var operationType = GetOperationType(chargeOperationDto, charge);

                switch (operationType)
                {
                    case OperationType.Create:
                        await HandleCreateEventAsync(chargeOperationDto).ConfigureAwait(false);
                        break;
                    case OperationType.Update:
                        HandleUpdateEvent(charge, chargeOperationDto);
                        break;
                    case OperationType.Stop:
                        StopCharge(charge, chargeOperationDto);
                        break;
                    default:
                        throw new InvalidOperationException("Could not handle charge dto");
                }

                await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);

                // Todo: Change to accept operation?
                await _chargeCommandReceiptService.AcceptAsync(commandReceivedEvent.Command).ConfigureAwait(false);
            }
        }

        private async Task HandleCreateEventAsync(ChargeOperationDto chargeOperationDto)
        {
            var charge = await _chargeFactory
                .CreateFromChargeOperationDtoAsync(chargeOperationDto)
                .ConfigureAwait(false);

            await _chargeRepository.AddAsync(charge).ConfigureAwait(false);
        }

        private void HandleUpdateEvent(Charge? charge, ChargeOperationDto chargeOperationDto)
        {
            if (charge == null)
                throw new InvalidOperationException("Could not update charge. Charge not found.");

            var newChargePeriod = _chargePeriodFactory.CreateFromChargeOperationDto(chargeOperationDto);
            charge.UpdateCharge(newChargePeriod);
            _chargeRepository.Update(charge);
        }

        private void StopCharge(Charge? charge, ChargeOperationDto chargeOperationDto)
        {
            if (charge == null)
                throw new InvalidOperationException("Could not stop charge. Charge not found.");

            if (chargeOperationDto.EndDateTime == null)
                throw new InvalidOperationException("Could not stop charge. Invalid end date.");

            var chargeOperationEndDateTime = chargeOperationDto.EndDateTime.Value;

            charge.StopCharge(chargeOperationEndDateTime);
            _chargeRepository.Update(charge);
        }

        private static OperationType GetOperationType(ChargeOperationDto chargeOperationDto, Charge? charge)
        {
            if (chargeOperationDto.StartDateTime == chargeOperationDto.EndDateTime)
            {
                return OperationType.Stop;
            }

            // Todo: If not first in list then it is also an update!
            return charge != null ? OperationType.Update : OperationType.Create;
        }

        private async Task<Charge?> GetChargeAsync(ChargeOperationDto chargeOperationDto)
        {
            var chargeIdentifier = new ChargeIdentifier(
                chargeOperationDto.ChargeId,
                chargeOperationDto.ChargeOwner,
                chargeOperationDto.Type);

            return await _chargeRepository.GetOrNullAsync(chargeIdentifier).ConfigureAwait(false);
        }
    }

    // Internal, so far...
    internal enum OperationType
    {
        Create = 0,
        Update = 1,
        Stop = 2,
    }
}
