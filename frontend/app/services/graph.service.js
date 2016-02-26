(function() {
  'use strict';

  angular
    .module('CRM')
    .service('graph', graph );

  graph.$inject = ['$http', '$q'];

  /* @ngInject */
  function graph ($http, $q) {

    google.charts.load('current', {'packages':['corechart']});
    google.charts.setOnLoadCallback(drawChart);



    return {
      productionGraph: productionGraph,
      drawChart: drawChart
    };

    function drawChart(divId) {
      if(!divId) divId = 'chart_div';
      productionGraph()
      .then(function (result) {
        var table = google.visualization.arrayToDataTable(result.data);
        // Instantiate and draw our chart, passing in some options.
        var chart = new google.visualization.AreaChart(document.getElementById(divId));
        chart.draw(table, result.options);
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
            data.push(_.flatten([date, result[0][date]]));
          }

          console.log(result[1]);

          var res = {
            data: data,
            options: {
              isStacked: true,
              legend: {position: 'top', maxLines: 3},
              title: 'Goals',
            //  hAxis: {title: 'Month',  titleTextStyle: {color: '#333'}},
              vAxis: {minValue: 0}
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
