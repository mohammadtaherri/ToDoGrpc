using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using ToDoGrpc.Data;
using ToDoGrpc.Models;

namespace ToDoGrpc.Services;

public class ToDoService : ToDo.ToDoBase
{
	private readonly AppDbCOntext _dbContext;

	public ToDoService(AppDbCOntext dbContext)
	{
		_dbContext = dbContext;
	}

	public override async Task<CreateToDoResponse> Create(CreateToDoRequest request, ServerCallContext context)
	{
		if (string.IsNullOrEmpty(request.Title) || string.IsNullOrEmpty(request.Description))
			throw new RpcException(new Status(statusCode: StatusCode.InvalidArgument, ""));

		var toDoItem = new ToDoItem
		{
			Title = request.Title,
			Description = request.Description,
		};

		await _dbContext.AddAsync(toDoItem);
		await _dbContext.SaveChangesAsync();

		return new CreateToDoResponse
		{
			Id = toDoItem.Id,
		};
	}

	public override async Task<GetSingleToDoRespose> GetSingle(GetSingleToDoRequest request, ServerCallContext context)
	{
		var toDo = await _dbContext.ToDoItems.FindAsync(request.Id);

		if (toDo is null)
			throw new RpcException(new Status(statusCode: StatusCode.NotFound, ""));

		return new GetSingleToDoRespose
		{
			Id = toDo.Id,
			Title = toDo.Title,
			Description = toDo.Description,
			Status = toDo.Status,
		};

	}

	public override async Task<GetAllToDosRespose> GetAll(GetAllToDosRequest request, ServerCallContext context)
	{
		var todos = await _dbContext.ToDoItems.ToListAsync();

		var result = new GetAllToDosRespose();

		foreach (var toDo in todos)
			result.ToDos.Add(new GetSingleToDoRespose
			{
				Id = toDo.Id,
				Title = toDo.Title,
				Description = toDo.Description,
				Status = toDo.Status,
			});

		return result;
	}

	public override async Task<UpdateToDoResponse> Update(UpdateToDoRequest request, ServerCallContext context)
	{
		if (string.IsNullOrEmpty(request.Title) || string.IsNullOrEmpty(request.Description))
			throw new RpcException(new Status(statusCode: StatusCode.InvalidArgument, ""));

		var toDo = await _dbContext.ToDoItems.FindAsync(request.Id);

		if (toDo is null)
			throw new RpcException(new Status(statusCode: StatusCode.NotFound, ""));

		toDo.Title = request.Title;
		toDo.Description = request.Description;
		toDo.Status = request.Status;

		await _dbContext.SaveChangesAsync();

		return new UpdateToDoResponse();
	}

	public override async Task<DeleteToDoResponse> Delete(DeleteToDoRequest request, ServerCallContext context)
	{
		var toDo = await _dbContext.ToDoItems.FindAsync(request.Id);

		if (toDo is null)
			throw new RpcException(new Status(statusCode: StatusCode.NotFound, ""));

		_dbContext.ToDoItems.Remove(toDo);
		await _dbContext.SaveChangesAsync();

		return new DeleteToDoResponse();
	}
}

