using System.Configuration;

namespace ProStudCreator
{
    public partial class ProStudentCreatorDBDataContext
    {
        public ProStudentCreatorDBDataContext() :
            base(ConfigurationManager.ConnectionStrings["ProStudCreatorConnectionString"].ConnectionString)
        {
            OnCreated();
        }

        public ProStudentCreatorDBDataContext(System.Data.Linq.Mapping.MappingSource mappingSource) :
            base(ConfigurationManager.ConnectionStrings["ProStudCreatorConnectionString"].ConnectionString, mappingSource)
        {
            OnCreated();
        }
    }
}