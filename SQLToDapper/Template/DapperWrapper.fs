namespace SQLToDapper

module DapperWrapper =

    let template = "
using Dapper;
using Microsoft.SqlServer.Server;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace DapperWrapper
{
    public class CommandArguments
    {
        public CommandArguments(string connection, string routineFullName, object args, CommandType commandType)
        {
            RoutineFullName = routineFullName;
            Connection = connection;
            Args = args;
            CommandType = commandType;
        }

        public string RoutineFullName { get; }
        public string Connection { get; }
        public object Args { get; }
        public CommandType CommandType { get; }
    }

    public class DapperExecute
    {
        private CommandArguments Args;

        public DapperExecute(CommandArguments commandArguments)
        {
            this.Args = commandArguments;
        }

        public int Execute()
        {
            using (var conn = new SqlConnection(Args.Connection))
            {
                conn.Open();
                return conn.Execute(Args.RoutineFullName, Args.Args, commandType: Args.CommandType);
            }
        }

        public async Task<int> ExecuteAsync()
        {
            using (var conn = new SqlConnection(Args.Connection))
            {
                await conn.OpenAsync();
                return await conn.ExecuteAsync(Args.RoutineFullName, Args.Args, commandType: Args.CommandType);
            }
        }

    }

    public class DapperQuery<T>
    {
        private CommandArguments Args;

        public DapperQuery(CommandArguments args)
        {
            Args = args;
        }

        public async Task<IEnumerable<T>> ExecuteAsync()
        {
            using (var conn = new SqlConnection(Args.Connection))
            {
                await conn.OpenAsync();
                return await conn.QueryAsync<T>(Args.RoutineFullName, Args.Args, commandType: Args.CommandType);
            }
        }
    }

    public interface IUDT
    {
        IEnumerable<SqlDataRecord> SqlMeta();
        string TypeName { get; }
    }

    public class ArgsWithUDT : Dapper.DynamicParameters, Dapper.SqlMapper.IDynamicParameters
    {
        private readonly Dictionary<string, IUDT> data;

        public ArgsWithUDT(Dictionary<string, IUDT> data)
        {
            this.data = data;
        }

        public new void AddParameters(IDbCommand command, SqlMapper.Identity identity)
        {
            base.AddParameters(command, identity);

            var sqlCommand = (SqlCommand)command;
            sqlCommand.CommandType = CommandType.StoredProcedure;

            foreach (var pair in data)
            {
                var p = sqlCommand.Parameters.Add(pair.Key, SqlDbType.Structured);
                p.Direction = ParameterDirection.Input;
                p.TypeName = pair.Value.TypeName;
                p.Value = pair.Value.SqlMeta();
            }
        }
    }

}
"

