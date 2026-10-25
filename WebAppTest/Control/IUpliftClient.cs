using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testing.DTO;

namespace Testing.Control
{
    public interface IUpliftClient
    {
        Task<UpliftResponse> AnalyzeAsync(
            UpliftRequest request
        );
    }
}
