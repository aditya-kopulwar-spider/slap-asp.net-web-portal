using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using SLAP_App.Models;
using SLAP_Data;
namespace SLAP_App.Mapper
{
    public class PeerMapper
    {
        public static void Initialize(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<Peer, PeerViewModel>();
            cfg.CreateMap<PeerViewModel, Peer>();
        }
    }
}