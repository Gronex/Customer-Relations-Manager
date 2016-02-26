(function() {
  'use strict';

  angular
    .module('CRM')
    .service('graph', graph );

  graph.$inject = ['$http', '$q'];

  /* @ngInject */
  function graph ($http, $q) {

    google.charts.load('current', {'packages':['corechart']});

    return {
      productionGraph: productionGraph,
      drawChart: drawChart
    };

    function drawChart(data, options, divId) {

      google.charts.setOnLoadCallback(function () {
        if(!divId) divId = 'chart_div';
        var table = google.visualization.arrayToDataTable(data);
        // Instantiate and draw our chart, passing in some options.
        var chart = new google.visualization.AreaChart(document.getElementById(divId));
        chart.draw(table, options);
      });
    }

    function productionGraph() {
      var production = $http.get("api/graph/production")
        .then(function (result) {
          return convertToDate(result.data);
        });

      var goals = $http.get("api/graph/goal")
        .then(function (result) {
          return convertToDate(result.data);
        });

      return $q.all([production, goals])
        .then(function (result) {
          var data = [];
          for (var date in result[0]) {
            if(date === "header"){
              data.push(_.flatten([date, result[0][date], "Goal"]));
            }
            else
              data.push(_.flatten([date, result[0][date], _.sum(result[1][date])]));

          }


          var goalSeries = {};

          goalSeries[data[0].length - 2] = {type: 'line'}
          console.log(goalSeries);

          var res = {
            data: data,
            options: {
              isStacked: true,
              legend: {position: 'top', maxLines: 3},
              title: 'Goals',
              hAxis: {title: "Time"},
              vAxis: {minValue: 0, title: "DKK"},
              seriesType: "area",
              series: goalSeries
            }
          };
          return res;
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
