﻿using FluentSqlBuilder.Core.Middlewares.Inputs;
using FluentSqlBuilder.Core.Middlewares.Services;
using FluentSqlBuilder.Data.DataModel;
using SqlBuilderFluent.Types;
using Xunit;

namespace FluentSqlBuilder.Test.Unit
{
    public class SelectFullTest
    {
        private readonly static FluentSqlBuilderMiddlewareOptions _fluentSqlBuilderMiddlewareOptions = new FluentSqlBuilderMiddlewareOptions()
        {
            SqlAdapterType = SqlAdapterType.SqlServer2019,
            Formatting = SqlBuilderFormatting.Indented
        };

        [Fact]
        public void Test_Select_OrderBy_Desc_Without_Alias()
        {
            // Arrange
            var tableOrderName = "order";
            var tableCustomerName = "customer";
            var tableNameOrderAlias = "order_alias";
            var tableNameCustomerAlias = "customer_alias";
            var limit = 10;
            var sqlBuilderWithoutAlias = new FluentSqlBuilderService(_fluentSqlBuilderMiddlewareOptions)
                                             .From<OrderDataModel>()
                                             .Projection((order) => order.CustomerId)
                                             .Where(x => x.Status == OrderStatus.Paid && x.CustomerId == 1)
                                             .InnerJoin<CustomerDataModel>((order, customer) => order.CustomerId == customer.Id)
                                             .OrderBy(x => x.Id)
                                             .Limit(limit);

            var sqlBuilderAlias = new FluentSqlBuilderService(_fluentSqlBuilderMiddlewareOptions)
                                      .From<OrderDataModel>(tableNameOrderAlias)
                                      .Projection((order) => order.CustomerId, tableNameOrderAlias)
                                      .Where(x => x.Status == OrderStatus.Paid && x.CustomerId == 1)
                                      .InnerJoin<CustomerDataModel>((order, customer) => order.CustomerId == customer.Id, tableNameCustomerAlias)
                                      .OrderBy(x => x.Id)
                                      .Limit(limit);

            // Act
            var sqlSelectWithoutAlias = sqlBuilderWithoutAlias.ToString();
            var sqlSelectAlias = sqlBuilderAlias.ToString();

            // Assert
            Assert.True(sqlSelectWithoutAlias.Contains($"SELECT TOP({limit})"), $"SELECT invalid");
            Assert.True(sqlSelectWithoutAlias.Contains($"[{tableOrderName}].[customer_id] AS CustomerId"), $"Column not found");
            Assert.True(sqlSelectWithoutAlias.Contains($"FROM [checkout].[{tableOrderName}]"), $"FROM invalid");
            Assert.True(sqlSelectWithoutAlias.Contains($"INNER JOIN [customers].[{tableCustomerName}] ON ([{tableOrderName}].[customer_id] = [{tableCustomerName}].[Id])"), $"FROM invalid");
            Assert.True(sqlSelectWithoutAlias.Contains($"WHERE ([{tableOrderName}].[Status] = @Param1"), $"FROM invalid");
            Assert.True(sqlSelectWithoutAlias.Contains($"AND [{tableOrderName}].[customer_id] = @Param2)"), $"FROM invalid");
            Assert.True(sqlSelectWithoutAlias.Contains($"ORDER BY [{tableCustomerName}].[Id] ASC"), $"Column not found");

            Assert.True(sqlSelectAlias.Contains($"SELECT TOP({limit})"), $"SELECT invalid");
            Assert.True(sqlSelectAlias.Contains($"[{tableNameOrderAlias}].[customer_id] AS CustomerId"), $"Column not found");
            Assert.True(sqlSelectAlias.Contains($"FROM [checkout].[{tableOrderName}] AS {tableNameOrderAlias}"), $"FROM invalid");
            Assert.True(sqlSelectAlias.Contains($"INNER JOIN [customers].[{tableCustomerName}] AS {tableNameCustomerAlias} ON ([{tableNameOrderAlias}].[customer_id] = [{tableNameCustomerAlias}].[Id])"), $"FROM invalid");
            Assert.True(sqlSelectAlias.Contains($"WHERE ([{tableNameOrderAlias}].[Status] = @Param1"), $"FROM invalid");
            Assert.True(sqlSelectAlias.Contains($"AND [{tableNameOrderAlias}].[customer_id] = @Param2)"), $"FROM invalid");
            Assert.True(sqlSelectAlias.Contains($"ORDER BY [{tableNameOrderAlias}].[Id] ASC"), $"Column not found");
        }
    }
}