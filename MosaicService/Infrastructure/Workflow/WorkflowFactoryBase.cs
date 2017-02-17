using System.Linq;
using VP.FF.PT.Common.Infrastructure.Logging;
using VP.FF.PT.Common.PlatformEssentials.Entities;

namespace MosaicSample.Infrastructure.Workflow
{
    public class WorkflowFactoryBase
    {
        private static readonly object _locking = new object();
        private static long _recipeItemId;
        protected ILogger _logger;

        private readonly IEntityContextFactory _contextFactory;

        protected WorkflowFactoryBase(IEntityContextFactory contextFactory, ILogger logger)
        {
            _logger = logger;
            _contextFactory = contextFactory;

            lock (_locking)
            {
                if (_recipeItemId == 0)
                {
                    using (var context = _contextFactory.CreateContext())
                    {
                        var recipeItems = context.GetQuery<RouteItem>();
                        _recipeItemId = recipeItems.Any() ? recipeItems.Max(a => a.Id) : 1;
                        context.SaveChanges();
                    }
                }
            }
        }

        public Route CreateRecipe()
        {
            return new Route();
        }

        protected void AddRecipeItem(Route recipe, int moduleType, int forbiddenModuleType = 0, params Value[] values)
        {
            var item = new RouteItem
            {
                Index = recipe.RouteItems.Count,
                ModuleType = moduleType,
                ForbiddenModuleType = forbiddenModuleType
            };

            foreach (var value in values)
            {
                item.ProcessSettings.Add(value);
            }

            recipe.RouteItems.Add(item);
        }

        protected void AddRecipeItem(Route recipe, int moduleType, string forceModuleInstance, int forbiddenModuleType = 0, params Value[] values)
        {
            var item = new RouteItem
            {
                Index = recipe.RouteItems.Count,
                ModuleType = moduleType,
                ForceModuleInstance = forceModuleInstance,
                ForbiddenModuleType = forbiddenModuleType
            };

            foreach (var value in values)
            {
                item.ProcessSettings.Add(value);
            }

            recipe.RouteItems.Add(item);
        }

        protected void SaveRecipeToDb(Route recipe, PlatformItem item)
        {
            var context = _contextFactory.CreateContext();

            foreach (var recipeItem in recipe.RouteItems)
            {
                lock (_locking)
                {
                    recipeItem.Id = ++_recipeItemId;
                }
                context.Add(recipeItem);
            }

            recipe.Id = item.ItemId;
            context.Add(recipe);
            context.Attach(item);
            context.SaveChangesAsync();
        }
    }
}
