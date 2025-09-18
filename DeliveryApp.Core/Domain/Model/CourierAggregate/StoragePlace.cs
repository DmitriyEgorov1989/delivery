using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.Model.OrderAggregate;
using Primitives;
using System.Diagnostics.CodeAnalysis;

namespace DeliveryApp.Core.Domain.Model.NewFolder
{
    /// <summary>
    /// Хранилище заказа
    /// </summary>
    public class StoragePlace : Entity<Guid>
    {
        /// <summary>
        /// ctr
        /// </summary>
        [ExcludeFromCodeCoverage]
        private StoragePlace() { }

        /// <summary>
        /// ctr
        /// </summary>
        /// <param name="Название"></param>
        /// <param volume="объем"></param>
        private StoragePlace(string name, int volume)
        {
            Id = Guid.NewGuid();
            Name = name;
            TotalVolume = volume;
        }

        /// <summary>
        /// Название
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Общий обьем
        /// </summary>
        public int TotalVolume { get; private set; }

        /// <summary>
        /// Идентификационный номер заказа
        /// </summary>
        public Guid? OrderId { get; private set; }

        /// <summary>
        /// Создание места хранения
        /// </summary>
        /// <param name="Название"></param>
        /// <param volume="Обьем заказа"></param>
        /// <returns>
        public static Result<StoragePlace, Error> Create(string name, int volume)
        {
            if (name == null || name == string.Empty)
            {
                return GeneralErrors.ValueIsInvalid(nameof(name));
            }

            if (volume <= 0)
            {
                return GeneralErrors.ValueIsRequired(nameof(volume));
            }
            return new StoragePlace(name, volume);
        }

        /// <summary>
        /// Проверяем можно ли положить заказ в хранилище
        /// </summary>
        /// <param volume="Обьем заказа"></param>
        /// <returns></returns>
        public Result<bool, Error> CanStore(int volume)
        {
            if (volume <= 0)
            {
                return GeneralErrors.ValueIsRequired(nameof(volume));
            }

            if (IsOccupied())
            {
                return Errors.StorageIsOccupied();
            }

            if (volume > TotalVolume)
            {
                return Errors.StorageIsSmall();
            }

            return true;
        }

        /// <summary>
        /// Помещаем заказ в хранилище
        /// </summary>
        /// <param orderId="Id заказа"></param>
        /// <param volume="обьем заказа"></param>
        /// <returns></returns>
        public UnitResult<Error> Store(Guid? orderId, int volume)
        {
            if (orderId == null)
            {
                return GeneralErrors.ValueIsInvalid(nameof(orderId));
            }

            if (CanStore(volume).IsSuccess)
            {
                OrderId = orderId;
                TotalVolume = volume;
            }
            return UnitResult.Success<Error>();
        }

        /// <summary>
        /// Достаем заказ из хранилища
        /// </summary>
        /// <param orderId="Id заказа"></param>
        /// <returns></returns>
        public UnitResult<Error> Clear(Guid? orderId)
        {
            if (OrderId != orderId)
            {
                return GeneralErrors.NotFound();
            }

            if (orderId == null)
            {
                return GeneralErrors.ValueIsInvalid(nameof(orderId));
            }

            OrderId = null;

            return UnitResult.Success<Error>();
        }

        /// <summary>
        /// Проверка пустое ли хранилище
        /// </summary>
        /// <returns></returns>
        public bool IsOccupied()
        {
            return OrderId != null;
        }

        /// <summary>
        /// Ошибки при работе с хранилищем
        /// </summary>
        public static class Errors
        {
            public static Error StorageIsOccupied()
            {
                return new Error($"Storage {nameof(Name).ToLowerInvariant()}.is.occupied",
                    $"В хранилище уже лежит заказ");
            }

            public static Error StorageIsSmall()
            {
                return new Error($"Storage {nameof(Name).ToLowerInvariant()}.is.small",
                    $"Обьем хранилища меньше обьема заказа");
            }
        }
    }
}