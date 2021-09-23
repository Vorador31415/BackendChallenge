using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EvolutionizerPlanerWeb {
    public abstract class AutoMapperBase {
        protected readonly IMapper _mapper;

        public class ProgramDto {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public System.DateTime StartDate { get; set; }
            public System.DateTime EndDate { get; set; }
            public int Duration { get; set; }
            public int Timestamp { get; set; }
        }

        public class ProjectsDto {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public System.DateTime StartDate { get; set; }
            public System.DateTime EndDate { get; set; }
            public int Duration { get; set; }
            public int Timestamp { get; set; }
        }

        public class TasksDto {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public System.DateTime StartDate { get; set; }
            public System.DateTime EndDate { get; set; }
            public int Duration { get; set; }
            public int Timestamp { get; set; }
        }

        protected AutoMapperBase() {
            var config = new MapperConfiguration(x =>
            {
                x.CreateMap<ProgramDataAccess.Program, ProgramDto>();
                x.CreateMap<ProgramDataAccess.Projects, ProjectsDto>();
                x.CreateMap<ProgramDataAccess.Tasks, TasksDto>();
            });

            _mapper = config.CreateMapper();
        }
    }
}