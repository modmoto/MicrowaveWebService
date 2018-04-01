//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System.Linq;
using Domain.Posts;
using GraphQL;
using GraphQL.Types;
using Newtonsoft.Json;
using SqlAdapter;

namespace HttpAdapter.Users
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Domain.Users;
    using Microsoft.AspNetCore.Mvc;
    using Application.Users;

    [Route("api/usersgraphl")]
    public class UserGraphlController : Controller
    {
        private readonly IUserRepository _userRepository;

        public UserCommandHandler Handler { get; private set; }
        
        public UserGraphlController(UserCommandHandler Handler, EventStoreContext context, IUserRepository userRepository)
        {
            _userRepository = userRepository;
            this.Handler = Handler;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] GraphQLQuery query)
        {
            var schema = new Schema { Query = new UserQuery(_userRepository) };

            var result = await new DocumentExecuter().ExecuteAsync(_ =>
            {
                _.Schema = schema;
                _.Query = query.Query;

            }).ConfigureAwait(false);

            if (result.Errors?.Count > 0)
            {
                return new BadRequestObjectResult(result);
            }

            return Ok(result);
        }
    }

    public class UserType : ObjectGraphType<User>
    {
        public UserType()
        {
            Field(x => x.IdToString).Description("The Id of the user.");
            Field(x => x.Name, nullable: true).Description("The name of the user.");
            Field(x => x.Age, nullable: true).Description("The age of the user.");
        }
    }

    public class UserQuery : ObjectGraphType
    {
        public UserQuery(IUserRepository userRepository)
        {
            Field<UserType>(
                "user",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "id", Description = "id of the user" }
                ),
                resolve: context =>
                {
                    return userRepository.GetUser(context.GetArgument<Guid>("id"));
                });
        }
    }

    public class GraphQLQuery
    {
        public string OperationName { get; set; }
        public string NamedQuery { get; set; }
        public string Query { get; set; }
        public string Variables { get; set; }
    }
}
