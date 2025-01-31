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

using System.Threading.Tasks;
using Energinet.DataHub.Core.SchemaValidation.Errors;
using Microsoft.Azure.Functions.Worker.Http;

namespace GreenEnergyHub.Charges.Infrastructure.Core.Function
{
    public interface IHttpResponseBuilder
    {
        HttpResponseData CreateAcceptedResponse(HttpRequestData request);

        Task<HttpResponseData> CreateBadRequestResponseAsync(
            HttpRequestData request,
            ErrorResponse errorResponse);

        HttpResponseData CreateBadRequestB2BResponse(HttpRequestData request, B2BErrorCode code);
    }
}
