using AutoMapper;
using System.Collections.Generic;
using System.Reflection;

namespace smrpo_be.Utilities
{
    public static class MapperFactory
    {
        public static IMapper CreateMapper(this Assembly assembly)
        {
            IReadOnlyCollection<Profile> automapperProfiles = ReflectionHelper.GetInstancesDerivedFrom<Profile>(assembly);
            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                foreach (Profile profile in automapperProfiles)
                {
                    cfg.AddProfile(profile);
                }
            });

            IMapper mapper = config.CreateMapper();

            return mapper;
        }
    }
}
