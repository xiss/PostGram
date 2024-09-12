using PostGram.Common.Interfaces.Services;

namespace PostGram.Common.Interfaces.Base.Queries;

public interface IQueryService<in TQuery, out TQueryResult> where TQuery : IQuery where TQueryResult : IQueryResult
{
    TQueryResult Execute(TQuery query);
}