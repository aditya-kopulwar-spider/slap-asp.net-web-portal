
using SLAP_App.Models;
using SLAP_Data;

namespace SLAP_App.Mapper
{
    public static class AutoMapperConfig
    {
        public static void Initialize()
        {
            AutoMapper.Mapper.Initialize(config =>
            {
                config.CreateMap<AppraisalProcessViewModel, AppraisalProcess>();
            });
        }
    }
}