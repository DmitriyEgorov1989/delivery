namespace DeliveryApp.Infrastructure.Adapters.Postgres.Entities
{
    public sealed class OutboxMessage
    {
        /// <summary>
        /// Уникальный идентификатор сообщения
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Тип сообщения
        /// </summary>
        public string Type {  get; set; }

        /// <summary>
        /// Тело сообщения
        /// </summary>
        public string Content {  get; set; }

        /// <summary>
        /// Дата создания
        /// </summary>
        public DateTime OccurredOnUtc { get; set; }
        
        /// <summary>
        /// Дата публикации
        /// </summary>
        public DateTime? ProcessedOnUtc { get; set; }
    }
}