using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using SLAP_App.Models;
using SLAP_Data;

namespace SLAP_App.Mapper
{
    public class PCAssociateUserViewModelMapper
    {
        public static void Initialize(IMapperConfigurationExpression cfg)
        {
            var mappingExpression = cfg.CreateMap<PCAssociate, PCAssociateUserViewModel>();
            mappingExpression.ForMember(destinationMember=>destinationMember.AssociateDisplayName, source=>source.Ignore());
            cfg.CreateMap<PCAssociateUserViewModel,PCAssociate>();
            //todo seprate mapper for below mapping
            cfg.CreateMap<PCAssociate, PCAssociateViewModel>().ForMember(destinationMember => destinationMember.AssociateDisplayName, source => source.Ignore());
            cfg.CreateMap<PCAssociateViewModel, PCAssociate>();
            
        }
    }
}