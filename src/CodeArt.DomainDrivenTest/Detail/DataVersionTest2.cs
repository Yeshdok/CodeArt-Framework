﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Data;

using Dapper;

using CodeArt.TestTools;
using CodeArt.DomainDriven;
using CodeArt.DomainDriven.DataAccess;
using CodeArt.Runtime;
using CodeArt.Concurrent;
using CodeArt.Util;

namespace CodeArt.DomainDrivenTest.Detail
{
    /// <summary>
    /// 在领域对象中，对象的属性以及子成员发生了改变，数据版本号都会改变
    /// </summary>
    [TestClass]
    public class DataVersionTest2 : DomainStage
    {
        protected override void PreEnterScene()
        {
            DataPortal.RuntimeBuild();
        }

        protected override void LeftScene()
        {
            DataPortal.Dispose();
        }

        protected override void EnteredScene()
        {
            this.BeginTransaction();
            Car car = CreateCar();

            var repository = Repository.Create<ICarRepository>();
            repository.Add(car);

            this.Commit();

            this.Fixture.Add(car);
        }

        #region 复杂根Car测试

        [TestMethod]
        public void CarTest1()
        {
            var car = this.Fixture.Get<Car>() as Car;
            CheckCarDataVersion(car.Id, 1);

            car.ErrorMessages = new List<string> { "myerror1", "myerror2" };

            UpdateCar(car);

            var carNew = FindCar(car.Id);
            var errorMessages = ((string)carNew.ErrorMessages).Split(',');

            Assert.AreEqual(errorMessages.Length, 2);
            Assert.AreEqual(((string)(errorMessages.ElementAt(0))).FromBase64(), "myerror1");
            Assert.AreEqual(((string)(errorMessages.ElementAt(1))).FromBase64(), "myerror2");

            CheckCarDataVersion(car.Id, 2);
        }

        [TestMethod]
        public void CarTest2()
        {
            var car = this.Fixture.Get<Car>() as Car;
            CheckCarDataVersion(car.Id, 1);

            car.ErrorMessages = new List<string> { "error1", "error2" };

            UpdateCar(car);

            var carNew = FindCar(car.Id);
            var errorMessages = ((string)carNew.ErrorMessages).Split(',');

            Assert.AreEqual(errorMessages.Length, 2);
            Assert.AreEqual(((string)(errorMessages.ElementAt(0))).FromBase64(), "error1");
            Assert.AreEqual(((string)(errorMessages.ElementAt(1))).FromBase64(), "error2");

            CheckCarDataVersion(car.Id, 2);
        }

        [TestMethod]
        public void CarTest3()
        {
            var car = this.Fixture.Get<Car>() as Car;
            CheckCarDataVersion(car.Id, 1);

            car.LightCounts.Add(20);
            car.LightCounts.Add(21);

            UpdateCar(car);

            var carNew = FindCar(car.Id);

            var lightCounts = ((string)carNew.LightCounts).Split(',');

            Assert.AreEqual(lightCounts.Length, 4);

            Assert.AreEqual(((string)(lightCounts.ElementAt(2))).FromBase64(), "20");
            Assert.AreEqual(((string)(lightCounts.ElementAt(3))).FromBase64(), "21");

            CheckCarDataVersion(car.Id, 2);

            car.LightCounts.Remove(21);
            UpdateCar(car);

            var carNew2 = FindCar(car.Id);

            var lightCounts2 = ((string)carNew2.LightCounts).Split(',');

            Assert.AreEqual(lightCounts2.Length, 3);
            Assert.AreEqual(((string)(lightCounts2.ElementAt(0))).FromBase64(), "1");

            CheckCarDataVersion(car.Id, 3);

        }

        [TestMethod]
        public void CarTest4()
        {
            var car = this.Fixture.Get<Car>() as Car;
            CheckCarDataVersion(car.Id, 1);

            car.AddDeliveryDate(new DateTime(2016, 7, 1));

            UpdateCar(car);

            Car carMemmory = GetCar(car.Id);

            Assert.AreEqual(carMemmory.DeliveryDates.Count(), 4);

            Assert.AreEqual(carMemmory.DeliveryDates.ElementAt(0).GetType().ToString(), typeof(Emptyable<DateTime>).ToString());

            var carNew = FindCar(car.Id);

            var dates = ((string)carNew.DeliveryDates).Split(',');
            Assert.AreEqual(dates.Length, 4);

            Assert.AreEqual(((string)(dates.ElementAt(0))).FromBase64(), "2016/6/1 00:00:00");
            Assert.AreEqual(((string)(dates.ElementAt(1))).FromBase64(), "2016/6/2 00:00:00");
            Assert.AreEqual(((string)(dates.ElementAt(2))).FromBase64(), "2016/6/3 00:00:00");
            Assert.AreEqual(((string)(dates.ElementAt(3))).FromBase64(), "2016/7/1 00:00:00");

            CheckCarDataVersion(car.Id, 2);

        }

