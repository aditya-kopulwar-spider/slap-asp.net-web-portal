using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using SLAP_App.Models;
using SLAP_Data;

namespace SLAP_App.Mapper
{
    public class ActiveDirectoryUserViewModelMapper
    {
        public static void Initialize(IMapperConfigurationExpression cfg)
        {
            var mappingExpression = cfg.CreateMap<User, ActiveDirectoryUser>();
            mappingExpression.ForMember(destinationMember=>destinationMember.ActiveDirectoryUser1,source=>source.Ignore());
            mappingExpression.ForMember(destinationMember=>destinationMember.ActiveDirectoryUser2,source=>source.MapFrom(p=>p.Manager));
            mappingExpression.ForMember(destinationMember=>destinationMember.ActiveDirectoryUserId,source=>source.Ignore());
            var map = cfg.CreateMap<ActiveDirectoryUser, User>();
            map.ForMember(destinationMember => destinationMember.IsAdmin, source => source.Ignore());
            map.ForMember(destinationMember => destinationMember.IsPC, source => source.Ignore());
            map.ForMember(destinationMember => destinationMember.PC, source => source.Ignore());
            map.ForMember(destinationMember => destinationMember.PCAssociateModel, source => source.Ignore());
            map.ForMember(destinationMember => destinationMember.SeekingFeedbackFrom, source => source.Ignore());
            map.ForMember(destinationMember => destinationMember.SendingFeedbackTo, source => source.Ignore());
            map.ForMember(destinationMember => destinationMember.businessPhones, source => source.Ignore());
            map.ForMember(destinationMember => destinationMember.surname, source => source.Ignore());
            map.ForMember(destinationMember => destinationMember.preferredLanguage, source => source.Ignore());
            map.ForMember(destinationMember => destinationMember.mobilePhone, source => source.Ignore());
            map.ForMember(destinationMember => destinationMember.jobTitle, source => source.Ignore());
            map.ForMember(destinationMember => destinationMember.officeLocation, source => source.Ignore());
            var expression = cfg.CreateMap<ActiveDirectoryUser, ActiveDirectoryUserViewModel>();
            expression.ForMember(destinationMember => destinationMember.Manager,
                source => source.MapFrom(p => p.ActiveDirectoryUser2));
            expression.ForMember(destinationMember => destinationMember.Associates,
                source => source.MapFrom(p => p.ActiveDirectoryUser1));
            cfg.CreateMap<ActiveDirectoryUserViewModel, ActiveDirectoryUser>();
        }
    }
}