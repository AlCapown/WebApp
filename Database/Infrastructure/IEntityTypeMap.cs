using Microsoft.EntityFrameworkCore;

namespace WebApp.Database;

public interface IEntityTypeMap
{
    void Map(ModelBuilder builder);
}
