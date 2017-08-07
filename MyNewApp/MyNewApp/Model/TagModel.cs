using System;
using System.Collections.Generic;

namespace MyNewApp.Model
{
    public class TagModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageCount { get; set; }
    }
        
    public class TagsModel
    {
        public string TotalTaggedImages { get; set; }
        public string TotalUntaggedImages { get; set; }
        public List<TagModel> Tags { get; set; }
    }
}