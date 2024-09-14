namespace PostGram.BLL.Interfaces.Base.Queries;

public interface IQueryHandler<in TQuery, TQueryResult> where TQuery : IQuery where TQueryResult : IQueryResult
{
    Task<TQueryResult> Execute(TQuery query);
}