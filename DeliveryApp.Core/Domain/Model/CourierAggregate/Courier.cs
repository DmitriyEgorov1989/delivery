using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Model.NewFolder;
using DeliveryApp.Core.Domain.Model.OrderAggregate;
using DeliveryApp.Core.Domain.Model.SharedKernel;
using Primitives;
using System.Diagnostics.CodeAnalysis;

namespace DeliveryApp.Core.Domain.Model.CourierAggregate
{
    public class Courier : Aggregate<Guid>
    {
        private static readonly StoragePlace storageDefault = StoragePlace.Create("Сумка", 10).Value;
        /// <summary>
        /// ctr
        /// </summary>
        [ExcludeFromCodeCoverage]
        private Courier() { }

        /// <summary>
        /// ctr
        /// </summary>
        /// <param name="name">Имя</param>
        /// <param name="speed">Скорость</param>
        /// <param name="location">Местоположение</param>
        /// <param name="storagePlace">Хранилище</param>
        private Courier(string name, int speed, Location location, StoragePlace storagePlace) : this()
        {
            Id = Guid.NewGuid();
            Name = name;
            Speed = speed;
            Location = location;
            StoragePlaces = [storagePlace];
        }

        /// <summary>
        /// Имя
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// скорость
        /// </summary>
        public int Speed { get; }

        /// <summary>
        /// Местоположение
        /// </summary>
        public Location Location { get; private set; }

        /// <summary>
        /// Список хранилищ
        /// </summary>
        public List<StoragePlace> StoragePlaces { get; }

        /// <summary>
        /// Fabric Method
        /// </summary>
        /// <param name="name">Имя</param>
        /// <param name="speed">Скорость</param>
        /// <param name="location">Местоположение</param>
        /// <returns></returns>
        public static Result<Courier, Error> Create(string name, int speed, Location location)
        {
            if (name == null || name == "")
            {
                return GeneralErrors.ValueIsInvalid(nameof(name));
            }

            if (speed <= 0)
            {
                return GeneralErrors.ValueIsRequired(nameof(speed));
            }

            if (location == null)
            {
                return GeneralErrors.ValueIsInvalid(nameof(location));
            }

            return new Courier(name, speed, location, storageDefault);
        }

        /// <summary>
        /// Добавление хранилища курьеру
        /// </summary>
        /// <param name="name">Название</param>
        /// <param name="volume">Обьем</param>
        /// <returns></returns>
        public UnitResult<Error> AddStoragePlace(string name, int volume)
        {
            if (name == null || name == "")
            {
                return GeneralErrors.ValueIsInvalid(nameof(name));
            }

            if (volume <= 0)
            {
                return GeneralErrors.ValueIsRequired(nameof(volume));
            }

            var storagePlace = StoragePlace.Create(name, volume);

            if (storagePlace.IsFailure)
            {
                return storagePlace.Error;
            }
            StoragePlaces.Add(storagePlace.Value);

            return UnitResult.Success<Error>();
        }

        /// <summary>
        /// Проверяем можно ли положить в одно из хранилищ
        /// </summary>
        /// <param name="order">Заказ</param>
        /// <returns></returns>
        public Result<bool, Error> CanTakeOrder(Order order)
        {
            if (order == null)
            {
                return GeneralErrors.ValueIsInvalid(nameof(order));
            }
            return StoragePlaces.Any(sp => sp.CanStore(order.Volume).IsSuccess);
        }

        /// <summary>
        /// Кладем заказ в хранилище
        /// </summary>
        /// <param name="order">Заказ</param>
        /// <returns></returns>
        public UnitResult<Error> TakeOrder(Order order)
        {
            if (order == null)
            {
                return GeneralErrors.ValueIsInvalid(nameof(order));
            }
            var storagePlace = StoragePlaces.FirstOrDefault(sp => CanTakeOrder(order).IsSuccess);

            if (storagePlace is null)
            {
                return UnitResult.Failure<Error>(GeneralErrors.NotFound());
            }
            storagePlace.Store(order.Id, order.Volume);

            return UnitResult.Success<Error>();
        }

        /// <summary>
        /// Завершаем заказ
        /// </summary>
        /// <param name="order">Заказ</param>
        /// <returns></returns>
        public UnitResult<Error> СompleteOrder(Order order)
        {
            if (order == null)
            {
                return GeneralErrors.ValueIsInvalid(nameof(order));
            }
            var storagePlace = StoragePlaces.FirstOrDefault(sp => sp.OrderId == order.Id);

            if (storagePlace is null)
            {
                return UnitResult.Failure<Error>(GeneralErrors.NotFound());
            }
            storagePlace.Clear(order.Id);

            return UnitResult.Success<Error>();
        }

        /// <summary>
        /// Высчитываем количество шагов до заказчика
        /// </summary>
        /// <param name="target">локация заказчика</param>
        /// <returns></returns>
        public Result<double, Error> CalculateTimeToLocation(Location target)
        {
            if (target == null)
            {
                return GeneralErrors.ValueIsInvalid(nameof(target));
            }
            var distance = Location.DistanceTo(target);

            if (distance.IsFailure)
            {
                return distance.Error;
            }
            return distance.Value / (double)Speed;
        }

        /// <summary>
        /// Изменить местоположение
        /// </summary>
        /// <param name="target">Целевое местоположение</param>
        /// <returns>Местоположение после сдвига</returns>
        public UnitResult<Error> Move(Location target)
        {
            if (target == null)
            {
                return GeneralErrors.ValueIsRequired(nameof(target));
            }
            var difX = target.X - Location.X;
            var difY = target.Y - Location.Y;
            var cruisingRange = Speed;

            var moveX = Math.Clamp(difX, -cruisingRange, cruisingRange);
            cruisingRange -= Math.Abs(moveX);

            var moveY = Math.Clamp(difY, -cruisingRange, cruisingRange);

            var locationCreateResult = Location.Create(Location.X + moveX, Location.Y + moveY);
            if (locationCreateResult.IsFailure)
            {
                return locationCreateResult.Error;
            }
            Location = locationCreateResult.Value;

            return UnitResult.Success<Error>();
        }
    }
}