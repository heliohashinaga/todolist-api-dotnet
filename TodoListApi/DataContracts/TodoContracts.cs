namespace TodoListApi.DataContracts
{
    internal record CreateTodo(string Description) { }

    internal record UpdateTodo(string Description, bool IsComplete) { }
}
