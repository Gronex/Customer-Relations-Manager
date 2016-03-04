(function() {
  'use strict';

  angular
    .module('CRM')
    .service('graph', graph );

  graph.$inject = ['dataservice', '$q'];

  /* @ngInject */
  function graph (dataservice, $q) {

    google.charts.load('current', {'packages':['corechart', 'table']});

    return {
      productionGraph: productionGraph,
      drawChart: drawChart,
      drawTable: drawTable
    };

    function drawChart(data, options, divId) {

      google.charts.setOnLoadCallback(function () {
        if(!divId) divId = 'chart_div';
        var table = google.visualization.arrayToDataTable(data);
        // Instantiate and draw our chart, passing in some options.
        var chart = new google.visualization.ComboChart(document.getElementById(divId));
        chart.draw(table, options);
      });
    }

    function drawTable(data, options, divId) {
      google.charts.setOnLoadCallback(function () {
        if(!divId) divId = 'table_div';
        var table = google.visualization.arrayToDataTable(data);
        // Instantiate and draw our chart, passing in some options.
        var chart = new google.visualization.Table(document.getElementById(divId));
        chart.draw(table, options);
      });
    }

    function productionGraph(config) {
      var production = dataservice.graph
            .get({id: "production", query: config})
            .then(function (result) {
              return convertToDate(result);
            });

      var goals = dataservice.graph
            .get({id: "goal", query: config})
            .then(function (result) {
              return convertToDate(result);
            });

      return $q.all([production, goals])
        .then(function (result) {
          var data = [];
          for (var date in result[0]) {
            if(date === "header")
              data.push(_.flatten(["Month", result[0][date], "Goal"]));
            else
              data.push(_.flatten([date, result[0][date], _.sum(result[1][date])]));
          }

          var goalSeries = {};
          goalSeries[data[0].length - 2] = {type: 'line'};

          return {
            data: data,
            graphOptions: {
              title: 'Production',
              isStacked: true,
              legend: {position: 'top', maxLines: 3},
              hAxis: {title: "Time"},
              vAxis: {title: "DKK"},
              seriesType: "area",
              series: goalSeries,
              crosshair: { trigger: 'both' }
            },
            tableOptions: {
              width: '100%',
              height: '100%'
            }
          };
        });
    }

    function convertToDate(set) {
      for(var key in set){
        _.map(set[key].dataPoints, function (point) {
          point.label = new Date(point.label);
          return point;
        });
      }
      return set;
    }
  }
})();
