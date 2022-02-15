namespace ElasticSearch.API.Models
{
    public class Indices 
    {
        public string[] Names { get; set; }
        public string[] Privileges { get; set; }
    }

    public class Applications
    {
        public string Application { get; set; }
        public string[] Privileges { get; set; }
        public string[] Resources { get; set; }
    }

    //{"MAK TEST":{"cluster":[],"indices":[{"names":["kibana_sample_data_flights"],"privileges":["read"],
    //"field_security":{"grant":["*"],"except":[]},"allow_restricted_indices":false}],
    //"applications":[],
    //"run_as":[],"metadata":{},"transient_metadata":{"enabled":true}}}


    //{"MAK TEST":{"cluster":[],"indices":[{"names":["kibana_sample_data_flights"],"privileges":["read"],
    //"field_security":{"grant":["*"],"except":[]},"allow_restricted_indices":false}],
    //"applications":[{"application":"kibana-.kibana","privileges":["feature_dashboard.read"],"resources":["*"]}],
    //"run_as":[],"metadata":{},"transient_metadata":{"enabled":true}}}

    public class Role
    {
        public string[] Cluster { get; set; }
        public List<Indices> Indices { get; set; }
        public List<Applications> Applications { get; set; }
        public Dictionary<string, int> Metadata { get; set; }

    }
}
