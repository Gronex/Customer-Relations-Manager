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
        // Instantiate and draw our chart, passing in some options.
        var chart = new google.visualization.ComboChart(document.getElementById(divId));
        chart.draw(data, options);
      });
    }

    function drawTable(data, options, divId) {
      google.charts.setOnLoadCallback(function () {
        if(!divId) divId = 'table_div';
        // Instantiate and draw our chart, passing in some options.
        var chart = new google.visualization.Table(document.getElementById(divId));
        chart.draw(data, options);
      });
    }

    function handleProductionData(data, keys, startDate, endDate){
      var result = [];

      for(var key of keys){
        data[key] = _.map(data[key], function(p){
          p.period = new Date(p.period);
          return p;
        });
      }

      var date = startDate;
      while(date < endDate){
        var row = [];
        row.push(date);
        for(key of keys){
          var valueHolder = _.find(data[key], function(p){return p.period.getTime() === date.getTime();});
          if(valueHolder !== undefined)
            row.push(valueHolder.value);
          else row.push(null);
        }
        result.push(row);
        date.setMonth(date.getMonth() + 1);
      }
      return result;
    }

    function productionGraph(config) {
      var headers = [];
      var production = dataservice.graph
            .get({id: "production", query: config});

      var goals = dataservice.graph
            .get({id: "goal", query: config});

      return $q.all({production: production, goals: goals})
        .then(function (result) {

          var data = new google.visualization.DataTable();
          data.addColumn('date', 'Month');

          var emails = Object.keys(result.goals);
          for (var email of emails) {
            var user = _.head(result.goals[email]).user;
            data.addColumn('number', user.firstName + " " + user.lastName);
          }
          console.log(handleProductionData(result.production, emails, config.startDate, config.endDate));
          data.addRows(handleProductionData(result.production, emails, config.startDate, config.endDate));

          var goalSeries = {};
          goalSeries[emails.length - 1] = {type: 'line'};

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
  }
})();
