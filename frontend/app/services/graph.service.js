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


    function drawChart() {

      $http.get("api/graph/goal")
        .then(function (result) {
          //var data = convertToDate(result.data);//["6e25cee6-c6ee-49a3-b05c-f3df1302ea35"];

          var data = [result.data.header];

          for (var date in result.data) {
            if(date === "header") continue;
            data.push(_.flatten([date, result.data[date]]));
          }

          console.log(data);
          var table = google.visualization.arrayToDataTable(data);
          //var table = google.visualization.arrayToDataTable([
          //  ["Date", "test", "test2"],
          //  ["2000", 5, 3]
          //]);

          //table.addRows(mergeData(data));

          // Set chart options
          var options = {
            isStacked: true,
            legend: {position: 'top', maxLines: 3},
            title: 'Goals',
          //  hAxis: {title: 'Month',  titleTextStyle: {color: '#333'}},
            vAxis: {minValue: 0}
          };

          // Instantiate and draw our chart, passing in some options.
          var chart = new google.visualization.AreaChart(document.getElementById('chart_div'));
          chart.draw(table, options);
        });
    }

    function mergeData(data) {
      var rows = [];
      for (var d in data) {
        var temp = _.map(data[d].dataPoints, function (d) {return [d.label, d.value];});
        if(temp.length > 0)
          rows.push(temp);
      }
      console.log(rows);
      var minDate = _.minBy(rows, function (r) {
        return r[0][0];
      })[0][0];



      console.log(minDate);
      return rows
    }



    return {
      productionGraph: productionGraph
    };

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
        .then(function (data) {

          var result = {
            set: data[1],
            options: {
              series: [],
              axes: {
                x: {
                  type: 'date', // or 'date', or 'log'
                  key: "label"
                }
              }
            }
          };

          for (var user in data[1]) {
            result.options.series.push({
              axis: "y",
              dataset: user,
              key: "value",
              label: data[1][user].label,
              defined: function(value) {
                return value.y1 !== undefined;
              },
              color: "#1f77b4",
              type: ['line', 'dot', 'area'],
              id: user
            });
          }



          return result;
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
