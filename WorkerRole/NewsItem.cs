//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WorkerRole
{
    using System;
    using System.Collections.Generic;
    
    public partial class NewsItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string URL { get; set; }
        public string ItemContent { get; set; }
        public DateTime DateTime { get; set; }

        public string ImageURL { get; set; }

        public int NewsSourceID { get; set; }

        public Nullable<int> CategoryID { get; set; }

        public int ClusterID { get; set; }
    }
}
