﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtoCommerce.Search.Providers.Azure
{
    using RedDog.Search.Model;

    using VirtoCommerce.Foundation.Catalogs.Search;
    using VirtoCommerce.Foundation.Search;

    public class AzureSearchQueryBuilder : ISearchQueryBuilder
    {
        public object BuildQuery(ISearchCriteria criteria)
        {
            var builder = new SearchQuery();
            var filterBuilder = new StringBuilder();
            var queryBuilder = new StringBuilder();

            builder.Skip(criteria.StartingRecord);
            builder.Top(criteria.RecordsToRetrieve);
            builder.Count(true);

            #region Sorting

            // Add sort order
            if (criteria.Sort != null)
            {
                var fields = criteria.Sort.GetSort();
                foreach (var field in fields)
                {
                    builder.OrderBy = String.Format("{0}{1}{2}", String.IsNullOrEmpty(builder.OrderBy) ? "" : builder.OrderBy + ",", field.FieldName, field.IsDescending ? " desc" : "");
                }
            }

            #endregion

            #region CatalogItemSearchCriteria
            if (criteria is CatalogItemSearchCriteria)
            {
                var c = criteria as CatalogItemSearchCriteria;

                if (!String.IsNullOrEmpty(c.SearchPhrase))
                {
                    queryBuilder.Query(c.SearchPhrase);
                }


                filterBuilder.Filter("startdate", c.StartDate, "lt");

                /*
                if (c.StartDateFrom.HasValue)
                {
                    mainQuery.Must(m => m
                        .Range(r => r.Field("startdate").From(c.StartDateFrom.Value.ToString("s")))
                   );
                }
                 * */

                if (c.EndDate.HasValue)
                {
                    //filterBuilder.Filter("enddate", c.EndDate.Value, "gt");
                }

                filterBuilder.Filter("sys__hidden", "false");

                if (c.Outlines != null && c.Outlines.Count > 0)
                {
                    queryBuilder.Query("sys__outline", c.Outlines.OfType<string>().ToArray());
                }


                if (!String.IsNullOrEmpty(c.Catalog))
                {
                    filterBuilder.FilterContains("catalog", c.Catalog);
                }

                

            }
            #endregion

            builder.Filter = filterBuilder.ToString();
            builder.Query = queryBuilder.ToString();

            return builder;
        }
    }
}
