using AutoMapper;
using EmployeeManagement.Application.Mappings;
using Microsoft.Extensions.Logging.Abstractions;

namespace EmployeeManagement.Tests.TestHelpers;

public static class MapperFactory
{
    public static IMapper Create()
    {
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>(), NullLoggerFactory.Instance);
        return configuration.CreateMapper();
    }
}
