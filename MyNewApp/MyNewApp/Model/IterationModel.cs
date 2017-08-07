using System;
using System.Collections.Generic;

namespace MyNewApp.Model
{
    public class IterationsList
    {
        public List<IterationModel> Iterations { get; set; }
    }

    public class IterationModel
    {
        public string Id { get; set; }
        public string ProjectId { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public string Created { get; set; }
        public string LastModifiedAt { get; set; }
        public string TrainedAt { get; set; }
        public string IsDefault { get; set; }
    }
}