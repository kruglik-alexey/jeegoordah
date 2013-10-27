using System.Web.Routing;
using Jeegoordah.Core.DL;

namespace Jeegoordah.Web.DL
{
    public class ContextDependentDbFactory
    {
        private readonly RequestContext requestContext;
        private readonly DbFactoryRepository dbFactoryRepository;

        public ContextDependentDbFactory(RequestContext requestContext, DbFactoryRepository dbFactoryRepository)
        {
            this.requestContext = requestContext;
            this.dbFactoryRepository = dbFactoryRepository;
        }

        public Db Open()
        {
            DbFactory dbFactory;
#if DEBUG
            bool isTest = requestContext.HttpContext.Request.Params["test"] != null;
            dbFactory = isTest ? dbFactoryRepository.TestDbFactory.Value : dbFactoryRepository.RealDbFactory.Value;
#else
            dbFactory = dbFactoryRepository.RealDbFactory.Value;
#endif
            return dbFactory.Open();
        }
    }
}