        [TestMethod]
        public void CarTest5()
        {
            var car = this.Fixture.Get<Car>() as Car;
            CheckCarDataVersion(car.Id, 1);

            car.RemoveDeliveryDate(car.DeliveryDates.ElementAt(1));

            UpdateCar(car);

            Car carMemmory = GetCar(car.Id);

            Assert.AreEqual(carMemmory.DeliveryDates.Count(), 2);
            Assert.AreEqual(carMemmory.DeliveryDates.ElementAt(0).GetType().ToString(), typeof(Emptyable<DateTime>).ToString());

            var carNew = FindCar(car.Id);

            var dates = ((string)carNew.DeliveryDates).Split(',');
            Assert.AreEqual(dates.Length, 2);

            Assert.AreEqual(((string)(dates.ElementAt(0))).FromBase64(), "2016/6/1 00:00:00");
            Assert.AreEqual(((string)(dates.ElementAt(1))).FromBase64(), "2016/6/3 00:00:00");

            CheckCarDataVersion(car.Id, 2);
        }

        [TestMethod]
        public void CarTest6()
        {
            var car = this.Fixture.Get<Car>() as Car;
            CheckCarDataVersion(car.Id, 1);

            car.AllColor = new WholeColor("白色", 9, false);

            UpdateCar(car);

            Car carMemmory = GetCar(car.Id);

            Assert.AreEqual(carMemmory.AllColor.Name, "白色");
            Assert.AreEqual(carMemmory.AllColor.ColorNum,9);
            Assert.AreEqual(carMemmory.AllColor.IsPainted, false);

            var carNew = FindCar(car.Id);

            CheckCarDataVersion(car.Id, 2);
        }

        [TestMethod]
        public void CarTest7()
        {
            var car = this.Fixture.Get<Car>() as Car;
            CheckCarDataVersion(car.Id, 1);

            Assert.AreEqual(car.CarAccessories.Count(), 3);

            List<CarAccessory> accessories = new List<CarAccessory>()
            {
                new CarAccessory("修改配饰1", 20, new DateTime(2017, 8, 1)),
                new CarAccessory("修改配饰2", 21, new DateTime(2017, 8, 2)),
            };

            car.CarAccessories = accessories;

            UpdateCar(car);

            Car carMemmory = GetCar(car.Id);

            Assert.AreEqual(carMemmory.CarAccessories.Count(), 2);
            var date = carMemmory.CarAccessories.ElementAt(0).SetupDate;

            Assert.AreEqual(date.Value, new DateTime(2017, 8, 1));
            Assert.AreEqual(date.GetType().ToString(), typeof(Emptyable<DateTime>).ToString());

            var carNew = FindCar(car.Id);

            CheckCarDataVersion(car.Id, 2);
        }

        [TestMethod]
        public void CarTest8()
        {
            var car = this.Fixture.Get<Car>() as Car;
            CheckCarDataVersion(car.Id, 1);

            car.AddCarAccessory(new CarAccessory("修改配饰1", 20, new DateTime(2017, 8, 1)));
            car.AddCarAccessory(new CarAccessory("修改配饰2", 21, new DateTime(2017, 8, 2)));

            UpdateCar(car);

            Car carMemmory = GetCar(car.Id);

            Assert.AreEqual(carMemmory.CarAccessories.Count(), 5);
            var date = carMemmory.CarAccessories.ElementAt(4).SetupDate;

            Assert.AreEqual(date.Value, new DateTime(2017, 8, 2));
            Assert.AreEqual(date.GetType().ToString(), typeof(Emptyable<DateTime>).ToString());

            var carNew = FindCar(car.Id);

            CheckCarDataVersion(car.Id, 2);

            carMemmory.AddCarAccessory(new CarAccessory("修改配饰3", 22, new DateTime(2017, 8, 3)));

            UpdateCar(carMemmory);

            Car carMemmory2 = GetCar(car.Id);

            Assert.AreEqual(carMemmory2.CarAccessories.Count(), 6);

        }

        [TestMethod]
        public void CarTest9()
        {
            var car = this.Fixture.Get<Car>() as Car;
            CheckCarDataVersion(car.Id, 1);

            car.RemoveCarAccessory(car.CarAccessories.ElementAt(0));

            UpdateCar(car);

            Car carMemmory = GetCar(car.Id);

            Assert.AreEqual(carMemmory.CarAccessories.Count(), 2);
            var date = carMemmory.CarAccessories.ElementAt(1).SetupDate;

            Assert.AreEqual(date.Value, new DateTime(2017, 6, 3));
            Assert.AreEqual(date.GetType().ToString(), typeof(Emptyable<DateTime>).ToString());

            var carNew = FindCar(car.Id);

            CheckCarDataVersion(car.Id, 2);
        }

        [TestMethod]
        public void CarTest10()
        {
            var car = this.Fixture.Get<Car>() as Car;
            CheckCarDataVersion(car.Id, 1);

            // 修改
            CarWheel wheel = new CarWheel(10)
            {
                OrderIndex = 1,
                Description = "modefy main CarWheel",
                TheColor = new WholeColor("蓝色", 6, false)
            };

            car.MainWheel = wheel;

            UpdateCar(car);

            Car carMemmory = GetCar(car.Id);

            Assert.AreEqual(carMemmory.MainWheel.Id, 10);

            Assert.AreEqual(carMemmory.MainWheel.OrderIndex, 1);
            Assert.AreEqual(carMemmory.MainWheel.Description, "modefy main CarWheel");
            Assert.AreEqual(carMemmory.MainWheel.TheColor.Name, "蓝色");

            var carNew = FindCar(car.Id);

            CheckCarDataVersion(car.Id, 2);
        }

