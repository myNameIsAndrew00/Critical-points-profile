﻿using MapWebSite.Controllers;
using MapWebSite.Core.Database;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace MapWebSite.Tests.Database
{
    [TestClass]
    public class SelectTest
    {
      
        [TestMethod]
        public void SelectPointDetails()
        {
            Repository.CassandraDataPointsRepository repository =
                Repository.CassandraDataPointsRepository.Instance;

            var res = repository.GetPointDetails(55, 3, new Model.PointBase() { Latitude = 44.44935m, Longitude = 26.15415m, Number = 5360 });
        }

        [TestMethod]
        public void SelectPalettes()
        {
            IUserRepository repository = new Repository.SQLUserRepository();
            var result = repository.GetColorMapsFiltered(ColorMapFilters.ColorMapName, "demo", 0, 5);
        }
    }
}