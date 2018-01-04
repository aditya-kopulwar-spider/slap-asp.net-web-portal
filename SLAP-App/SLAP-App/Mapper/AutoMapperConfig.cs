
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
                AppraisalProcessMapper.Initialize(config);
                PCAssociateUserViewModelMapper.Initialize(config);
                PeerMapper.Initialize(config);
                EmployeeMapper.Initialize(config);
//                config.CreateMap<AppraisalProcessViewModel, AppraisalProcess>();
//                config.CreateMap<PCAssociateUserViewModel, PCAssociate>().ForMember(p=>p.);
            });
        }
    }
}