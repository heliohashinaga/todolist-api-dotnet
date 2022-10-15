namespace TodoListApi.Models
{
    internal class Todo
    {
        public Guid Id { get; set; }
        public string? Description { get; set; }
        public bool IsComplete { get; set; }
    }
}