        [TestMethod]
        public void CarTest11()
        {
            var car = this.Fixture.Get<Car>() as Car;
            CheckCarDataVersion(car.Id, 1);

            // 修改
            car.MainWheel.Description = "modefy main CarWheel";
            car.MainWheel.OrderIndex = 3;

            UpdateCar(car);

            Car carMemmory = GetCar(car.Id);

            Assert.AreEqual(carMemmory.MainWheel.Id, 1);

            Assert.AreEqual(carMemmory.MainWheel.OrderIndex, 3);
            Assert.AreEqual(carMemmory.MainWheel.Description, "modefy main CarWheel");

            var carNew = FindCar(car.Id);

            CheckCarDataVersion(car.Id, 2);
        }

        [TestMethod]
        public void CarTest12()
        {
            var car = this.Fixture.Get<Car>() as Car;
            CheckCarDataVersion(car.Id, 1);

            // 修改
            car.MainWheel.TheColor = new WholeColor("蓝色", 6, false);

            UpdateCar(car);

            Car carMemmory = GetCar(car.Id);

            Assert.AreEqual(carMemmory.MainWheel.Id, 1);

            Assert.AreEqual(carMemmory.MainWheel.TheColor.Name, "蓝色");

            var carNew = FindCar(car.Id);

            CheckCarDataVersion(car.Id, 2);
        }

        [TestMethod]
        public void CarTest13()
        {
            var car = this.Fixture.Get<Car>() as Car;
            CheckCarDataVersion(car.Id, 1);

            // 修改
            CarWheel wheel = new CarWheel(4)
            {
                OrderIndex = 13,
                Description = "theCarWheel3",
                TheColor = new WholeColor("绿色", 9, false)
            };

            car.AddCarWheel(wheel);

            UpdateCar(car);

            Car carMemmory = GetCar(car.Id);

            Assert.AreEqual(carMemmory.Wheels.Count(), 3);

            Assert.AreEqual(carMemmory.Wheels.ElementAt(2).TheColor.Name, "绿色");

            var carNew = FindCar(car.Id);

            CheckCarDataVersion(car.Id, 2);
        }

        #endregion

        #region Car测试工具

        private Car CreateCar()
        {
            Car car = new Car(Guid.NewGuid());

            car.Name = "宝马i7";
            car.IsNewCar = true;

            car.LightCounts.Add(1);
            car.LightCounts.Add(2);

            car.ErrorMessages = new List<string> {"error1", "error2"};

            car.DeliveryDates = new List<Emptyable<DateTime>> {
                new DateTime(2016, 6, 1),
                new DateTime(2016, 6, 2),
                new DateTime(2016, 6, 3)
            };

            car.AllColor = new WholeColor("颜色", 8, true);

            List<CarAccessory> accessories = new List<CarAccessory>()
            {
                new CarAccessory("配饰1", 10, new DateTime(2017, 6, 1)),
                new CarAccessory("配饰2", 11, new DateTime(2017, 6, 2)),
                new CarAccessory("配饰3", 12, new DateTime(2017, 6, 3))
            };

            car.CarAccessories = accessories;

            car.MainWheel = new CarWheel(1)
            {
                OrderIndex = 1,
                Description = "the main CarWheel",
                TheColor = new WholeColor("主色", 5, true)
            };

            CarWheel wheel1 = new CarWheel(2)
            {
                OrderIndex = 11,
                Description = "theCarWheel1",
                TheColor = new WholeColor("红色", 6, true)
            };

            CarWheel wheel2 = new CarWheel(3)
            {
                OrderIndex = 12,
                Description = "theCarWheel2",
                TheColor = new WholeColor("蓝色", 7, true)
            };

            car.AddCarWheel(wheel1);
            car.AddCarWheel(wheel2);

            return car;
        }

        private void UpdateCar(Car car)
        {
            this.BeginTransaction();

            var repository = Repository.Create<ICarRepository>();
            repository.Update(car);

            this.Commit();
        }

        private dynamic FindCar(Guid id)
        {
            dynamic car = null;
            DataPortal.Direct<Car>((conn) =>
            {
                var data = conn.QuerySingle("select * from Car where id=@id", new { Id = id });
                car = data;
            });

            return car;
        }

        private Car GetCar(Guid id)
        {
            var repository = Repository.Create<ICarRepository>();
            return repository.Find(id, QueryLevel.None);
        }

        private void CheckCarDataVersion(Guid id, int dataVersion)
        {
            DataPortal.Direct<Car>((conn) =>
            {
                var data = conn.QuerySingle("select * from Car where id=@id", new { Id = id });
                Assert.AreEqual(data.DataVersion, dataVersion);
            });
        }

        #endregion
    }
}
