// namespace cmedcc_idass.backend.Utilize;


// public class EntityFrmwkDaoUtilize(IConfiguration pvConfiguration)
// {
//     public IQueryable<T> ExecQuery<T>(Func<IQueryable<T>, IQueryable<T>>? queryBuilder = null) where T : class
//     {
//         var connectionString = GetConnectionString();
//         var pvContext = new ApplicationDbContext(connectionString);
//         IQueryable<T> query = pvContext.Set<T>();
//         if (queryBuilder != null)
//         {
//             query = queryBuilder(query);
//         }

//         return query;
//     }

//     public (int affectedCount, T? affectedEntity) ExecCommand<T>(T entity, Operators operators,
//         Expression<Func<T, bool>>? filter = null) where T : class
//     {
//         var connectionString = GetConnectionString();
//         using var pvContext = new ApplicationDbContext(connectionString);
//         switch (operators)
//         {
//             case Operators.Insert:
//                 return HandleInsert(pvContext, entity);
//             case Operators.Update:
//                 return HandleUpdate(pvContext, entity, filter);
//             case Operators.Delete:
//                 return HandleDelete(pvContext, entity, filter);
//             default:
//                 throw new AppException(ErrorCodes.MethodNotAllowed,
//                     "Invalid operation. Use 'INSERT', 'UPDATE', or 'DELETE'.");
//         }
//     }

//     //------------------------ Function handling operation ------------------------//
//     private string GetConnectionString()
//     {
//         var connectionString = pvConfiguration.GetConnectionString("DefaultConnection")
//                                ?? throw new AppException(ErrorCodes.NotFound, Strings.DataSourceNotFound);
//         return connectionString;
//     }

//     private static (int affectedCount, T? affectedEntity) HandleInsert<T>(ApplicationDbContext pvContext, T entity)
//         where T : class
//     {
//         pvContext.Set<T>().Add(entity);
//         return (pvContext.SaveChanges(), entity);
//     }

//     private static (int affectedCount, T? affectedEntity) HandleUpdate<T>(ApplicationDbContext pvContext, T entity,
//         Expression<Func<T, bool>>? filter) where T : class
//     {
//         var entityToUpdate = GetEntityToUpdate(pvContext, entity, filter);

//         if (entityToUpdate != null)
//         {
//             UpdateEntityProperties(pvContext, entityToUpdate, entity);
//             return (pvContext.SaveChanges(), entityToUpdate);
//         }

//         throw new AppException(ErrorCodes.NotFound, "Entity not found for update.");
//     }

//     private static T? GetEntityToUpdate<T>(ApplicationDbContext pvContext, T entity, Expression<Func<T, bool>>? filter)
//         where T : class
//     {
//         if (filter != null)
//         {
//             return pvContext.Set<T>().FirstOrDefault(filter);
//         }
//         else
//         {
//             return pvContext.Set<T>().Find(entity.GetType().GetProperty("Id")?.GetValue(entity));
//         }
//     }

//     private static void UpdateEntityProperties<T>(ApplicationDbContext pvContext, T entityToUpdate, T entity)
//         where T : class
//     {
//         var primaryKeyProperty = pvContext.Model.FindEntityType(typeof(T))?
//             .FindPrimaryKey()
//             ?.Properties
//             .FirstOrDefault()?.Name;

//         var entry = pvContext.Entry(entityToUpdate);

//         foreach (var property in entity.GetType().GetProperties())
//         {
//             if (property.Name == primaryKeyProperty)
//                 continue;

//             var newValue = property.GetValue(entity);
//             var existingValue = property.GetValue(entityToUpdate);

//             if (newValue != null && !newValue.Equals(existingValue))
//             {
//                 entry.Property(property.Name).CurrentValue = newValue;
//                 entry.Property(property.Name).IsModified = true;
//             }
//             else
//             {
//                 entry.Property(property.Name).IsModified = false;
//             }
//         }
//     }

//     private static (int affectedCount, T? affectedEntity) HandleDelete<T>(ApplicationDbContext pvContext, T entity,
//         Expression<Func<T, bool>>? filter) where T : class
//     {
//         if (filter != null)
//         {
//             var entityToDelete = pvContext.Set<T>().FirstOrDefault(filter);
//             if (entityToDelete != null)
//             {
//                 pvContext.Set<T>().Remove(entityToDelete);
//             }
//             else
//             {
//                 throw new AppException(ErrorCodes.NotFound,
//                     "Entity not found for deletion with the given filter.");
//             }
//         }
//         else
//         {
//             pvContext.Set<T>().Remove(entity);
//         }

//         return (pvContext.SaveChanges(), entity);
//     }
// }