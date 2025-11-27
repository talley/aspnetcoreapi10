namespace aspnetcoreapi10.EndPoints
{
    public static class UsersEndPoints
    {
        public static void MapUsersEndPoints(this WebApplication app)
        {
            //app.MapGroup("/users");
            app.MapGet("/users", async (northwindContext db) =>
            {
                var users =await db.users.ToListAsync();
                return Results.Ok(users);
            })
            .WithName("GetUsers");

            app.MapGet("/users/{id}", async (northwindContext db, int id) =>
            {
                var user = await db.users.FindAsync(id);
                return user != null ? Results.Ok(user) : Results.NotFound();
            })
            .WithName("GetUserById");

            app.MapPost("/users", async (northwindContext db, users user) =>
            {
                await db.users.AddAsync(user).ConfigureAwait(false);
                await db.SaveChangesAsync().ConfigureAwait(false);
                return Results.Created($"/users/{user.id}", user);
            })
            .WithName("CreateUser");

            app.MapPut("/users/{id}", async (northwindContext db, int id, users updatedUser) =>
            {
                var user = await db.users.FindAsync(id);
                if (user == null)
                {
                    return Results.NotFound();
                }
                
                DateTime utcNow = DateTime.UtcNow;

                // Create a new DateTime that has the Unspecified Kind
                DateTime unspecifiedUtc = DateTime.SpecifyKind(utcNow, DateTimeKind.Unspecified);

                user.name = updatedUser.name;
                user.age = updatedUser.age;
                user.status = updatedUser.status;
                user.addedat = updatedUser.addedat;
                user.addedby = updatedUser.updatedby;
                user.updatedat=unspecifiedUtc;
                user.updatedby=Environment.MachineName;
               
                db.Entry(user).State = EntityState.Modified;
                await db.SaveChangesAsync().ConfigureAwait(false);
                return Results.NoContent();
            })
            .WithName("UpdateUser");


            app.MapDelete("/users/{id}", async (northwindContext db, int id) =>
            {
                var user = await db.users.FindAsync(id);
                if (user == null)
                {
                    return Results.NotFound();
                }

                db.users.Remove(user);
                await db.SaveChangesAsync().ConfigureAwait(false);
                return Results.NoContent();
            })
            .WithName("DeleteUser");
        }
    }
}