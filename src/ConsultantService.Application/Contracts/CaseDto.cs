using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultantService.Application.Dtos
{

    public record CaseDto(
        Guid Id,
        string Description
    );
}
